using Digipolis.Serilog;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Serilog.Filters;
using StarterKit.Logging;
using StarterKit.Shared.Options;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StarterKit
{
  public static class LoggingEngineExtensions
  {
    public static IServiceCollection AddLoggingEngine(this IServiceCollection services)
    {
      services.AddSingleton<IApplicationLogger, ApplicationLogger>();

      services.AddSerilogExtensions(options =>
      {
        options.AddApplicationServicesEnricher();
        options.AddAuthServiceEnricher();
        options.AddCorrelationEnricher();
        options.AddMessagEnricher(msgOptions => msgOptions.MessageVersion = "1");
      });

      return services;
    }

    public static ILoggerFactory AddLoggingEngine(this ILoggerFactory loggerFactory, IApplicationBuilder app, IApplicationLifetime appLifetime, IConfiguration config)
    {
      var enrichers = app.ApplicationServices.GetServices<ILogEventEnricher>().ToArray();

      var systemLogSection = config.GetSection("SystemLog");
      var applicationLogSection = config.GetSection("ApplicationLog");

      var appLogger = typeof(ApplicationLogger).FullName;

      Log.Logger = new LoggerConfiguration()
                      .Enrich.With(enrichers)
                      .WriteTo.Logger(l => l.ReadFrom.ConfigurationSection(systemLogSection).Filter.ByExcluding(Matching.FromSource(appLogger)))
                      .WriteTo.Logger(l => l.ReadFrom.ConfigurationSection(applicationLogSection).Filter.ByIncludingOnly(Matching.FromSource(appLogger)))
                      .CreateLogger();

      loggerFactory.AddSerilog(dispose: true);

      appLifetime.ApplicationStopped.Register(Log.CloseAndFlush);

      return loggerFactory;
    }

    /// <summary>
    /// overwrite logging configuration settings with the settings in the environment variables
    /// </summary>
    /// <param name="configurationBuilder"></param>
    /// <param name="hostingEnv"></param>
    /// <returns></returns>
    public static IConfigurationBuilder AddLoggingConfiguration(this IConfigurationBuilder configurationBuilder, IHostingEnvironment hostingEnv)
    {
      var env = Environment.GetEnvironmentVariables();

      var environmentDict = new Dictionary<string, string>();

      // if this is deployed, overwrite some settings from the environment variables
      if (!hostingEnv.IsDevelopment())
      {
        // DEFAULT LOGGING LEVELS
        ConfigUtil.FillFromEnvironment($"LOG_LEVEL_DEFAULT", "Logging:LogLevel:Default", environmentDict);
        ConfigUtil.FillFromEnvironment($"LOG_LEVEL_SYSTEM", "Logging:LogLevel:System", environmentDict);
        ConfigUtil.FillFromEnvironment($"LOG_LEVEL_MICROSOFT", "Logging:LogLevel:Microsoft", environmentDict);

        // CONSOLE
        ConfigUtil.FillFromEnvironment($"LOG_CONSOLE_LEVEL_DEFAULT", "Logging:Console:LogLevel:Default", environmentDict);
        ConfigUtil.FillFromEnvironment($"LOG_CONSOLE_LEVEL_SYSTEM", "Logging:Console:LogLevel:System", environmentDict);
        ConfigUtil.FillFromEnvironment($"LOG_CONSOLE_LEVEL_MICROSOFT", "Logging:Console:LogLevel:Microsoft", environmentDict);
        
        // APPLICATION
        ConfigUtil.FillFromEnvironment("LOG_ELASTIC_APPLICATION_BUFFERPATH", "ApplicationLog:WriteTo:0:Args:bufferBaseFilename", environmentDict);
        ConfigUtil.FillFromEnvironment("LOG_ELASTIC_APPLICATION_HEADERS", "ApplicationLog:WriteTo:0:Args:connectionGlobalHeaders", environmentDict);
        ConfigUtil.FillFromEnvironment("LOG_ELASTIC_APPLICATION_URL", "ApplicationLog:WriteTo:0:Args:nodeUris", environmentDict);

        ConfigUtil.FillFromEnvironment($"LOG_ELASTIC_APPLICATION_MINIMUMLEVEL_DEFAULT", "ApplicationLog:MinimumLevel:Default", environmentDict);
        ConfigUtil.FillFromEnvironment($"LOG_ELASTIC_APPLICATION_MINIMUMLEVEL_OVERRIDE_SYSTEM", "ApplicationLog:MinimumLevel:Override:System", environmentDict);
        ConfigUtil.FillFromEnvironment($"LOG_ELASTIC_APPLICATION_MINIMUMLEVEL_OVERRIDE_MICROSOFT", "ApplicationLog:MinimumLevel:Override:Microsoft", environmentDict);
        
        // SYSTEM
        ConfigUtil.FillFromEnvironment($"LOG_ELASTIC_SYSTEM_BUFFERPATH", "SystemLog:WriteTo:1:Args:bufferBaseFilename", environmentDict);
        ConfigUtil.FillFromEnvironment($"LOG_ELASTIC_SYSTEM_HEADERS", "SystemLog:WriteTo:1:Args:connectionGlobalHeaders", environmentDict);
        ConfigUtil.FillFromEnvironment($"LOG_ELASTIC_SYSTEM_URL", "SystemLog:WriteTo:1:Args:nodeUris", environmentDict);

        ConfigUtil.FillFromEnvironment($"LOG_ELASTIC_SYSTEM_MINIMUMLEVEL_DEFAULT", "SystemLog:MinimumLevel:Default", environmentDict);
        ConfigUtil.FillFromEnvironment($"LOG_ELASTIC_SYSTEM_MINIMUMLEVEL_OVERRIDE_SYSTEM", "SystemLog:MinimumLevel:Override:System", environmentDict);
        ConfigUtil.FillFromEnvironment($"LOG_ELASTIC_SYSTEM_MINIMUMLEVEL_OVERRIDE_MICROSOFT", "SystemLog:MinimumLevel:Override:Microsoft", environmentDict);
      }
      
      // load in this order so that json-settings will be overridden with environment settings when getting the configuration section
      configurationBuilder.AddJsonFile("logging.json");
      configurationBuilder.AddInMemoryCollection(environmentDict);
      return configurationBuilder;
    }    

  }
}

// example of application configuration for environment variables:
//"log": {
//	"level": {
//		"default": "Debug",
//		"system": "Information",
//		"microsoft": "Information"
//	},
//	"console": {
//		"level": {
//			"default": "Debug",
//			"system": "Information",
//			"microsoft": "Information"
//		}
//	},
//	"elastic": {
//		"system": {
//			"minimumlevel": {
//				"override": {
//					"system": "Information",
//					"microsoft": "Information"
//				},
//				"default": "Debug"
//			},
//			"url": "https://logging-app1-o.antwerpen.be",
//			"globalheaders": "Authorization=Basic YXBwbGljYXRpb24tYXV0aHphcGk6cWEyT096blU=",
//			"bufferpath": "elk-system-authz"
//		},
//		"application": {
//			"minimumlevel": {
//				"override": {
//					"system": "Information",
//					"microsoft": "Information"
//				},
//				"default": "Debug"
//			},
//			"url": "https://logging-app1-o.antwerpen.be",
//			"globalheaders": "Authorization=Basic YXBwbGljYXRpb24tYXV0aHphcGk6cWEyT096blU=",
//			"bufferpath": "elk-application-authz"
//		}
//	}
//}
