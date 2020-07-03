using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using AutoMapper;
using Digipolis.ApplicationServices;
using Digipolis.Correlation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using StarterKit.Framework.Logging;
using StarterKit.Shared.Extensions;
using StarterKit.Shared.Options;
using StarterKit.Shared.Swagger;

namespace StarterKit.Startup
{
  public class Startup
  {
    public IConfiguration Configuration { get; }
    public string ApplicationBasePath { get; }
    private string XmlCommentsPath
    {
      get
      {
        var fileName = typeof(Startup).GetTypeInfo().Assembly.GetName().Name + ".xml";
        return Path.Combine(ApplicationBasePath, fileName);
      }
    }

    public Startup(IConfiguration configuration, IHostEnvironment env)
    {
      ApplicationBasePath = env.ContentRootPath;
      Configuration = configuration;
    }


    public void ConfigureServices(IServiceCollection services)
    {
      // Check out ExampleController to find out how these configs are injected into other classes
      var appSettingsSection = Configuration.GetSection("AppSettings");
      var appSettings = appSettingsSection.Get<AppSettings>();
      services.AddSingleton(Configuration).Configure<AppSettings>(appSettingsSection);
      
      services.AddApplicationServices(opt =>
      {
        opt.ApplicationId = appSettings.ApplicationId;
        opt.ApplicationName = appSettings.AppName;
      });

      services.AddCorrelation(options => { options.CorrelationHeaderRequired = true; });

      services.AddLoggingEngine();

      //--dataaccess-startupServices--

      services.AddControllers()
          .AddNewtonsoftJson(options =>
          {
            options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
            options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            options.SerializerSettings.MetadataPropertyHandling = MetadataPropertyHandling.Ignore;
            options.SerializerSettings.Converters.Add(new StringEnumConverter());
          });

      services
        .AddApiVersioning(options =>
        {
          options.ReportApiVersions = true;
          options.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
          options.AssumeDefaultVersionWhenUnspecified = true;
          options.UseApiBehavior = false;
        })
        .AddVersionedApiExplorer(options =>
        {
          options.GroupNameFormat = "'v'VVV";
          options.SubstituteApiVersionInUrl = true;
        });

      services.AddBusinessServices();
      services.AddServiceAgentServices();
      services.AddDataAccessServices();

      services.AddAutoMapper(typeof(Startup).Assembly);
      //services.AddAuthentication(IISDefaults.AuthenticationScheme);

      services
        .AddSwaggerGen(options =>
        {

          // Define multiple swagger docs if you have multiple api versions
          options.SwaggerDoc("v1",
            new OpenApiInfo
            {
              Title = "STARTERKIT API",
              Version = "v1",
              Description = "<API DESCRIPTION>",
              TermsOfService = null,
              Contact = new OpenApiContact()
              {
                Email = "<MAIL>",
                Name = "<NAME>"
              }
            });

          options.OperationFilter<RemoveVersionFromParameter>();
          options.DocumentFilter<ReplaceVersionWithExactValueInPath>();
          options.OperationFilter<AddCorrelationHeaderRequired>();
          options.OperationFilter<AddAuthorizationHeaderRequired>();
          options.OperationFilter<RemoveSyncRootParameter>();
          options.OperationFilter<LowerCaseQueryAndBodyParameterFilter>();

          if (File.Exists(XmlCommentsPath))
          {
            options.IncludeXmlComments(XmlCommentsPath);
          }
        });
        services.AddGlobalErrorHandling<ApiExceptionMapper>();
    }

    public void Configure(IApplicationBuilder app,
                          IApiVersionDescriptionProvider versionProvider,
                          ILoggerFactory loggerFactory, IHostApplicationLifetime appLifetime,
                          IApplicationLogger appLogger)
    {
      loggerFactory.AddLoggingEngine(app, appLifetime, Configuration);

      // Enable Serilog selflogging to console.
      Serilog.Debugging.SelfLog.Enable(Console.Out);

      // log application lifetime events
      var appName = app.ApplicationServices.GetService<IOptions<AppSettings>>().Value.AppName;
      appLifetime.ApplicationStarted.Register(() => appLogger.LogInformation($"Application {appName} Started"));
      appLifetime.ApplicationStopped.Register(() => appLogger.LogInformation($"Application {appName} Stopped"));
      appLifetime.ApplicationStopping.Register(() => appLogger.LogInformation($"Application {appName} Stopping"));

      app.UseApiExtensions();

      // CORS
      app.UseCors((policy) =>
      {
        policy.AllowAnyHeader();
        policy.AllowAnyMethod();
        policy.AllowAnyOrigin();
      });

      var rewriteOptions = new RewriteOptions();
      rewriteOptions.AddRedirect("^$", "swagger");
      app.UseRewriter(rewriteOptions);

      
      app.UseAuthentication();
      app.UseRouting();
      
      app.UseAuthorization();
      app.UseEndpoints(endpoints =>
        endpoints.MapDefaultControllerRoute().RequireAuthorization("IsAuthenticated"));

      app.UseSwagger(options =>
      {
        options.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
        {
          swaggerDoc.Servers = new List<OpenApiServer>() { new OpenApiServer() { Url = $"{httpReq.Scheme}://{httpReq.Host.Value}" } };
        });
      });

      app.UseSwaggerUI(options =>
      {
        foreach (var description in versionProvider.ApiVersionDescriptions)
          options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
        options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
      });
    }

    //--dataaccess-connString--
  }
}
