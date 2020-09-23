using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace StarterKit.IntegrationTests.Startup
{
  public class TestStartup: StarterKit.Startup.Startup
  {
    // Define readonly mocks here
    // fe: private readonly Mock<IMocked> _mockedMock = new Mock<IMocked>();
    public TestStartup(IConfiguration configuration, IHostEnvironment env) : base(configuration, env)
    {
    }

    public override void ConfigureServices(IServiceCollection services)
    {
      // Add your mock objects as a singleton
      // fe: services.AddSingleton(_mockedMock);
      base.ConfigureServices(services);

      // Add the mocked object as a singleton
      // fe: services.AddSingleton(_mockedMock.Object);
    }
  }
}
