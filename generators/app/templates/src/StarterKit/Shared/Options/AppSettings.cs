using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StarterKit.Shared.Constants;

namespace StarterKit.Shared.Options
{
  public class AppSettings: SettingsBase
  {
    public string AppName { get; set; }
    public string ApplicationId { get; set; }
    public string DataDirectory { get; set; }
    public string TempDirectory { get; set; }
    public bool LogExceptions { get; set; }
    public bool DisableGlobalErrorHandling { get; set; }

    public RequestLogging RequestLogging { get; set; }


    public static void RegisterConfiguration(IServiceCollection services, IConfigurationSection section, IHostEnvironment environment)
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
      const string APPSETTINGS = "APPSETTINGS";

      AppName = GetValue(AppName, AppSettingsConfigKey.AppName, environment);
      ApplicationId = GetValue(ApplicationId, AppSettingsConfigKey.ApplicationId, environment);
      DataDirectory = GetValue(DataDirectory, AppSettingsConfigKey.DataDirectory, environment);
      TempDirectory = GetValue(TempDirectory, AppSettingsConfigKey.TempDirectory, environment);
      LogExceptions = GetValue(LogExceptions, AppSettingsConfigKey.LogExceptions, environment);
      DisableGlobalErrorHandling = GetValue(DisableGlobalErrorHandling, AppSettingsConfigKey.DisableGlobalErrorHandling, environment);

      bool parseResultSuccess;

      if(bool.TryParse(Environment.GetEnvironmentVariable($"{APPSETTINGS}_REQUESTLOGGING_INCOMINGENABLED"), out bool incomingRequestLoggingEnabled))
      {
        RequestLogging.IncomingEnabled = incomingRequestLoggingEnabled;
      }

      if(bool.TryParse(Environment.GetEnvironmentVariable($"{APPSETTINGS}_REQUESTLOGGING_OUTGOINGENABLED"), out bool outgoingRequestLoggingEnabled))
      {
        RequestLogging.OutgoingEnabled = outgoingRequestLoggingEnabled;
      }

      if(bool.TryParse(Environment.GetEnvironmentVariable($"{APPSETTINGS}_REQUESTLOGGING_LOGPAYLOAD"), out bool outgoingRequestLogPayload))
      {
        RequestLogging.LogPayload = outgoingRequestLogPayload;
      }

      if(bool.TryParse(Environment.GetEnvironmentVariable($"{APPSETTINGS}_REQUESTLOGGING_LOGPAYLOADONERROR"), out bool outgoingRequestLogPayloadOnError))
      {
        RequestLogging.LogPayloadOnError = outgoingRequestLogPayloadOnError;
      }

    }
  }

  public class RequestLogging
  {
    public bool IncomingEnabled { get; set; }
    public bool OutgoingEnabled { get; set; }
    public bool LogPayload { get; set; }
    public bool LogPayloadOnError { get; set; }
  }
}
