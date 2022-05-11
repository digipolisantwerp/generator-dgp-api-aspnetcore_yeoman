using Microsoft.Extensions.Hosting;
using StarterKit.Shared.Options;

namespace StarterKit.ServiceAgents._base.Settings
{
  public class ServiceAgentSettingsExtensions
  {
    public static void SetConfig(ServiceAgentSettings serviceAgentSettings, IHostEnvironment env)
    {
      if (env.IsDevelopment()) return;

      foreach (var agent in serviceAgentSettings.Services)
      {
        var settings = agent.Value;
        var agentName = agent.Key.ToUpper();
        var agentConfigName = $"SERVICEAGENTS_{agentName}";

        settings.AuthScheme = ConfigUtil.GetEnvironmentVariable($"{agentConfigName}_AUTHSCHEME", true, settings.AuthScheme);
       
        settings.Host = ConfigUtil.GetEnvironmentVariable($"{agentConfigName}_HOST", false);
        settings.Path = ConfigUtil.GetEnvironmentVariable($"{agentConfigName}_PATH", false);
        settings.Port = ConfigUtil.GetEnvironmentVariable($"{agentConfigName}_PORT", true);
        settings.Scheme = ConfigUtil.GetEnvironmentVariable($"{agentConfigName}_SCHEME", false);

        settings.OAuthClientId = ConfigUtil.GetEnvironmentVariable($"{agentConfigName}_OAUTHCLIENTID", true, settings.OAuthClientId);
        settings.OAuthClientSecret = ConfigUtil.GetEnvironmentVariable($"{agentConfigName}_OAUTHCLIENTSECRET", true, settings.OAuthClientSecret);
        settings.OAuthPathAddition = ConfigUtil.GetEnvironmentVariable($"{agentConfigName}_OAUTHPATHADDITION", true, settings.OAuthPathAddition);

        if (settings.Headers == null)
          continue;

        // iterate possible headers in environment variables;
        // environment variables may only contain underscores as non-alphanumeric character; so name and value of headers are split into two
        // separate environment variables to allow other non-alphanumeric charachters such as a dash-symbol (ex. "owner-key")
        // "headers": [
        //    {
        //      "value": "123456a-7890-abcd-efg2-34bedfd3d210",
        //      "name": "apikey"
        //    }, ...
        //  ]
        int headerIndex = 0;
        var variableNameHeaderName = GetServiceHeaderNameEnvVarName(agentConfigName, headerIndex);
        var headerName = ConfigUtil.GetEnvironmentVariable(variableNameHeaderName, true);

        while (! string.IsNullOrWhiteSpace(headerName))
        {
          var variableNameHeaderValue = GetServiceHeaderValueEnvVarName(agentConfigName, headerIndex);
          var headerValue = ConfigUtil.GetEnvironmentVariable(variableNameHeaderValue, true, string.Empty);

          if (settings.Headers.ContainsKey(headerName))
          {
            settings.Headers.Remove(headerName);
            settings.Headers.Add(headerName, headerValue);
          }

          headerIndex++;
          variableNameHeaderName = GetServiceHeaderNameEnvVarName(agentConfigName, headerIndex);
          headerName = ConfigUtil.GetEnvironmentVariable(variableNameHeaderName, true);
        }
      }
    }

    /// <summary>
    /// get environment variable name of the desired service agent header name ex. SERVICEAGENTS_EVENTHANDLERAGENT_HEADERS_0_NAME
    /// </summary>
    private static string GetServiceHeaderNameEnvVarName(string agentConfigName, int index)
    {
      return ($"{agentConfigName}_HEADERS_{index.ToString()}_NAME").ToUpper();
    }

    /// <summary>
    /// get environment variable name of the desired service agent header value ex. SERVICEAGENTS_EVENTHANDLERAGENT_HEADERS_0_VALUE
    /// </summary>
    private static string GetServiceHeaderValueEnvVarName(string agentConfigName, int index)
    {
      return ($"{agentConfigName}_HEADERS_{index.ToString()}_VALUE").ToUpper();
    }
  }
}
