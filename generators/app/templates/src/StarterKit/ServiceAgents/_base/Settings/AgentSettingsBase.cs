using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using StarterKit.Shared;

namespace StarterKit.ServiceAgents._base.Settings
{
	public class AgentSettingsBase : SettingsBase
	{
		public string Scheme { get; set; }
		public string Host { get; set; }
		public string Path { get; set; }
		public string Port { get; set; }
		public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();
		public string AuthScheme { get; set; }
		public string OAuthClientId { get; set; }
		public string OAuthClientSecret { get; set; }
		public string OAuthScope { get; set; }
		public string OAuthPathAddition { get; set; }
		public string OAuthTokenEndpoint => $"{Url}{OAuthPathAddition}";
		public virtual string Url => $"{Scheme}://{Host}/{Path}{(string.IsNullOrWhiteSpace(Path) ? "" : "/")}";

		protected void LoadFromConfigSection(IConfigurationSection section)
		{
			section.Bind(this);
		}
	}
}