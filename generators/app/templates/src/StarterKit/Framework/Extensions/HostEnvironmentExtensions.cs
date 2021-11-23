using StarterKit.Shared.Constants;
using Microsoft.Extensions.Hosting;
using System;

namespace StarterKit.Framework.Extensions
{
  public static class HostEnvironmentExtensions
  {
    /// <summary>
    /// Checks if the current hosting environment name is <see cref="RuntimeEnvironments.Local"/>.
    /// </summary>
    /// <param name="hostEnvironment">An instance of <see cref="IHostEnvironment"/>.</param>
    /// <returns>True if the environment name is <see cref="RuntimeEnvironments.Local"/>, otherwise false.</returns>
    public static bool IsLocal(this IHostEnvironment hostEnvironment)
    {
      if (hostEnvironment == null)
      {
        throw new ArgumentNullException(nameof(hostEnvironment));
      }

      return hostEnvironment.IsEnvironment(RuntimeEnvironments.Local);
    }

    /// <summary>
    /// Checks if the current hosting environment name is <see cref="RuntimeEnvironments.Acceptance"/>.
    /// </summary>
    /// <param name="hostEnvironment">An instance of <see cref="IHostEnvironment"/>.</param>
    /// <returns>True if the environment name is <see cref="RuntimeEnvironments.Acceptance"/>, otherwise false.</returns>
    public static bool IsAcceptance(this IHostEnvironment hostEnvironment)
    {
      if (hostEnvironment == null)
      {
        throw new ArgumentNullException(nameof(hostEnvironment));
      }

      return hostEnvironment.IsEnvironment(RuntimeEnvironments.Acceptance);
    }
  }
}
