using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StarterKit.Shared.Options;

namespace StarterKit.ServiceAgents.Options
{

  public class ServiceAgentBaseSettings
  {
    public string AuthScheme { get; set; }
    public string Scheme { get; set; }
    public string Host { get; set; }
    public string Path { get; set; }
    public string Port { get; set; }

    public Dictionary<string, string> Headers { get; set; }

    public virtual string Url
    {
      get
      {
        return $"{Scheme}://{Host}{(String.IsNullOrWhiteSpace(Port) ? "" : $":{ Port}")}/{Path}{(String.IsNullOrWhiteSpace(Path) ? "" : "/")}";        
      }
    }

    public static void RegisterConfiguration(IServiceCollection services, IConfigurationSection section, IWebHostEnvironment env)
    {
      services.Configure<ServiceAgentBaseSettings>(settings =>
      {
        settings.LoadFromConfigSection(section);
        settings.OverrideFromEnvironmentVariables(env);
      });
    }

    private void LoadFromConfigSection(IConfiguration section)
    {
      section.Bind(this);
    }

    private void OverrideFromEnvironmentVariables(IHostEnvironment env)
    {
      if (env.IsDevelopment()) return;

      var agentName = this.GetType().Name.ToUpper();
      var agentConfigName = $"SERVICEAGENTS_{agentName}";

      Scheme = ConfigUtil.GetEnvironmentVariable($"{agentConfigName}_SCHEME", false);
      Host = ConfigUtil.GetEnvironmentVariable($"{agentConfigName}_HOST", false);
      Path = ConfigUtil.GetEnvironmentVariable($"{agentConfigName}_PATH", false);
      Port = ConfigUtil.GetEnvironmentVariable($"{agentConfigName}_PORT", true, string.Empty);

      AuthScheme = ConfigUtil.GetEnvironmentVariable($"{agentConfigName}_AUTHSCHEME", false);

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

      while (!string.IsNullOrWhiteSpace(headerName))
      {
        var variableNameHeaderValue = GetServiceHeaderValueEnvVarName(agentConfigName, headerIndex);
        var headerValue = ConfigUtil.GetEnvironmentVariable(variableNameHeaderValue, true, string.Empty);

        if (Headers.ContainsKey(headerName))
        {
          Headers.Remove(headerName);
          Headers.Add(headerName, headerValue);
        }

        headerIndex++;
        variableNameHeaderName = GetServiceHeaderNameEnvVarName(agentConfigName, headerIndex);
        headerName = ConfigUtil.GetEnvironmentVariable(variableNameHeaderName, true);
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
