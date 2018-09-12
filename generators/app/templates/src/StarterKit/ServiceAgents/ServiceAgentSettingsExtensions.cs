using Digipolis.ServiceAgents.Settings;
using System;

namespace StarterKit.ServiceAgents
{
  public class ServiceAgentSettingsExtensions
  {
    public static void SetConfig(ServiceAgentSettings serviceAgentSettings)
    {
      var env = Environment.GetEnvironmentVariables();

      foreach (var agent in serviceAgentSettings.Services)
      {
        var settings = agent.Value;
        var agentName = agent.Key.ToUpper();
        var agentConfigName = $"SERVICEAGENTS_{agentName}";

        settings.AuthScheme = env.Contains($"{agentConfigName}_AUTHSCHEME") ? env[$"{agentConfigName}_AUTHSCHEME"].ToString() : settings.AuthScheme;
        settings.BasicAuthPassword = env.Contains($"{agentConfigName}_BASICAUTHPASSWORD") ? env[$"{agentConfigName}_BASICAUTHPASSWORD"].ToString() : settings.BasicAuthPassword;
        settings.BasicAuthUserName = env.Contains($"{agentConfigName}_BASICAUTHUSERNAME") ? env[$"{agentConfigName}_BASICAUTHUSERNAME"].ToString() : settings.BasicAuthUserName;
        settings.Host = env.Contains($"{agentConfigName}_HOST") ? env[$"{agentConfigName}_HOST"].ToString() : settings.Host;
        settings.Path = env.Contains($"{agentConfigName}_PATH") ? env[$"{agentConfigName}_PATH"].ToString() : settings.Path;
        settings.Port = env.Contains($"{agentConfigName}_PORT") ? env[$"{agentConfigName}_PORT"].ToString() : settings.Port;
        settings.Scheme = env.Contains($"{agentConfigName}_SCHEME") ? env[$"{agentConfigName}_SCHEME"].ToString() : settings.Scheme;
        settings.OAuthClientId = env.Contains($"{agentConfigName}_OAUTHCLIENTID") ? env[$"{agentConfigName}_OAUTHCLIENTID"].ToString() : settings.OAuthClientId;
        settings.OAuthClientSecret = env.Contains($"{agentConfigName}_OAUTHCLIENTSECRET") ? env[$"{agentConfigName}_OAUTHCLIENTSECRET"].ToString() : settings.OAuthClientSecret;
        settings.OAuthPathAddition = env.Contains($"{agentConfigName}_OAUTHPATHADDITION") ? env[$"{agentConfigName}_OAUTHPATHADDITION"].ToString() : settings.OAuthPathAddition;

        if (settings.Headers == null)
          continue;

        // iterate possible headers in environment variables;
        // environment variables may only contain underscores as non-alphanumeric character; so name and value of headers are split into two
        // separate environment variables to allow other non-alphanumeric than dash-symbols (ex. "owner-key")
        // "headers": [
        //    {
        //      "value": "1234567a-7654-abcf-b12b-34bedfded2ae",
        //      "name": "apikey"
        //    }, ...
        //  ]
        int headerIndex = 0;
        var envHeaderName = GetServiceHeaderNameEnvVarName(agentConfigName, headerIndex);

        while (env.Contains(envHeaderName))
        {
          var envHeaderValue = GetServiceHeaderValueEnvVarName(agentConfigName, headerIndex);

          var headerName = env[envHeaderName].ToString();
          var headerValue = env.Contains(envHeaderValue) ? env[envHeaderValue].ToString() : string.Empty;

          if (settings.Headers.ContainsKey(headerName))
          {
            settings.Headers.Remove(headerName);
            settings.Headers.Add(headerName, headerValue);
          }

          headerIndex++;
          envHeaderName = GetServiceHeaderNameEnvVarName(agentConfigName, headerIndex);
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
