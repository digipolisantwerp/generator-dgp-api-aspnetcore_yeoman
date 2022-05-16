using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using StarterKit.ServiceAgents._base;

namespace StarterKit.ServiceAgents.ExampleServiceAgent
{
	public class ExampleServiceAgent : AgentBase<ExampleServiceAgent>, IExampleServiceAgent
	{
		public ExampleServiceAgent(ILogger<ExampleServiceAgent> logger, HttpClient httpClient,
			IServiceProvider serviceProvider)
			: base(logger, httpClient, serviceProvider)
		{
		}

		public Task<object> Get(string id)
		{
			// service agent code to retrieve data

			return Task.FromResult(new object());
		}
	}
}