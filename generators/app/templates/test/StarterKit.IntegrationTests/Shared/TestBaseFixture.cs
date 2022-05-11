using System;
using System.Collections.Generic;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using StarterKit.IntegrationTests.Startup;
using Xunit;

namespace StarterKit.IntegrationTests.Shared
{
  // This class defines the shared context between all the tests
  public class TestBaseFixture: IDisposable
  {
    public readonly IList<Mock> Mocks = new List<Mock>();
    
    public readonly HttpClient Client;

    private readonly TestWebApplicationFactory<StarterKit.Startup.Startup> _factory;
    private readonly IServiceProvider _serviceProvider;

    // Define your mocks here so you can access them in your test classes
    // fe: public readonly Mock<IMocked> MockedMock;

    public TestBaseFixture()
    {
      _factory = new TestWebApplicationFactory<StarterKit.Startup.Startup>();
      Client = _factory.CreateClient();

      using var scope = _factory.Server.Host.Services.CreateScope();
      _serviceProvider = _factory.Server.Host.Services;

      // Initialize your mock in scope. Helper method AddMock will get the required mock from the service. 
      // this.MockedMock = AddMock<IMocked>(scope);
    }

    public T GetService<T>() => (T)_serviceProvider.GetService(typeof(T));

    private Mock<T> AddMock<T>(IServiceScope scope) where T : class
    {
      var mock = scope.ServiceProvider.GetRequiredService<Mock<T>>();
      Mocks.Add(mock);
      return mock;
    }

    public void Dispose()
    {
      Client?.Dispose();
      _factory?.Dispose();

      foreach (var mock in Mocks)
      {
          mock.Reset();
      }
    }
  }

  #region Test collections

  // Test collections: grouping of test classes which use the same fixture
  [CollectionDefinition(TestConstants.Collections.ControllerTests)]
  public class TestBaseCollection : ICollectionFixture<TestBaseFixture>
  {
  }

  #endregion
}
