﻿using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sia.Domain.Playbook;
using Sia.Playbook.Initialization;
using Sia.Playbook.Requests;
using Sia.Shared.Authentication;
using Sia.Shared.Middleware;
using Sia.Shared.Protocol;
using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Reflection;
using System.Threading.Tasks;

namespace Sia.Playbook
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            if (env.IsDevelopment())
            {
                builder.AddUserSecrets<Startup>();
            }
            Configuration = builder.Build();

            _env = env;
        }

        public IConfigurationRoot Configuration { get; }

        private readonly IHostingEnvironment _env;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            ConfigureAuth(services, Configuration);

            services.AddSession();

            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddScoped<IUrlHelper, UrlHelper>(iFactory
                    => new UrlHelper(iFactory.GetService<IActionContextAccessor>().ActionContext)
                );

            services.AddMvc(options =>
            {
                options.OutputFormatters.Insert(0, new PartialSerializedJsonOutputFormatter(
                        new MvcJsonOptions().SerializerSettings,
                        ArrayPool<char>.Shared));
            });
            services
                .AddAuthentication(authOptions =>
                {
                    authOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    authOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(jwtOptions =>
                {
                    jwtOptions.Authority = String.Format(Configuration["AzureAd:AadInstance"], Configuration["AzureAd:Tenant"]);
                    jwtOptions.Audience = Configuration["ClientId"];
                    jwtOptions.SaveToken = true;
                });
            var eventTypeIndexSingleton = new ConcurrentDictionary<long, EventType>();
            services.AddSingleton(context => eventTypeIndexSingleton);
            services.AddMediatR(typeof(GetEventTypeRequest).GetTypeInfo().Assembly);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, 
            IHostingEnvironment env, 
            ILoggerFactory loggerFactory,
            ConcurrentDictionary<long, EventType> eventTypeIndex)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment() || env.IsStaging()) { app.UseDeveloperExceptionPage(); }
            else { app.UseMiddleware<ExceptionHandler>(); }
            
            app.UseAuthentication();
            app.UseSession();
            app.UseMvc();

            var token = Configuration["GitHub:Token"];
            var name = Configuration["GitHub:Repository:Name"];
            var owner = Configuration["GitHub:Repository:Owner"];

            Task dataAddTask;
            if (string.IsNullOrWhiteSpace(token)
                || string.IsNullOrWhiteSpace(name)
                || string.IsNullOrWhiteSpace(owner))
            {
                //Load from method in source control
                //dataAddTask = context.AddSeedData();
            }
            else
            {
                //Load from configured git repository
                dataAddTask = eventTypeIndex.AddSeedDataFromGitHub(
                    loggerFactory,
                    LoadDataFromGitHub.GetAuthenticatedClient(token),
                    name,
                    owner);
                Task.WaitAll(dataAddTask);
            }
        }

        private static void ConfigureAuth(IServiceCollection services, IConfigurationRoot config)
        {
            var incidentAuthConfig = new AzureActiveDirectoryAuthenticationInfo("unused placeholder", "Unused placeholder", "Unused placeholder", config["AzureAd:Tenant"]);
            services.AddSingleton<AzureActiveDirectoryAuthenticationInfo>(i => incidentAuthConfig);
        }
    }
}
