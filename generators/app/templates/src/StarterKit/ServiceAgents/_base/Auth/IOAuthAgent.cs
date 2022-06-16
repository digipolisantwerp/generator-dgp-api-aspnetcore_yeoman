using System.Threading.Tasks;
using StarterKit.ServiceAgents._base.Settings;

namespace StarterKit.ServiceAgents._base.Auth
{
	public interface IOAuthAgent
	{
		Task<string> ReadOrRetrieveAccessToken<TServiceAgentSettings>(TServiceAgentSettings settings)
			where TServiceAgentSettings : AgentSettingsBase;
	}
}