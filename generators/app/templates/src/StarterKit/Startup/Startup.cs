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
using Swashbuckle.AspNetCore.Swagger;
using AutoMapper;

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

      services.AddApplicationServices(opt =>
      {
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
          });

      services.AddBusinessServices();
      services.AddAutoMapper();

      services.AddSwaggerGen<ApiExtensionSwaggerSettings>((options) => { });

      services.ConfigureSwaggerGen(options =>
      {
        options.SwaggerDoc("v1",
            new Info
            {
              Title = "Geo Search API",
              Version = "v1",
              Description = "A simple api to search using geo location in Elasticsearch",
              TermsOfService = "None"
            }
         );

        //var filePath = Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, pathToDoc);
        //options.IncludeXmlComments(filePath);
        options.DescribeAllEnumsAsStrings();
      });

      services.AddGlobalErrorHandling<ApiExceptionMapper>();
    }

    public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IApplicationLifetime appLifetime)
    {
      loggerFactory.AddConsole(Configuration.GetSection("ConsoleLogging"));
      loggerFactory.AddDebug(LogLevel.Debug);
      loggerFactory.AddLoggingEngine(app, appLifetime, Configuration);

      // CORS
      app.UseCors((policy) =>
      {
        policy.AllowAnyHeader();
        policy.AllowAnyMethod();
        policy.AllowAnyOrigin();
        policy.AllowCredentials();
      });

      app.UseApiExtensions();

      app.UseMvc(routes => { });

      app.UseSwagger(c =>
      {
        c.PreSerializeFilters.Add((swagger, httpReq) => swagger.Host = httpReq.Host.Value);
      });

      app.UseSwaggerUI(options  =>
      {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "V1 Docs");
      });


      app.UseSwaggerUiRedirect();
    }

    //--dataaccess-connString--
  }
}
