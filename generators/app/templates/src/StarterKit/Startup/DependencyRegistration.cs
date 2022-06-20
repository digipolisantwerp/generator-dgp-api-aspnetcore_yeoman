using Microsoft.Extensions.DependencyInjection;
using StarterKit.Business.Monitoring;
using StarterKit.Framework.Logging.DelegatingHandler;

namespace StarterKit.Startup
{
	public static class DependencyRegistration
	{
		public static void AddBusinessServices(this IServiceCollection services)
		{
			// Register your business services here, e.g. services.AddTransient<IMyService, MyService>();

			services.AddTransient<IStatusReader, StatusReader>();
		}

		public static void AddServiceAgentServices(this IServiceCollection services)
		{
			services.AddTransient<RuntimeOutgoingRequestLogger>();
		}

		public static void AddDataAccessServices(this IServiceCollection services)
		{
		}
	}
}
