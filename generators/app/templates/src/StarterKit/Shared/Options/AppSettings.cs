using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StarterKit.Shared.Constants;
using System.Collections.Generic;

namespace StarterKit.Shared.Options
{
  public class AppSettings : SettingsBase
  {
    public string AppName { get; set; }
    public string ApplicationId { get; set; }
    public string DataDirectory { get; set; }
    public string TempDirectory { get; set; }
    public bool DisableGlobalErrorHandling { get; set; }


    public static void RegisterConfiguration(IServiceCollection services, IConfigurationSection section, IHostEnvironment environment)
    {
      services.Configure<AppSettings>(settings =>
      {
        settings.LoadFromConfigSection(section);
        settings.OverrideFromEnvironmentVariables(environment);
      });
    }

    private IEnumerable<string> GetStringsFromConfigSection(IConfiguration section, string subSectionKey, IEnumerable<string> defaultIfMissing)
    {
      var result = section.GetSection(subSectionKey).Get<IEnumerable<string>>();

      return result != null ? result : defaultIfMissing;
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
      DisableGlobalErrorHandling = GetValue(DisableGlobalErrorHandling, AppSettingsConfigKey.DisableGlobalErrorHandling, environment);
    }
  }
}
