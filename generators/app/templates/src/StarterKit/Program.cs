using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace StarterKit
{
  public class Program
  {
    public static void Main(string[] args)
    {
      BuildWebHost(args).Run();
    }

    public static IWebHost BuildWebHost(string[] args)
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
            config.AddLoggingConfiguration();
            config.AddJsonFile("app.json");
            //--dataaccess-config--
            config.AddEnvironmentVariables();
          })          
          .UseConfiguration(configuration)
          .UseUrls(serverUrls)
          .Build();
    }
  }
}
