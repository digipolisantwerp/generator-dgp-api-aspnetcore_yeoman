using AutoMapper;
using Microsoft.Extensions.Logging;
using StarterKit.ServiceAgents._base;
using System.Net.Http;
using System.Threading.Tasks;

namespace StarterKit.ServiceAgents.ExampleServiceAgent
{
  public class ExampleServiceAgent : AgentBase<ExampleServiceAgent>, IExampleServiceAgent
  {
    public ExampleServiceAgent(ILogger<ExampleServiceAgent> logger, IMapper mapper,
                               HttpClient httpClient)
        : base(logger, httpClient)
    {

    }

    public async Task<object> Get(string id)
    {
      // service agent code to retrieve data

      return Task.FromResult(new object());
    }

  }
}
