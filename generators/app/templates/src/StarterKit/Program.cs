using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using StarterKit.Framework.Logging;
using StarterKit.Shared.Constants;

namespace StarterKit
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			// Get a configuration up and running
			var configPath = Path.Combine(Directory.GetCurrentDirectory(), JsonFilesKey.JsonFilesPath);
			var loggingConfig = new ConfigurationBuilder().SetBasePath(configPath).AddJsonFile(JsonFilesKey.LoggingJson)
				.Build();

			// set up our preliminary logger to console so logs can be picked up by an external system. Fe: FileBeat
			Log.Logger = new LoggerConfiguration()
				.MinimumLevel.Debug()
				.MinimumLevel.Override("Microsoft", LogEventLevel.Information)
				.ReadFrom.Configuration(loggingConfig, ConfigurationSectionKey.Serilog)
				.Enrich.FromLogContext()
				.CreateLogger();

			try
			{
				Log.Information("Application started.");
				Log.Information("Starting web host...");

				await CreateWebHostBuilder(args).Build().RunAsync();
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
				.AddJsonFile(JsonFilesKey.HostingJson)
				.Build();

			var envVars = Environment.GetEnvironmentVariables();
			var serverUrls = envVars.Contains(AppSettingsConfigKey.ServerUrls)
				? envVars[AppSettingsConfigKey.ServerUrls]?.ToString()
				: hostingConfig.GetValue<string>(AppSettingsConfigKey.LocalServerUrls);

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
					config.AddJsonFile(JsonFilesKey.AppJson);
					config.AddJsonFile(JsonFilesKey.ServiceAgentsJson);
					//--dataaccess-config--
					//--authorization-config--
					config.AddEnvironmentVariables();
				})
				.CaptureStartupErrors(true)
				.UseSerilog()
				.UseConfiguration(hostingConfig)
				.UseUrls(serverUrls);
		}

		public static IWebHostBuilder CreateWebHostBuilder(string[] args)
		{
			var configPath = Path.Combine(Directory.GetCurrentDirectory(), JsonFilesKey.JsonFilesPath);
			return ConfigureWebHostBuilder(args, configPath);
		}
	}
}