using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;

namespace StarterKit.IntegrationTests.Startup
{
  public class TestWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
  {
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
      builder.UseStartup<TestStartup>()
        .UseEnvironment(Environments.Development);
    }
  }
}
