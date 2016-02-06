using System.IO;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using Toolbox.WebApi;

namespace StarterKit
{
    public class Startup
    {
		public Startup(IHostingEnvironment env, IApplicationEnvironment appEnv)
		{
		    _applicationBasePath = appEnv.ApplicationBasePath;
		}
		
		private readonly string _applicationBasePath;

        public void ConfigureServices(IServiceCollection services)
        {
            var configPath = Path.Combine(_applicationBasePath, "_config");
			var config = new ConfigurationConfig(configPath);
			config.Configure(services);
			
            LoggingConfig.Configure(services);
            AutoMapperConfiguration.Configure();

			services.AddMvc()
                .AddActionOverloading()
                .AddVersioning();
            
            services.AddBusinessServices();
            services.AddAutoMapper();
                
            services.AddSwaggerGen();
		}
        
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
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
