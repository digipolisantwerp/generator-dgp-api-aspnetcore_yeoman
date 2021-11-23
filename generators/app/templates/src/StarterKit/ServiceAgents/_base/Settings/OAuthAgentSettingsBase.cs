namespace StarterKit.ServiceAgents._base
{
    public abstract class OAuthAgentSettingsBase : AgentSettingsBase
    {
        public string AuthScheme { get; set; }

        public string OAuthClientId { get; set; }
        public string OAuthClientSecret { get; set; }
        public string OAuthScope { get; set; }
        public string OAuthPathAddition { get; set; }

        public string OAuthTokenEndpoint
        {
            get
            {
                return $"{Url}{OAuthPathAddition}";
            }
        }
    }
}
