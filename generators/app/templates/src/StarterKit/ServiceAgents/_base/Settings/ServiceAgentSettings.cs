using System;
using System.Collections.Generic;
using System.Linq;

namespace StarterKit.ServiceAgents._base.Settings
{
  public class ServiceAgentSettings
  {
    public ServiceAgentSettings()
    {
      Services = new Dictionary<string, AgentSettingsBase>();
    }

    public IDictionary<string, AgentSettingsBase> Services { get; private set; }

    /// <summary>
    /// search service agent settings by service type name
    /// </summary>
    public AgentSettingsBase GetServiceSettings(string typeName)
    {
      // corresponding type name and service setting key
      if (Services.Any(s => string.Equals(s.Key, typeName, StringComparison.CurrentCultureIgnoreCase)))
      {
        return Services.First(s => string.Equals(s.Key, typeName, StringComparison.CurrentCultureIgnoreCase)).Value;
      }

      // service setting key is part of type name ex. (type name) MyServiceAgent <>  (service settings key) MyService
      if (Services.Any(s => typeName.ToLower().Contains(s.Key.ToLower())))
      {
        return Services.FirstOrDefault(s => typeName.ToLower().Contains(s.Key.ToLower())).Value;
      }

      throw new Exception($"Settings not found for service agent {typeName}");
    }
  }
}
