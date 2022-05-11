using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting.Internal;
using StarterKit.Shared.Constants;
using StarterKit.Shared.Extensions;
using StarterKit.UnitTests.ServiceAgents.DummyServiceAgent;
using Xunit;

namespace StarterKit.UnitTests.ServiceAgents
{
  public class ServiceAgentSettingsExtensionsTests
  {
    private readonly HostingEnvironment _env = new HostingEnvironment { EnvironmentName = "Development" };
    private readonly ServiceCollection _sharedServices = new ServiceCollection();


    [Fact]
    public void WhenRegisteringServiceAgents_ShouldThrowException_WhenNoSuitableServiceAgentClassIsFound()
    {
      //arrange
      var services = new ServiceCollection();
      var configuration = BuildConfigurationRoot("TestAgent");

      //act
      void Act() => services.AddServiceAgents(configuration.GetSection(ConfigurationSectionKey.ServiceAgents), Assembly.Load(Assembly.GetExecutingAssembly().GetName().Name ?? "TestProject"), null, _env);

      //assert
      var exception = Assert.Throws<ArgumentException>(Act);
      Assert.StartsWith("Couldn't find class for service agent 'TestAgent'", exception.Message);
    }

    [Fact]
    public void WhenRegisteringServiceAgents_ShouldCorrectlyRegisterServiceAgentAndValues()
    {
      //arrange
      var configuration = BuildConfigurationRoot("DummyAgent");

      //act
      _sharedServices.AddServiceAgents(configuration.GetSection(ConfigurationSectionKey.ServiceAgents), Assembly.Load(Assembly.GetExecutingAssembly().GetName().Name ?? "TestProject"), null, _env);

      //assert
      var registrations = _sharedServices.Where(sd => sd.ServiceType == typeof(IDummyAgent)).ToArray();
      Assert.Single(registrations);
      Assert.Equal(ServiceLifetime.Transient, registrations[0].Lifetime);
    }


    [Fact]
    public void AfterRegisteringServiceAgents_ServiceAgentShouldContainCorrectSettings()
    {
      //arrange
      var configuration = BuildConfigurationRoot("DummyAgent");
      _sharedServices.AddServiceAgents(configuration.GetSection(ConfigurationSectionKey.ServiceAgents), Assembly.Load(Assembly.GetExecutingAssembly().GetName().Name ?? "TestProject"), null, _env);
      var sp = _sharedServices.BuildServiceProvider();
      var dummyAgentImplementation = sp.GetRequiredService<IDummyAgent>();

      //act
      var settings = dummyAgentImplementation.GetRegisteredSettings();

      //assert
      Assert.Equal("TheHost", settings.Host);
      Assert.Equal("ThePath", settings.Path);
      Assert.Equal("TestAgent", settings.AuthScheme);
      Assert.Equal("TheScheme", settings.Scheme);

      Assert.True(settings.Headers.Count == 2);
      Assert.Contains("ApiKey", settings.Headers.Keys);
      Assert.Contains("Accept", settings.Headers.Keys);

    }

    private static IConfigurationRoot BuildConfigurationRoot(string serviceName)
    {
      var configurationOptions = new Dictionary<string, string>
            {
                {$"ServiceAgents:{serviceName}:FriendlyName", "TestAgent"},
                {$"ServiceAgents:{serviceName}:AuthScheme", "TestAgent"},
                {$"ServiceAgents:{serviceName}:Headers:ApiKey", "ApiKey"},
                {$"ServiceAgents:{serviceName}:Headers:Accept", "application/hal+json"},
                {$"ServiceAgents:{serviceName}:Host", "TheHost"},
                {$"ServiceAgents:{serviceName}:Path", "ThePath"},
                {$"ServiceAgents:{serviceName}:Scheme", "TheScheme"},
            };

      return new ConfigurationBuilder()
          .AddInMemoryCollection(configurationOptions)
          .Build();
    }
  }
}
