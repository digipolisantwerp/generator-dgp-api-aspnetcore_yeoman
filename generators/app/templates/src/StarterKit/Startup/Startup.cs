using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
//--dataaccess-startupImports--
using Digipolis.Web;
using Digipolis.Web.Startup;
using StarterKit.Options;
using Digipolis.ApplicationServices;
using Digipolis.Correlation;

namespace StarterKit
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var appEnv = PlatformServices.Default.Application;
            ApplicationBasePath = appEnv.ApplicationBasePath;
            ConfigPath = Path.Combine(env.ContentRootPath, "_config");

            var builder = new ConfigurationBuilder()
                .SetBasePath(ConfigPath)
                .AddJsonFile("logging.json")
                .AddJsonFile("app.json")
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; private set; }
        public string ApplicationBasePath { get; private set; }
        public string ConfigPath { get; private set; }

        public void ConfigureServices(IServiceCollection services)
        {
            // Check out ExampleController to find out how these configs are injected into other classes
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));

            services.AddApplicationServices(opt => {
                opt.ApplicationId = "enter-your-application-id-here";
                opt.ApplicationName = "StarterKit";
            });

            services.AddCorrelation();

            services.AddLoggingEngine();

            //--dataaccess-startupServices--

            services.AddMvc()
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                })
                .AddApiExtensions(null, options =>
                {
                    options.DisableVersioning = true;
                    options.DisableGlobalErrorHandling = true;
                })
                .AddVersionEndpoint();

            services.AddBusinessServices();
            services.AddAutoMapper();
                
            services.AddSwaggerGen();

            services.AddGlobalErrorHandling<ApiExceptionMapper>();
		}
        
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IApplicationLifetime appLifetime)
        {
            loggerFactory.AddConsole(Configuration.GetSection("ConsoleLogging"));
            loggerFactory.AddDebug(LogLevel.Debug);
            loggerFactory.AddLoggingEngine(app, appLifetime, Configuration);
                       
			// CORS
            app.UseCors((policy) => {
                policy.AllowAnyHeader();
                policy.AllowAnyMethod();
                policy.AllowAnyOrigin();
                policy.AllowCredentials();
            });
            
            app.UseApiExtensions();

			app.UseMvc(routes =>
			{
				routes.MapRoute(
					name: "default",
					template: "api/{controller}/{id?}");
			});

            app.UseSwagger();
            app.UseSwaggerUi();
            app.UseSwaggerUiRedirect();
        }

        //--dataaccess-connString--
    }
}
