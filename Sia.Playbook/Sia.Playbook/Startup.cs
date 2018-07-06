using MediatR;
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
using Sia.Core.Authentication;
using Sia.Core.Middleware;
using Sia.Core.Protocol;
using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Reflection;
using System.Threading.Tasks;
using System.Globalization;
using Sia.Playbook.Configuration;
using Sia.Core.Configuration.Sources.GitHub;

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
            PlaybookConfig = Configuration.Get<PlaybookConfig>();
            _env = env;
        }

        public PlaybookConfig PlaybookConfig { get; private set; }
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
                    jwtOptions.Authority = String.Format(CultureInfo.InvariantCulture, Configuration["AzureAd:AadInstance"], Configuration["AzureAd:Tenant"]);
                    jwtOptions.Audience = Configuration["ClientId"];
                    jwtOptions.SaveToken = true;
                });

            var playbookData = new PlaybookData();
            playbookData.RegisterData(services);

            services.AddMediatR(typeof(GetEventTypeRequest).GetTypeInfo().Assembly);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, 
            IHostingEnvironment env, 
            ILoggerFactory loggerFactory,
            PlaybookData playbookData)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            if (Configuration.GetSection("Github") != null)
            {
                var configFinalizeTask = PlaybookConfig.GitHub.EnsureValidTokenAsync(PlaybookConfig, PlaybookConfig.GithubTokenName);
                configFinalizeTask.Wait();
                var githubLoadTask = playbookData.LoadFromGithub(PlaybookConfig.GitHub, loggerFactory);
                githubLoadTask.Wait();
            }
            else
            {
                playbookData.
            }


            app.UseMiddleware<ExceptionHandler>();
            app.UseAuthentication();
            app.UseSession();
            app.UseMvc();
        }

        private static void ConfigureAuth(IServiceCollection services, IConfigurationRoot config)
        {
            var incidentAuthConfig = new AzureActiveDirectoryAuthenticationInfo("unused placeholder", "Unused placeholder", "Unused placeholder", config["AzureAd:Tenant"]);
            services.AddSingleton<AzureActiveDirectoryAuthenticationInfo>(i => incidentAuthConfig);
        }
    }
}
