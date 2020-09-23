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
  public class TestBaseFixture
  {
    public readonly IList<Mock> Mocks = new List<Mock>();
    
    public readonly HttpClient Client;

    private readonly TestWebApplicationFactory<StarterKit.Startup.Startup> factory;
    private readonly IServiceProvider serviceProvider;

    // Define your mocks here so you can access them in your test classes
    // fe: public readonly Mock<IMocked> MockedMock;

    public TestBaseFixture()
    {
      this.factory = new TestWebApplicationFactory<StarterKit.Startup.Startup>();

      // Transaction scope is running into issues in .net core 3.1
      // https://github.com/dotnet/aspnetcore/issues/18001 
      factory.Server.PreserveExecutionContext = true;

      this.Client = factory.CreateClient();

      using var scope = factory.Server.Host.Services.CreateScope();
      this.serviceProvider = factory.Server.Host.Services;

      // Initialize your mock in scope. Helper method AddMock will get the required mock from the service. 
      // this.MockedMock = AddMock<IMocked>(scope);
    }

    public T GetService<T>() => (T)serviceProvider.GetService(typeof(T));

    private Mock<T> AddMock<T>(IServiceScope scope) where T : class
    {
      var mock = scope.ServiceProvider.GetRequiredService<Mock<T>>();
      Mocks.Add(mock);
      return mock;
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
