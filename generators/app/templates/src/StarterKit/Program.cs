using System;
using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using StarterKit.Startup;

namespace StarterKit
{
  public class Program
  {
    public static void Main(string[] args)
    {
      // Get a configuration up and running
      var configPath = Path.Combine(Directory.GetCurrentDirectory(), "_config");
      var loggingConfig = new ConfigurationBuilder().SetBasePath(configPath).AddJsonFile("logging.json").Build();

      // Set up our preliminary logger
      Log.Logger = new LoggerConfiguration()
        .ReadFrom.Configuration(loggingConfig, "Logging")
        .Enrich.FromLogContext()
        .CreateLogger();

      try
      {
        Log.Information("Application started.");

        Log.Information("Starting web host...");
        CreateWebHostBuilder(args).Build().Run();
      }
      catch (Exception e)
      {
        Log.Fatal(e, "Host terminated unexpectedly.");
        throw;
      }
      finally
      {
        Log.CloseAndFlush();
      }
    }

    public static IWebHostBuilder ConfigureWebHostBuilder(string[] args, string configPath)
    {

      var hostingConfig = new ConfigurationBuilder()
        .SetBasePath(configPath)
        .AddJsonFile("hosting.json")
        .Build();

      var envVars = Environment.GetEnvironmentVariables();
      var serverUrls = envVars.Contains($"SERVER_URLS") ? envVars[$"SERVER_URLS"].ToString() : hostingConfig.GetValue<string>("server.urls");

      return WebHost.CreateDefaultBuilder(args)
          .UseStartup<Startup.Startup>()
          .UseDefaultServiceProvider(options => options.ValidateScopes = false)
          .ConfigureAppConfiguration((hostingContext, config) =>
          {
            // delete all default configuration providers
            config.Sources.Clear();

            var env = hostingContext.HostingEnvironment;
            config.SetBasePath(configPath);
            config.AddLoggingConfiguration(env);
            config.AddJsonFile("app.json");
            //--dataaccess-config--
            config.AddEnvironmentVariables();
          })
          .ConfigureLogging((hostingContext, logging) =>
          {
            logging.ClearProviders();
            logging.SetMinimumLevel(LogLevel.Debug);

            logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
            logging.AddConsole();
            logging.AddDebug();
          })
          .UseConfiguration(hostingConfig)
          .UseUrls(serverUrls);
    }

    public static IWebHostBuilder CreateWebHostBuilder(string[] args)
    {
      var configPath = Path.Combine(Directory.GetCurrentDirectory(), "_config");
      return ConfigureWebHostBuilder(args, configPath);
    }
  }
}
