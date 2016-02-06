using System.IO;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using Toolbox.WebApi;
using SeriLog;
using SeriLog.Sinks.RollingFile;

namespace StarterKit
{
    public class Startup
    {
		public Startup(IHostingEnvironment env, IApplicationEnvironment appEnv)
		{
            var configPath = Path.Combine(appEnv.ApplicationBasePath, "_config");
            var builder = new ConfigurationBuilder()
                .SetBasePath(configPath)
                .AddJsonFile("logging.json")
                .AddJsonFile("app.json")
                .AddEnvironmentVariables();
            Configuration = builder.Build();
            
            ApplicationBasePath = appEnv.ApplicationBasePath;     
            
            Log.Logger = new LoggerConfiguration().WriteTo.RollingFile("pathToLogFile").CreateLogger();       
		}
		
        public IConfigurationRoot Configuration { get; private set; }
        public string ApplicationBasePath { get; private set; }
        
        public void ConfigureServices(IServiceCollection services)
        {
            var configPath = Path.Combine(ApplicationBasePath, "_config");
			var config = new ConfigurationConfig(configPath);
			config.Configure(services);
			
            LoggingConfig.Configure(services);

			services.AddMvc()
                .AddActionOverloading()
                .AddVersioning();
            
            services.AddBusinessServices();
            services.AddAutoMapper();
                
            services.AddSwaggerGen();
		}
        
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
		{
            loggerFactory.MinimumLevel = LogLevel.Debug;    // ToDo: to config file
            loggerFactory.AddConsole(Configuration.GetSection("ConsoleLogging"));
            loggerFactory.AddDebug(LogLevel.Debug);
            loggerFactory.AddSeriLog();
            
			// CORS
            app.UseCors((policy) => {
                policy.AllowAnyHeader();
                policy.AllowAnyMethod();
                policy.AllowAnyOrigin();
                policy.AllowCredentials();
            });

            app.UseIISPlatformHandler();

			app.UseMvc(routes =>
			{
				routes.MapRoute(
					name: "default",
					template: "api/{controller}/{id?}");
			});
            
            app.UseSwaggerGen();
            app.UseSwaggerUi();
            app.UseSwaggerUiRedirect();
		}
        
        public static void Main(string[] args) => WebApplication.Run<Startup>(args);
    }
}
