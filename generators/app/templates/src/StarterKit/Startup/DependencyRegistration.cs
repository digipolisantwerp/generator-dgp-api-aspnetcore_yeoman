using Microsoft.Extensions.DependencyInjection;
using StarterKit.Business.Monitoring;

namespace StarterKit
{
  public static class DependencyRegistration
  {
    public static IServiceCollection AddBusinessServices(this IServiceCollection services)
    {
      // Register your business services here, e.g. services.AddTransient<IMyService, MyService>();

      services.AddTransient<IStatusReader, StatusReader>();

      return services;
    }
  }
}
