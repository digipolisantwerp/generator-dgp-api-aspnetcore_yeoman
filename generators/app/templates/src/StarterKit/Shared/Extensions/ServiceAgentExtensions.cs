using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StarterKit.ServiceAgents._base;
using StarterKit.ServiceAgents._base.Auth;
using StarterKit.ServiceAgents._base.Helper;
using StarterKit.ServiceAgents._base.Settings;

namespace StarterKit.Shared.Extensions
{
  public static class ServiceAgentExtensions
  {
    public static IServiceCollection AddServiceAgents(this IServiceCollection services, IConfigurationSection configSection, Assembly serviceAgentAssembly, Assembly sharedAssembly,
                                                            IHostEnvironment environment,
                                IDictionary<string, Action<IHttpClientBuilder>> customRegistrations = null)
    {
      services.AddScoped<IOAuthAgent, OAuthAgent>();
      services.AddScoped<IRequestHeaderHelper, RequestHeaderHelper>();

      var addTypedClientMethodGeneric = typeof(HttpClientBuilderExtensions).GetMethod(nameof(HttpClientBuilderExtensions.AddTypedClient), 2, new[] { typeof(IHttpClientBuilder) });

      var assemblyTypes = serviceAgentAssembly.GetTypes();
      var sharedAssemblyTypes = sharedAssembly?.GetTypes();

      var serviceAgentSettings = FromSection(configSection);
      foreach (var serviceAgent in serviceAgentSettings.Services)
      {
        //Console.WriteLine($"Adding service agent '{serviceAgent.Key}' with '{Newtonsoft.Json.JsonConvert.SerializeObject(serviceAgent.Value)}'");
        if ((serviceAgent.Value.Headers?.Count ?? 0) > 0)
        {
          if ((serviceAgent.Value.Headers.TryGetValue("apikey", out var tempIdString)) && (!string.IsNullOrEmpty(tempIdString)) && (!Guid.TryParseExact(tempIdString, "D", out _)))
            throw new ArgumentException($"Invalid apikey with value '{serviceAgent.Value.OAuthClientId}' for service agent '{serviceAgent.Key}'");
          if ((serviceAgent.Value.Headers.TryGetValue("X-internal-apikey", out tempIdString)) && (!string.IsNullOrEmpty(tempIdString)) && (!Guid.TryParseExact(tempIdString, "D", out _)))
            throw new ArgumentException($"Invalid X-internal-apikey with value '{serviceAgent.Value.OAuthClientId}' for service agent '{serviceAgent.Key}'");
        }
        if ((!string.IsNullOrEmpty(serviceAgent.Value.OAuthClientId)) && (!Guid.TryParseExact(serviceAgent.Value.OAuthClientId, "D", out _)))
          throw new ArgumentException($"Invalid {nameof(AgentSettingsBase.OAuthClientId)} with value '{serviceAgent.Value.OAuthClientId}' for service agent '{serviceAgent.Key}'");
        if ((!string.IsNullOrEmpty(serviceAgent.Value.OAuthClientSecret)) && (!Guid.TryParseExact(serviceAgent.Value.OAuthClientSecret, "D", out _)))
          throw new ArgumentException($"Invalid {nameof(AgentSettingsBase.OAuthClientSecret)} with value '{serviceAgent.Value.OAuthClientSecret}' for service agent '{serviceAgent.Key}'");

        var serviceAgentType = assemblyTypes.FirstOrDefault(t => IsAssignableToGenericType(t.GetTypeInfo().BaseType, typeof(AgentBase<>)) &&
                                     t.Name.StartsWith(serviceAgent.Key, StringComparison.OrdinalIgnoreCase));

        if (serviceAgentType == null)
        {
          // Check for a shared agent type
          var sharedServiceAgentType = sharedAssemblyTypes?.FirstOrDefault(t => t.Name.Equals(serviceAgent.Key, StringComparison.OrdinalIgnoreCase));
          if (sharedServiceAgentType == null)
          {
            throw new ArgumentException($"Couldn't find class for service agent '{serviceAgent.Key}' in '{serviceAgentAssembly.FullName}' or '{sharedAssembly?.FullName}'.");
          }

          // NOTE: The service agents toolkit will use the name of this type as a registration key so it needs to be unique and we can't just use the line below
          //serviceAgentType = typeof(ServiceAgents.SharedAgent<>).MakeGenericType(sharedServiceAgentType);
          serviceAgentType = assemblyTypes.FirstOrDefault(t => IsAssignableToGenericType(t.GetTypeInfo().BaseType, typeof(AgentBase<>)) &&
                                                                         t.Name.StartsWith($"Shared{serviceAgent.Key}", StringComparison.OrdinalIgnoreCase));
        }

        // Now that we have the service agent implementation type try and find the interface type
        if (serviceAgentType != null)
        {
          var serviceAgentInterface = serviceAgentType
              .GetInterfaces()
              .SingleOrDefault(i => i.Name.Equals($"I{serviceAgent.Key}", StringComparison.InvariantCultureIgnoreCase) || i.Name.StartsWith("ISharedAgentBase", StringComparison.InvariantCultureIgnoreCase));
          if (serviceAgentInterface != null)
          {
            services.Configure<ServiceAgentSettings>(s =>
            {
              s.Services.Add(serviceAgentType.Name, serviceAgent.Value);
              ServiceAgentSettingsExtensions.SetConfig(s, environment);
            });

            var httpClientBuilder = services.AddHttpClient(serviceAgent.Key, (serviceProvider, cfgClient) =>
            {
              cfgClient.BaseAddress = new Uri(serviceAgent.Value.Url);

              var requestHeaderHelper = serviceProvider.GetService<IRequestHeaderHelper>();
              requestHeaderHelper.InitializeHeaders(cfgClient, serviceAgent.Value).Wait();
            });

            if (customRegistrations != null && customRegistrations.TryGetValue(serviceAgent.Key, out var customRegistration))
            {
              customRegistration(httpClientBuilder);
            }
            else
            {
              if (addTypedClientMethodGeneric == null) continue;
              var addTypedClientMethod = addTypedClientMethodGeneric.MakeGenericMethod(serviceAgentInterface, serviceAgentType);
              addTypedClientMethod.Invoke(httpClientBuilder, new object[] { httpClientBuilder });
            }
          }
          else
          {
            // We currently only support XXXAgent -> IXXXAgent or XXXAgent -> ISharedAgentBase<XXXAgent>
            throw new ArgumentException($"Couldn't find matching interface for service agent '{serviceAgent.Key}'");
          }
        }
      }

      return services;
    }

    internal static ServiceAgentSettings FromSection(IConfigurationSection configSection)
    {
      var serviceAgentSettings = new ServiceAgentSettings();
      var sections = configSection.GetChildren().ToDictionary(s => s.Key);
      foreach (var (key, value) in sections)
      {
        try
        {
          var settings = new AgentSettingsBase();
          value.Bind(settings);
          serviceAgentSettings.Services.Add(key, settings);
        }
        catch (Exception)
        {
          // ignored
        }
      }
      return serviceAgentSettings;
    }

    public static bool IsAssignableToGenericType(Type givenType, Type genericType)
    {
      if (givenType == null || genericType == null)
      {
        return false;
      }

      var interfaceTypes = givenType.GetInterfaces();

      if (interfaceTypes.Any(it => it.IsGenericType && it.GetGenericTypeDefinition() == genericType))
      {
        return true;
      }

      if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
        return true;

      var baseType = givenType.BaseType;
      return baseType != null && IsAssignableToGenericType(baseType, genericType);
    }
  }
}
