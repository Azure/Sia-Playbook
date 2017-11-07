using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sia.Data.Playbooks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using System.Buffers;
using Microsoft.AspNetCore.Mvc.Routing;
using Sia.Shared.Protocol;
using MediatR;
using Sia.Playbook.Requests;
using System.Reflection;
using Sia.Data.Playbook;
using Sia.Playbook.Initialization;
using Sia.Shared.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Sia.Shared.Middleware;
using System.Collections.Concurrent;
using Sia.Domain.Playbook;

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

            if (env.IsDevelopment()) { app.UseDeveloperExceptionPage(); }
            else { app.UseMiddleware<ExceptionHandler>(); }
            
            app.UseAuthentication();
            app.UseSession();
            app.UseMvc();

            AutoMapperStartup.InitializeAutomapper();

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
                dataAddTask = eventTypeIndex.AddSeedDataFromGitHub(token, name, owner);
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
