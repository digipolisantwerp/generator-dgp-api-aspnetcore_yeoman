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

namespace StarterKit.Framework.Logging
{
  public static class LoggingEngineExtensions
  {
    public static void AddLoggingEngine(this IServiceCollection services)
    {
      services.AddSerilogExtensions(options =>
      {
        options.AddAuthServiceEnricher();
        options.AddCorrelationEnricher();
      });
    }

    public static void AddLoggingEngine(
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
    }

    /// <summary>
    /// overwrite logging configuration settings with the settings in the environment variables
    /// </summary>
    /// <param name="configurationBuilder"></param>
    /// <param name="hostingEnv"></param>
    /// <returns></returns>
    public static void AddLoggingConfiguration(this IConfigurationBuilder configurationBuilder,
      IWebHostEnvironment hostingEnv)
    {
      var env = Environment.GetEnvironmentVariables();

      var environmentDict = new Dictionary<string, string>();

      // if this is deployed, overwrite some settings from the environment variables
      if (hostingEnv.EnvironmentName != Environments.Development)
      {
        ConfigUtil.FillFromEnvironment($"LOG_SYSTEM_MINIMUMLEVEL_DEFAULT", "Serilog:MinimumLevel:Default",
          environmentDict);
        ConfigUtil.FillFromEnvironment($"LOG_SYSTEM_MINIMUMLEVEL_OVERRIDE_SYSTEM",
          "Serilog:MinimumLevel:Override:System", environmentDict);
        ConfigUtil.FillFromEnvironment($"LOG_SYSTEM_MINIMUMLEVEL_OVERRIDE_MICROSOFT",
          "Serilog:MinimumLevel:Override:Microsoft", environmentDict);
      }

      // load in this order so that json-settings will be overridden with environment settings when getting the configuration section
      configurationBuilder.AddJsonFile(JsonFilesKey.LoggingJson);
      configurationBuilder.AddInMemoryCollection(environmentDict);
    }
  }
}
