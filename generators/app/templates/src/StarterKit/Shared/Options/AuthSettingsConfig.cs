using System;
using Digipolis.Auth.Options;
using Microsoft.Extensions.Hosting;

namespace StarterKit.Shared.Options
{
  public class AuthSettingsConfig
  {
    public static void SetConfig(AuthOptions authOptions, IHostEnvironment env)
    {
      if (env.IsDevelopment()) return;

      authOptions.ApplicationName = Environment.GetEnvironmentVariable("AUTH_APPLICATIONNAME");
      if (string.IsNullOrWhiteSpace(authOptions.ApplicationName)) throw new ArgumentNullException(nameof(authOptions.ApplicationName), "Configuration error: invalid parameter AUTH_APPLICATIONNAME");

      var parseResultSuccess = bool.TryParse(Environment.GetEnvironmentVariable("AUTH_ENABLEJWTHEADERAUTH"), out var enableJwtHeaderAuth);
      if (!parseResultSuccess) throw new ArgumentNullException(nameof(authOptions.EnableJwtHeaderAuth), "Configuration error: invalid parameter AUTH_ENABLEJWTHEADERAUTH");
      authOptions.EnableJwtHeaderAuth = enableJwtHeaderAuth;

      authOptions.PdpUrl = Environment.GetEnvironmentVariable("AUTH_PDPURL");
      if (string.IsNullOrWhiteSpace(authOptions.PdpUrl)) throw new ArgumentNullException(nameof(authOptions.PdpUrl), "Configuration error: invalid parameter AUTH_PDPURL");

      authOptions.ApiKey = Environment.GetEnvironmentVariable("AUTH_APIKEY");
      if (string.IsNullOrWhiteSpace(authOptions.ApiKey)) throw new ArgumentNullException(nameof(authOptions.ApiKey), "Configuration error: invalid parameter AUTH_APIKEY");

      //Default 60
      parseResultSuccess = int.TryParse(Environment.GetEnvironmentVariable("AUTH_PDPCACHEDURATION"), out int pdpCacheDuration);
      if (!parseResultSuccess) throw new ArgumentNullException(nameof(authOptions.PdpCacheDuration), "Configuration error: invalid parameter AUTH_PDPCACHEDURATION");
      authOptions.PdpCacheDuration = pdpCacheDuration;

      authOptions.JwtAudience = Environment.GetEnvironmentVariable("AUTH_JWTAUDIENCE");
      if (string.IsNullOrWhiteSpace(authOptions.JwtAudience)) throw new ArgumentNullException(nameof(authOptions.JwtAudience), "Configuration error: invalid parameter AUTH_JWTAUDIENCE");

      authOptions.JwtTokenSource = Environment.GetEnvironmentVariable("AUTH_JWTTOKENSOURCE");
      if (string.IsNullOrWhiteSpace(authOptions.JwtTokenSource)) throw new ArgumentNullException("Configuration error: invalid parameter AUTH_JWTTOKENSOURCE");
    }
  }
}
