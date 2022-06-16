using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StarterKit.Shared.Constants;

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

		public static void RegisterConfiguration(IServiceCollection services, IConfigurationSection section,
			IHostEnvironment environment)
		{
			services.Configure<AppSettings>(settings =>
			{
				settings.LoadFromConfigSection(section);
				settings.OverrideFromEnvironmentVariables(environment);
			});
		}

		private void LoadFromConfigSection(IConfiguration section)
		{
			section.Bind(this);
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
}