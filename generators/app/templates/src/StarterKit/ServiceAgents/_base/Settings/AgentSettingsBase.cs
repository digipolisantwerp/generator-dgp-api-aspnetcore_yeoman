using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using StarterKit.Shared;
using System;
using System.Collections.Generic;

namespace StarterKit.ServiceAgents._base
{
    public abstract class AgentSettingsBase : SettingsBase
    {
        public string Scheme { get; set; }
        public string Host { get; set; }
        public string Path { get; set; }

        public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();

        public virtual string Url
        {
            get
            {
                return $"{Scheme}://{Host}/{Path}{(String.IsNullOrWhiteSpace(Path) ? "" : "/")}";
            }
        }

        protected void LoadFromConfigSection(IConfigurationSection section)
        {
            section.Bind(this);
        }

        protected abstract void OverrideFromEnvironmentVariables(IHostEnvironment env, System.Collections.IDictionary envVariables);
    }
}
