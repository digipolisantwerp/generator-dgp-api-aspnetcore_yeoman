using Microsoft.Extensions.DependencyInjection;
using StarterKit.Business.Monitoring;

namespace StarterKit.Startup
{
  public static class DependencyRegistration
  {
    public static IServiceCollection AddBusinessServices(this IServiceCollection services)
    {
      // Register your business services here, e.g. services.AddTransient<IMyService, MyService>();

      services.AddTransient<IStatusReader, StatusReader>();

      return services;
    }

    public static IServiceCollection AddServiceAgentServices(this IServiceCollection services)
    {
      return services;
    }

    public static IServiceCollection AddDataAccessServices(this IServiceCollection services)
    {
      return services;
    }

  }
}
