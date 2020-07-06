using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StarterKit.Shared.Constants;

namespace StarterKit.Shared.Options
{
  public class AppSettings
  {
    public string AppName { get; set; }
    public string ApplicationId { get; set; }
    public string DataDirectory { get; set; }
    public string TempDirectory { get; set; }
    public bool LogExceptions { get; set; }
    public bool DisableGlobalErrorHandling { get; set; }

    public static void RegisterConfiguration(IServiceCollection services, IConfigurationSection section)
    {
      services.Configure<AppSettings>(settings =>
      {
        settings.LoadFromConfigSection(section);
        settings.OverrideFromEnvironmentVariables();
      });
    }

    private void LoadFromConfigSection(IConfiguration section)
    {
      section.Bind(this);
    }

    private void OverrideFromEnvironmentVariables()
    {
      var env = Environment.GetEnvironmentVariables();
      AppName = env.Contains(AppSettingsConfigKey.AppName) ? env[AppSettingsConfigKey.AppName]?.ToString() : AppName;
      ApplicationId = env.Contains(AppSettingsConfigKey.ApplicationId) ? env[AppSettingsConfigKey.ApplicationId]?.ToString() : ApplicationId;
      DataDirectory = env.Contains(AppSettingsConfigKey.DataDirectory) ? env[AppSettingsConfigKey.DataDirectory]?.ToString() : DataDirectory;
      TempDirectory = env.Contains(AppSettingsConfigKey.TempDirectory) ? env[AppSettingsConfigKey.TempDirectory]?.ToString() : TempDirectory;
      LogExceptions = env.Contains(AppSettingsConfigKey.LogExceptions) ? bool.Parse(env[AppSettingsConfigKey.LogExceptions]?.ToString() ?? "true") : LogExceptions;
      DisableGlobalErrorHandling = env.Contains(AppSettingsConfigKey.DisableGlobalErrorHandling) ? bool.Parse(env[AppSettingsConfigKey.DisableGlobalErrorHandling]?.ToString() ?? "false") : LogExceptions;
    }
  }
}
