using System;

namespace StarterKit.ServiceAgents._base.Auth
{
    [Serializable]
    public class TokenReply
    {
        public string token_type { get; set; }
        public string access_token { get; set; }
        public int expires_in { get; set; }
        public string refresh_token { get; set; }
    }
}
