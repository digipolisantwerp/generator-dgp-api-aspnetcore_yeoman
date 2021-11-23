using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StarterKit.ServiceAgents._base;
using System.Collections.Generic;

namespace StarterKit.ServiceAgents.ExampleServiceAgent
{
  public class ExampleServiceAgentSettings : AgentSettingsBase
    {
        public static void RegisterConfiguration(IServiceCollection services,
                                                 IConfigurationSection section,
                                                 IHostEnvironment env)
        {
            services.Configure<ExampleServiceAgentSettings>(settings =>
            {
                settings.LoadFromConfigSection(section);
                settings.OverrideFromEnvironmentVariables(env, System.Environment.GetEnvironmentVariables());
            });
        }

        protected override void OverrideFromEnvironmentVariables(IHostEnvironment env, System.Collections.IDictionary envVariables)
        {
            if (!(env.IsDevelopment() || env.IsEnvironment(StarterKit.Shared.Constants.RuntimeEnvironment.IntegrationTesting)))
            {
                Scheme = GetValue(Scheme, Config.Scheme, env);
                Host = GetValue(Host, Config.Host, env);
                Path = GetValue(Path, Config.Path, env);

                // iterate possible headers in environment variables
                // environment variables may only contain underscores as non-alphanumeric character; so name and value of headers are split into two
                // separate environment variables to allow dash-symbols (ex. owner-key)
                int headerIndex = 0;
                var headerKeyVariable = GetHeaderKeyVariableName(headerIndex);

                if (envVariables.Contains(headerKeyVariable)) Headers = new Dictionary<string, string>();

                while (envVariables.Contains(headerKeyVariable))
                {
                    var headerKeyValue = GetValue(string.Empty, headerKeyVariable, env);

                    var headerValueVar = GetHeaderValueVariableName(headerIndex);
                    var headerValueValue = GetValue(string.Empty, headerValueVar, env);

                    if (Headers.ContainsKey(headerKeyValue))
                    {
                        Headers.Remove(headerKeyValue);
                        Headers.Add(headerKeyValue, headerValueValue);
                    }

                    // get next header
                    headerIndex++;
                    headerKeyVariable = GetHeaderKeyVariableName(headerIndex);
                }
            }
        }

        /// <summary>
        /// get environment variable name of the desired service agent header name ex. SERVICEAGENTS_ExampleServiceAgent_HEADERS_0_NAME
        /// </summary>
        private static string GetHeaderKeyVariableName(int index)
        {
            return ($"{Config.Headers}_{index}_NAME").ToUpper();
        }

        /// <summary>
        /// get environment variable name of the desired service agent header value ex. SERVICEAGENTS_ExampleServiceAgent_HEADERS_0_VALUE
        /// </summary>
        private static string GetHeaderValueVariableName(int index)
        {
            return ($"{Config.Headers}_{index}_VALUE").ToUpper();
        }
    }
}
