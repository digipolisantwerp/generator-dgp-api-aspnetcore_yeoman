using Microsoft.Extensions.Hosting;
using System;

namespace StarterKit.Shared
{
  public abstract class SettingsBase
  {
    public static string GetValue(string value, string configKey, IHostEnvironment env)
    {
      var configKeyValue = System.Environment.GetEnvironmentVariable(configKey);
      return configKeyValue ?? ((env.IsDevelopment()) ? value : throw new ArgumentException($"Configuration error: invalid parameter '{configKey}'"));
    }

    public static int GetValue(int value, string configKey, IHostEnvironment env)
    {
      return int.TryParse(System.Environment.GetEnvironmentVariable(configKey), out int configKeyValue) ?
        configKeyValue : ((env.IsDevelopment()) ? value : throw new ArgumentException($"Configuration error: invalid parameter '{configKey}'"));
    }

    public static bool GetValue(bool value, string configKey, IHostEnvironment env)
    {
      return bool.TryParse(System.Environment.GetEnvironmentVariable(configKey), out bool configKeyValue) ?
        configKeyValue : ((env.IsDevelopment()) ? value : throw new ArgumentException($"Configuration error: invalid parameter '{configKey}'"));
    }

    public static bool IsValidUri(string source, bool allowHttp = false) => Uri.TryCreate(source, UriKind.Absolute, out Uri uriResult) && (allowHttp || uriResult.Scheme == Uri.UriSchemeHttps);
  }
}
