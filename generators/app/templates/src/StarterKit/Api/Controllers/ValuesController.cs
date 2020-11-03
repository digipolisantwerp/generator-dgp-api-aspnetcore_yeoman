using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StarterKit.ServiceAgents;

namespace StarterKit.Api.Controllers
{
    [Route("[controller]")]
    public class ValuesController : Controller
    {
        private readonly IDemoTodo _serviceAgent;

        public ValuesController(IDemoTodo serviceAgent)
        {
            _serviceAgent = serviceAgent;
        }

        // GET: values
        [HttpGet]
        public async Task<string> Get()
        {
            try
            {
                return await _serviceAgent.GetTodosAsStringAsync();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

    }
}
