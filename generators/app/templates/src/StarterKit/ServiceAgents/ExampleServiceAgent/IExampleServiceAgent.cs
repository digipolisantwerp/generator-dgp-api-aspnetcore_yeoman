using System.Threading.Tasks;

namespace StarterKit.ServiceAgents.ExampleServiceAgent
{
  public interface IExampleServiceAgent
    {
        Task<object> Get(string id);
    }
}
