using System.Threading.Tasks;

namespace StarterKit.ServiceAgents._base.Auth
{
    public interface IOAuthAgent
    {
        Task<string> ReadOrRetrieveAccessToken<TServiceAgentSettings>(TServiceAgentSettings settings) where TServiceAgentSettings : OAuthAgentSettingsBase;
    }
}