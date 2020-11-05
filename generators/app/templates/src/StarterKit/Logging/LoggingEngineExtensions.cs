using System;
using System.Collections.Generic;
using System.Linq;
using Digipolis.Serilog;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using StarterKit.Shared.Constants;
using StarterKit.Shared.Options;

namespace StarterKit.Logging
{
  public static class LoggingEngineExtensions
  {
    public static IServiceCollection AddLoggingEngine(this IServiceCollection services)
    {


      services.AddSerilogExtensions(options =>
      {
        options.AddApplicationServicesEnricher();
        options.AddAuthServiceEnricher();
        options.AddCorrelationEnricher();
        options.AddMessagEnricher(msgOptions => msgOptions.MessageVersion = "1");
      });

      return services;
    }

    public static ILoggerFactory AddLoggingEngine(
      this ILoggerFactory loggerFactory,
      IApplicationBuilder app,
      IHostApplicationLifetime appLifetime,
      IConfiguration config,
      IHostEnvironment hostingEnv)
    {
      var enrich = app.ApplicationServices.GetServices<ILogEventEnricher>().ToArray();

      var logSection = hostingEnv.EnvironmentName == Environments.Development
        ? ConfigurationSectionKey.SerilogDev
        : ConfigurationSectionKey.Serilog;

      Log.Logger = new LoggerConfiguration()
        .Enrich.With(enrich)
        .Enrich.With(new TypeEnricher())
        .ReadFrom.Configuration(config, logSection)
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
    public static IConfigurationBuilder AddLoggingConfiguration(this IConfigurationBuilder configurationBuilder, IWebHostEnvironment hostingEnv)
    {
      var env = Environment.GetEnvironmentVariables();

      var environmentDict = new Dictionary<string, string>();

      // if this is deployed, overwrite some settings from the environment variables
      if (hostingEnv.EnvironmentName != Environments.Development)
      {
        ConfigUtil.FillFromEnvironment($"LOG_SYSTEM_MINIMUMLEVEL_DEFAULT", "Serilog:MinimumLevel:Default", environmentDict);
        ConfigUtil.FillFromEnvironment($"LOG_SYSTEM_MINIMUMLEVEL_OVERRIDE_SYSTEM", "Serilog:MinimumLevel:Override:System", environmentDict);
        ConfigUtil.FillFromEnvironment($"LOG_SYSTEM_MINIMUMLEVEL_OVERRIDE_MICROSOFT", "Serilog:MinimumLevel:Override:Microsoft", environmentDict);
      }

      // load in this order so that json-settings will be overridden with environment settings when getting the configuration section
      configurationBuilder.AddJsonFile(JsonFilesKey.LoggingJson);
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
