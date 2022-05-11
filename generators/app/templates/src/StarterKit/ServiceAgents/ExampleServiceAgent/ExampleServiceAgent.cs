using System;
using Microsoft.Extensions.Logging;
using StarterKit.ServiceAgents._base;
using System.Net.Http;
using System.Threading.Tasks;

namespace StarterKit.ServiceAgents.ExampleServiceAgent
{
  public class ExampleServiceAgent : AgentBase<ExampleServiceAgent>, IExampleServiceAgent
  {
    public ExampleServiceAgent(ILogger<ExampleServiceAgent> logger, HttpClient httpClient, IServiceProvider serviceProvider)
        : base(logger, httpClient, serviceProvider)
    {

    }

    public Task<object> Get(string id)
    {
      // service agent code to retrieve data

      return Task.FromResult<object>(new object());
    }

  }
}
