using System;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using StarterKit.ServiceAgents._base;
using StarterKit.ServiceAgents._base.Settings;

namespace StarterKit.UnitTests.ServiceAgents.DummyServiceAgent
{
  public class DummyAgent : ConfigInjectedAgentBase<DummyAgent>, IDummyAgent
  {
    public DummyAgent(ILogger<DummyAgent> logger, HttpClient httpClient, IServiceProvider serviceProvider) : base(logger, httpClient, serviceProvider)
    {
    }

    public AgentSettingsBase GetRegisteredSettings()
    {
      return Settings;
    }
  }
}
