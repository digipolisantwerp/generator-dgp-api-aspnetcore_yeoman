using System;
using Microsoft.Extensions.Hosting;
using StarterKit.Shared.Constants;

namespace StarterKit.Framework.Extensions
{
	public static class HostEnvironmentExtensions
	{
		/// <summary>
		/// Checks if the current hosting environment name is <see cref="RuntimeEnvironment.Local"/>.
		/// </summary>
		/// <param name="hostEnvironment">An instance of <see cref="IHostEnvironment"/>.</param>
		/// <returns>True if the environment name is <see cref="RuntimeEnvironment.Local"/>, otherwise false.</returns>
		public static bool IsLocal(this IHostEnvironment hostEnvironment)
		{
			if (hostEnvironment == null)
			{
				throw new ArgumentNullException(nameof(hostEnvironment));
			}

			return hostEnvironment.IsEnvironment(RuntimeEnvironment.Local);
		}

		/// <summary>
		/// Checks if the current hosting environment name is <see cref="RuntimeEnvironment.Acceptance"/>.
		/// </summary>
		/// <param name="hostEnvironment">An instance of <see cref="IHostEnvironment"/>.</param>
		/// <returns>True if the environment name is <see cref="RuntimeEnvironment.Acceptance"/>, otherwise false.</returns>
		public static bool IsAcceptance(this IHostEnvironment hostEnvironment)
		{
			if (hostEnvironment == null)
			{
				throw new ArgumentNullException(nameof(hostEnvironment));
			}

			return hostEnvironment.IsEnvironment(RuntimeEnvironment.Acceptance);
		}
	}
}