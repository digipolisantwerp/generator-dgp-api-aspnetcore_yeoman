using Microsoft.Extensions.DependencyInjection;

namespace StarterKit
{
	public class Factory
	{
		public static void Configure(IServiceCollection services)
		{
			ConfigureUtilitites(services);
			ConfigureServiceAgents(services);
			ConfigureDataAccess(services);
			ConfigureBusiness(services);
		}

		private static void ConfigureBusiness(IServiceCollection services)
		{
			
		}

		private static void ConfigureDataAccess(IServiceCollection services)
		{

        }

		private static void ConfigureServiceAgents(IServiceCollection services)
		{

        }

		private static void ConfigureUtilitites(IServiceCollection services)
		{

		}
	}
}