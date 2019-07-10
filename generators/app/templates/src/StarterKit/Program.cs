using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace StarterKit
{
  public class Program
  {
    public static void Main(string[] args)
    {
      CreateWebHostBuilder(args).Build().Run();
    }

    public static IWebHostBuilder CreateWebHostBuilder(string[] args)
    {
      var configuration = new ConfigurationBuilder()
          .SetBasePath(Directory.GetCurrentDirectory())
          .AddJsonFile("_config/hosting.json")
          .Build();

      var envVars = Environment.GetEnvironmentVariables();
      var serverUrls = envVars.Contains($"SERVER_URLS") ? envVars[$"SERVER_URLS"].ToString() : configuration.GetValue<string>("server.urls");


      return WebHost.CreateDefaultBuilder(args)
          .UseStartup<Startup>()
          .UseDefaultServiceProvider(options => options.ValidateScopes = false)
          .ConfigureAppConfiguration((hostingContext, config) =>
          {
            // delete all default configuration providers
            config.Sources.Clear();

            var env = hostingContext.HostingEnvironment;
            var configPath = Path.Combine(env.ContentRootPath, "_config");

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
          .UseConfiguration(configuration)
          .UseUrls(serverUrls);
    }
  }
}
