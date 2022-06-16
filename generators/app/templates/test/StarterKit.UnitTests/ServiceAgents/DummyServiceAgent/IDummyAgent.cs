using StarterKit.ServiceAgents._base.Settings;

namespace StarterKit.UnitTests.ServiceAgents.DummyServiceAgent
{
  public interface IDummyAgent
  {
    public AgentSettingsBase GetRegisteredSettings();
  }
}
