using System;
using System.Collections.Generic;
using System.Linq;
using Digipolis.Serilog;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using StarterKit.Shared.Constants;
using StarterKit.Shared.Options;
using StarterKit.Shared.Options.Logging;

namespace StarterKit.Framework.Logging
{
	public static class LoggingExtensions
	{
		/// <summary>
		/// configure logging services in Startup
		/// </summary>
		public static void AddLoggingServices(this IServiceCollection services,
			IConfiguration config,
			IHostEnvironment Environment)
		{
			// initialize Logsettings options object and overwrite from environment variables
			LogSettings.RegisterConfiguration(services, config.GetSection(ConfigurationSectionKey.LogSettings),
				Environment);

			// add serilog enrichers to services
			services.AddSerilogExtensions(options =>
			{
				options.AddAuthServiceEnricher();
				options.AddCorrelationEnricher();
			});
		}

		/// <summary>
		/// load logging configuration from json and overwrite serilog log levels from environment variables
		/// </summary>
		/// <param name="configurationBuilder"></param>
		/// <param name="hostingEnv"></param>
		/// <returns></returns>
		public static void AddLoggingConfiguration(this IConfigurationBuilder configurationBuilder,
			IHostEnvironment hostingEnv)
		{
			var environmentDict = new Dictionary<string, string>();

			// overwrite serilog log levels from the environment variables
			if (hostingEnv.EnvironmentName != Environments.Development)
			{
				ConfigUtil.FillFromEnvironment(LogSettingsEnvVariableKey.SeriLog_MinimumLevel_Default,
					"Serilog:MinimumLevel:Default", environmentDict);
				ConfigUtil.FillFromEnvironment(LogSettingsEnvVariableKey.SeriLog_MinimumLevel_Override_System,
					"Serilog:MinimumLevel:Override:System", environmentDict);
				ConfigUtil.FillFromEnvironment(LogSettingsEnvVariableKey.SeriLog_MinimumLevel_Override_Microsoft,
					"Serilog:MinimumLevel:Override:Microsoft", environmentDict);
			}

			// load in this order so that json-settings will be overridden with environment settings when getting the configuration section;
			// IOptions<ApplicationLogSettings> are set in StartUp	> services.AddLoggingServices(Configuration, Environment);
			configurationBuilder.AddJsonFile(JsonFilesKey.LoggingJson);
			configurationBuilder.AddInMemoryCollection(environmentDict);
		}
	}
}
