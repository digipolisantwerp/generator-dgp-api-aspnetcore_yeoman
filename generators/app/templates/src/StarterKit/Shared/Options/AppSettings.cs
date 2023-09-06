using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StarterKit.Shared.Constants;
using System;
using System.Collections.Generic;

namespace StarterKit.Shared.Options
{
	public class AppSettings : SettingsBase
	{
		public string AppName { get; set; }
		public string ApplicationId { get; set; }
		public string DataDirectory { get; set; }
		public string TempDirectory { get; set; }

		// uri to overview of possible error types returned by this API: see errors.md at solution level
		public string ErrorReferenceUri { get; set; }
		public bool DisableGlobalErrorHandling { get; set; }

		/// <summary>
		/// duration an object is cached before it expires
		/// </summary>
		public int CacheExpiration { get; set; }

		public TimeSpan GetCacheExpirationAsTimeSpan
		{
			get
			{
				if (CacheExpiration <= 0) CacheExpiration = 30;
				return new TimeSpan(hours: 0, minutes: CacheExpiration, seconds: 0);
			}
		}

		public static IConfigurationSection GetConfigurationSection(IConfiguration Configuration)
		{
			return Configuration.GetSection(ConfigurationSectionKey.AppSettings);
		}

		public static void RegisterConfiguration(IServiceCollection services, IConfiguration configuration)
		{
			// appSettings from json are already overridden by environment variables in Program.cs
			services.Configure<AppSettings>(GetConfigurationSection(configuration));
		}


		private void OverrideFromEnvironmentVariables(IHostEnvironment environment)
		{
			AppName = GetValue(AppName, AppSettingsConfigKey.AppName, environment);
			ApplicationId = GetValue(ApplicationId, AppSettingsConfigKey.ApplicationId, environment);
			DataDirectory = GetValue(DataDirectory, AppSettingsConfigKey.DataDirectory, environment);
			TempDirectory = GetValue(TempDirectory, AppSettingsConfigKey.TempDirectory, environment);
			ErrorReferenceUri = GetValue(ErrorReferenceUri, AppSettingsConfigKey.ErrorReferenceUri, environment);
			DisableGlobalErrorHandling = GetValue(DisableGlobalErrorHandling,
				AppSettingsConfigKey.DisableGlobalErrorHandling, environment);
			CacheExpiration = GetValue(CacheExpiration, AppSettingsConfigKey.CacheExpiration, environment);

			if (!string.IsNullOrWhiteSpace(ErrorReferenceUri))
			{
				if (!IsValidUri(ErrorReferenceUri, true))
					throw new System.Exception(
						$"Invalid configuration value {ErrorReferenceUri} for appsetting ErrorReferenceUri.");
			}
		}
	}
	public static class AppSettingsExtensions
	{
		/// <summary>
		/// load appsettings from json and overwrite necessary params from environment variables
		/// </summary>
		/// <param name="configurationBuilder"></param>
		/// <param name="hostingEnv"></param>
		/// <returns></returns>
		public static IConfigurationBuilder AddAppSettingsConfiguration(this IConfigurationBuilder configurationBuilder,
		IHostEnvironment hostingEnv)
		{
			// load in this order so that json-settings will be overridden with environment settings when getting the configuration section;
			configurationBuilder.AddJsonFile(JsonFilesKey.AppJson);
			configurationBuilder.AddInMemoryCollection(GetEnvironmentVariablesDict(hostingEnv));

			return configurationBuilder;
		}

		private static Dictionary<string, string> GetEnvironmentVariablesDict(IHostEnvironment hostingEnv)
		{
			var environmentDict = new Dictionary<string, string>();

			// overwrite settings from environment variables;
			// if overwriting the json-file value from environment variables isn't necesarry, omit the variable declaration in appconfig and beneath
			if (hostingEnv.EnvironmentName != RuntimeEnvironment.Local)
			{
				ConfigUtil.FillFromEnvironment(AppSettingsConfigKey.ApplicationId, "AppSettings:ApplicationId", environmentDict);
				ConfigUtil.FillFromEnvironment(AppSettingsConfigKey.AppName, "AppSettings:AppName", environmentDict);
				ConfigUtil.FillFromEnvironment(AppSettingsConfigKey.DataDirectory, "AppSettings:DataDirectory", environmentDict);
				ConfigUtil.FillFromEnvironment(AppSettingsConfigKey.TempDirectory, "AppSettings:TempDirectory", environmentDict);

				ConfigUtil.FillFromEnvironment(AppSettingsConfigKey.DisableGlobalErrorHandling, "AppSettings:DisableGlobalErrorHandling", environmentDict);
				ConfigUtil.FillFromEnvironment(AppSettingsConfigKey.CacheExpiration, "AppSettings:CacheExpiration", environmentDict);

				var errorReferenceUri = ConfigUtil.GetEnvironmentVariable(AppSettingsConfigKey.ErrorReferenceUri, false);
				if (!string.IsNullOrWhiteSpace(errorReferenceUri))
				{
					if (!IsValidUri(errorReferenceUri, true))
						throw new System.Exception(
							$"Invalid configuration value {errorReferenceUri} for appsetting ErrorReferenceUri.");
				}
				environmentDict.Add("AppSettings:ErrorReferenceUri", errorReferenceUri);
			}

			return environmentDict;
		}

		public static bool IsValidUri(string source, bool allowHttp = false) =>
			Uri.TryCreate(source, UriKind.Absolute, out var uriResult) &&
			(allowHttp || uriResult.Scheme == Uri.UriSchemeHttps);
	}
}
