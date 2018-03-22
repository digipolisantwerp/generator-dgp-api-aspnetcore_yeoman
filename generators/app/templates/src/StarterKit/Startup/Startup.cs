using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
using System.Reflection;
using StarterKit.Shared.Swagger;
using Microsoft.Extensions.Options;
using StarterKit.Api.Mapping;

namespace StarterKit
{
  public class Startup
  {
    public Startup(IConfiguration configuration, IHostingEnvironment env)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; private set; }
    public string ApplicationBasePath { get; private set; }

    public void ConfigureServices(IServiceCollection services)
    {
      // Check out ExampleController to find out how these configs are injected into other classes
      AppSettings.RegisterConfiguration(services, Configuration.GetSection("AppSettings"));

      var appSettings = services.BuildServiceProvider().GetService<IOptions<AppSettings>>().Value;

      services.AddApplicationServices(opt =>
      {
        opt.ApplicationId = appSettings.ApplicationId;
        opt.ApplicationName = appSettings.AppName;
      });

      services.AddCorrelation(options => { options.CorrelationHeaderRequired = true; });

      services.AddLoggingEngine();

      //--dataaccess-startupServices--

      services.AddMvc()
          .AddJsonOptions(options =>
          {
            options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            options.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
          })
          .AddApiExtensions(null, options =>
          {
            options.DisableVersioning = true;
          });

      services.AddBusinessServices();
      services.AddServiceAgentServices();
      services.AddDataAccessServices();
      services.AddAutoMapper((config) => {
        config.AddProfile<StatusProfile>();
      });

      services.AddSwaggerGen<ApiExtensionSwaggerSettings>((options) => {
        
      });

      services.ConfigureSwaggerGen(options =>
      {
        options.SwaggerDoc("v1",
            new Info
            {
              Title = "STARTERKIT API",
              Version = "v1",
              Description = "<API DESCRIPTION>",
              TermsOfService = "None",
              Contact = new Contact()
              {
                Email = "<MAIL>",
                Name = "<NAME>"
              }
            }
         );

        options.OperationFilter<AddCorrelationHeaderRequired>();
        options.OperationFilter<AddAuthorizationHeaderRequired>();
        options.OperationFilter<RemoveSyncRootParameter>();
        options.OperationFilter<LowerCaseQueryAndBodyParameterFilter>();

        var location = Assembly.GetEntryAssembly().Location;

        string xmlComments = Path.Combine(Path.GetDirectoryName(location), Path.GetFileNameWithoutExtension(location) + ".xml");

        if (File.Exists(xmlComments))
        {
          options.IncludeXmlComments(xmlComments);
        }

        options.DescribeAllEnumsAsStrings();
      });

      services.AddGlobalErrorHandling<ApiExceptionMapper>();
    }

    public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IApplicationLifetime appLifetime)
    {
      loggerFactory.AddConsole(Configuration.GetSection("ConsoleLogging"));
      loggerFactory.AddDebug(LogLevel.Debug);
      loggerFactory.AddLoggingEngine(app, appLifetime, Configuration);

      // Enable Serilog selflogging to console.
      Serilog.Debugging.SelfLog.Enable(Console.Out);

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

      app.UseSwaggerUI(options =>
      {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "V1 Docs");
      });


      app.UseSwaggerUiRedirect();
    }

    //--dataaccess-connString--
  }
}
