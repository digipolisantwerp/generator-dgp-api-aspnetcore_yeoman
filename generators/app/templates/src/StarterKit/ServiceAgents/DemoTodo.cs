using System;
using System.Net.Http;
using System.Threading.Tasks;
using Digipolis.ServiceAgents;
using Digipolis.ServiceAgents.Settings;
using Microsoft.Extensions.Options;

namespace StarterKit.ServiceAgents
{
    public class DemoTodo : AgentBase, IDemoTodo
    {
        public DemoTodo(HttpClient client, IServiceProvider serviceProvider, IOptions<ServiceAgentSettings> options) 
            : base(client, serviceProvider, options)
        {
        }

        public Task<string> GetTodosAsStringAsync()
        {
            return GetStringAsync("todos/1");
        }
    }
}
