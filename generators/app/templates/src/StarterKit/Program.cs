using Digipolis.Serilog.Startup;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Settings.Configuration;
using StarterKit.DataAccess.Options;
using StarterKit.Framework.Logging;
using StarterKit.Shared.Constants;
using StarterKit.Shared.Options;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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

			// set up our preliminary "bootstrap" logger to console so logs can be picked up by an external system.
			// The initial "bootstrap" logger is able to log errors during start-up. It's completely replaced by the
			// logger configured in `UseSerilog()` below, once configuration and dependency-injection have both been set up successfully.
			Log.Logger = new LoggerConfiguration()
				.MinimumLevel.Debug()
				.MinimumLevel.Override("Microsoft", LogEventLevel.Information)
				.ReadFrom.Configuration(loggingConfig)  // by default Serilog looks for Serilog section in configuration
				.Enrich.FromLogContext()
				.Filter.ByExcluding(l => l.Properties.Any(p => p.Value.ToString().Contains("/status/")))
				.CreateBootstrapLogger();

			try
			{
				Log.Information("Application started. Starting web host...");

				await CreateWebHostBuilder(args).Build().RunAsync();
				Log.Information("Application stopped successfully.");
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

		public static IHostBuilder ConfigureWebHostBuilder(string[] args, string configPath)
		{
			var hostingConfig = new ConfigurationBuilder()
				.SetBasePath(configPath)
				.AddJsonFile(JsonFilesKey.HostingJson)
				.Build();

			var envVars = Environment.GetEnvironmentVariables();
			var serverUrls = envVars.Contains(AppSettingsConfigKey.ServerUrls)
				? envVars[AppSettingsConfigKey.ServerUrls]?.ToString()
				: hostingConfig.GetValue<string>(AppSettingsConfigKey.LocalServerUrls);

			var builder = Host.CreateDefaultBuilder(args);

			builder.ConfigureWebHostDefaults(webHostBuilder =>
				{
					webHostBuilder.UseStartup<Startup.Startup>();
					webHostBuilder.UseDefaultServiceProvider(options => options.ValidateScopes = false);
					webHostBuilder.ConfigureAppConfiguration((hostingContext, config) =>
					{
						// delete all default configuration providers
						config.Sources.Clear();

						var env = hostingContext.HostingEnvironment;
						config.SetBasePath(configPath);
						config.AddLoggingConfiguration(env);
						config.AddAppSettingsConfiguration(env);
						config.AddJsonFile(JsonFilesKey.ServiceAgentsJson);
						//--dataaccess-config--
						//--authorization-config--
						config.AddEnvironmentVariables();
					});
					webHostBuilder.CaptureStartupErrors(true);
					webHostBuilder.UseConfiguration(hostingConfig);
					webHostBuilder.UseUrls(serverUrls);
				}
			);

			builder.UseSerilog((hostingContext, serviceProvider, loggerConfiguration) =>
			{
				var logSection = hostingContext.HostingEnvironment.EnvironmentName == Environments.Development
									? ConfigurationSectionKey.SerilogDev
									: ConfigurationSectionKey.Serilog;

				var options = new ConfigurationReaderOptions { SectionName = logSection };

				loggerConfiguration
				.ReadFrom.Configuration(hostingContext.Configuration, options)
				.ReadFrom.Services(serviceProvider)
				.Enrich.WithRegisteredEnrichers(serviceProvider)
				.Filter.ByExcluding(l => l.Properties.Any(p => p.Value.ToString().Contains("/status/")));
			});

			return builder;
		}

		public static IHostBuilder CreateWebHostBuilder(string[] args)
		{
			var configPath = Path.Combine(Directory.GetCurrentDirectory(), JsonFilesKey.JsonFilesPath);
			return ConfigureWebHostBuilder(args, configPath);
		}
	}
}
