using System.Net.Http;
using System.Threading.Tasks;
using StarterKit.ServiceAgents._base.Settings;

namespace StarterKit.ServiceAgents._base.Helper
{
  public interface IRequestHeaderHelper
  {
    Task InitializeHeaders(HttpClient client, AgentSettingsBase settings);
    Task ValidateAuthHeaders(HttpClient client, AgentSettingsBase settings);
  }
}
