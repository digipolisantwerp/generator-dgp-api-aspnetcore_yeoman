using System;
using System.Collections.Generic;
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

    private IEnumerable<string> GetStringsFromConfigSection(IConfiguration section, string subSectionKey, IEnumerable<string> defaultIfMissing)
    {
      var result = section.GetSection(subSectionKey).Get<IEnumerable<string>>();

      return result != null ? result : defaultIfMissing;
    }

    private void LoadFromConfigSection(IConfiguration section)
    {
      section.Bind(this);

      this.RequestLogging.AllowedIncomingRequestHeaders = GetStringsFromConfigSection(section, "RequestLogging:AllowedIncomingRequestHeaders", this.RequestLogging.AllowedIncomingRequestHeaders);
      this.RequestLogging.AllowedIncomingResponseHeaders = GetStringsFromConfigSection(section, "RequestLogging:AllowedIncomingResponseHeaders", this.RequestLogging.AllowedIncomingResponseHeaders);
      this.RequestLogging.AllowedOutgoingRequestHeaders = GetStringsFromConfigSection(section, "RequestLogging:AllowedOutgoingRequestHeaders", this.RequestLogging.AllowedOutgoingRequestHeaders);
      this.RequestLogging.AllowedOutgoingResponseHeaders = GetStringsFromConfigSection(section, "RequestLogging:AllowedOutgoingResponseHeaders", this.RequestLogging.AllowedOutgoingResponseHeaders);
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

      if (bool.TryParse(Environment.GetEnvironmentVariable($"{APPSETTINGS}_REQUESTLOGGING_INCOMINGENABLED"),
          out bool incomingRequestLoggingEnabled))
      {
        RequestLogging.IncomingEnabled = incomingRequestLoggingEnabled;
      }

      if (bool.TryParse(Environment.GetEnvironmentVariable($"{APPSETTINGS}_REQUESTLOGGING_OUTGOINGENABLED"),
          out bool outgoingRequestLoggingEnabled))
      {
        RequestLogging.OutgoingEnabled = outgoingRequestLoggingEnabled;
      }

      if (bool.TryParse(Environment.GetEnvironmentVariable($"{APPSETTINGS}_REQUESTLOGGING_LOGPAYLOAD"),
          out bool outgoingRequestLogPayload))
      {
        RequestLogging.LogPayload = outgoingRequestLogPayload;
      }

      if (bool.TryParse(Environment.GetEnvironmentVariable($"{APPSETTINGS}_REQUESTLOGGING_LOGPAYLOADONERROR"),
          out bool outgoingRequestLogPayloadOnError))
      {
        RequestLogging.LogPayloadOnError = outgoingRequestLogPayloadOnError;
      }

      var allowedIncomingRequestHeaders = Environment.GetEnvironmentVariable($"{APPSETTINGS}_REQUESTLOGGING_ALLOWEDINCOMINGREQUESTHEADERS");
      if (allowedIncomingRequestHeaders != null)
      {
        RequestLogging.AllowedIncomingRequestHeaders = !string.IsNullOrWhiteSpace(allowedIncomingRequestHeaders) ? allowedIncomingRequestHeaders.Split(",") : null;
      }

      var allowedIncomingResponseHeaders = Environment.GetEnvironmentVariable($"{APPSETTINGS}_REQUESTLOGGING_ALLOWEDINCOMINGRESPONSEHEADERS");
      if (allowedIncomingResponseHeaders != null)
      {
        RequestLogging.AllowedIncomingResponseHeaders = !string.IsNullOrWhiteSpace(allowedIncomingResponseHeaders) ? allowedIncomingResponseHeaders.Split(",") : null;
      }

      var allowedOutgoingRequestHeaders = Environment.GetEnvironmentVariable($"{APPSETTINGS}_REQUESTLOGGING_ALLOWEDOUTGOINGREQUESTHEADERS");
      if (allowedOutgoingRequestHeaders != null)
      {
        RequestLogging.AllowedOutgoingRequestHeaders = !string.IsNullOrWhiteSpace(allowedOutgoingRequestHeaders) ? allowedOutgoingRequestHeaders.Split(",") : null;
      }

      var allowedOutgoingResponseHeaders = Environment.GetEnvironmentVariable($"{APPSETTINGS}_REQUESTLOGGING_ALLOWEDOUTGOINGRESPONSEHEADERS");
      if (allowedOutgoingResponseHeaders != null)
      {
        RequestLogging.AllowedOutgoingResponseHeaders = !string.IsNullOrWhiteSpace(allowedOutgoingResponseHeaders) ? allowedOutgoingResponseHeaders.Split(",") : null;
      }
    }
  }

  public class RequestLogging
  {
    public RequestLogging()
    {
      AllowedIncomingRequestHeaders = DefaultAllowedRequestHeaders;
      AllowedIncomingResponseHeaders = DefaultAllowedResponseHeaders;
      AllowedOutgoingRequestHeaders = DefaultAllowedRequestHeaders;
      AllowedOutgoingResponseHeaders = DefaultAllowedResponseHeaders;
    }

    public bool IncomingEnabled { get; set; }
    public bool OutgoingEnabled { get; set; }
    public bool LogPayload { get; set; }
    public bool LogPayloadOnError { get; set; }

    public IEnumerable<string> AllowedIncomingRequestHeaders { get; set; }
    public IEnumerable<string> AllowedIncomingResponseHeaders { get; set; }
    public IEnumerable<string> AllowedOutgoingRequestHeaders { get; set; }
    public IEnumerable<string> AllowedOutgoingResponseHeaders { get; set; }

    public readonly IEnumerable<string> DefaultAllowedRequestHeaders = new List<string>()
    {
      // Standard request headers
      "Accept",
      "Accept-Charset",
      "Accept-Datetime",
      "Accept-Encoding",
      "Accept-Language",
      "Access-Control-Request-Method",
      "Access-Control-Request-Headers",
      // "Authorization",
      "Cache-Control",
      "Connection",
      "Content-Encoding",
      "Content-Length",
      "Content-Type",
      // "Cookie",
      "Date",
      "Expect",
      "Forwarded",
      "From",
      "Host",
      "HTTP2-Settings",
      "If-Match",
      "If-Modified-Since",
      "If-None-Match",
      "If-Range",
      "If-Unmodified-Since",
      "Max-Forwards",
      "Origin",
      "Pragma",
      "Prefer",
      // "Proxy-Authorization",
      "Range",
      "Referer",
      "TE",
      "Trailer",
      "Transfer-Encoding",
      "User-Agent",
      "Upgrade",
      "Via",
      "Warning",

      // Non-standard request headers
      "X-Forwarded-For",
      "X-Forwarded-Host",
      "X-Forwarded-Proto",

      // Digipolis-specific request headers
      "Dgp-Correlation"
    };

    public readonly IEnumerable<string> DefaultAllowedResponseHeaders = new List<string>()
    {
      // Standard response headers
      "Accept-CH",
      "Access-Control-Allow-Origin",
      "Access-Control-Allow-Credentials",
      "Access-Control-Expose-Headers",
      "Access-Control-Max-Age",
      "Access-Control-Allow-Methods",
      "Access-Control-Allow-Headers",
      "Accept-Patch",
      "Accept-Ranges",
      "Age",
      "Allow",
      "Alt-Svc",
      "Cache-Control",
      "Connection",
      "Content-Disposition",
      "Content-Encoding",
      "Content-Language",
      "Content-Length",
      "Content-Location",
      "Content-Range",
      "Content-Type",
      "Date",
      "Delta-Base",
      "ETag",
      "Expires",
      "IM",
      "Last-Modified",
      "Link",
      "Location",
      "P3P",
      "Pragma",
      "Preference-Applied",
      "Proxy-Authenticate",
      "Public-Key-Pins",
      "Retry-After",
      "Server",
      // "Set-Cookie",
      "Strict-Transport-Security",
      "Trailer",
      "Transfer-Encoding",
      "Tk",
      "Upgrade",
      "Vary",
      "Via",
      "Warning",
      "WWW-Authenticate"
    };
  }
}
