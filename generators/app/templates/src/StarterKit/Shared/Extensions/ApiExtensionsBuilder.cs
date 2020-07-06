using Digipolis.Errors;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using StarterKit.Shared.Exceptions;
using StarterKit.Shared.Exceptions.Handler;
using StarterKit.Shared.Options;

namespace StarterKit.Shared.Extensions
{
  // Pulled out of Digipolis.Web package untill updated to .net core 3.1
  // https://github.com/digipolisantwerp/web_aspnetcore/blob/master/src/Digipolis.Web/Startup/ApiExtensionsBuilder.cs

  public static class ApiExtensionsBuilder
  {
    public static IServiceCollection AddGlobalErrorHandling<TExceptionMapper>(this IServiceCollection services) where TExceptionMapper : ExceptionMapper
    {
      services.TryAddSingleton<IExceptionMapper, TExceptionMapper>();
      services.TryAddSingleton<IExceptionHandler, ExceptionHandler>();

      return services;
    }

    public static void UseApiExtensions(this IApplicationBuilder app)
    {
      var settings = app.ApplicationServices.GetService<IOptions<AppSettings>>();

      var httpContextAccessor = app.ApplicationServices.GetService<IActionContextAccessor>();

      if (settings?.Value?.DisableGlobalErrorHandling == false)
      {
        app.UseExceptionHandler(new ExceptionHandlerOptions
        {
          ExceptionHandler = new ExceptionResponseHandler().Invoke
        });
      };

      //PageOptionsExtensions.Configure(httpContextAccessor);

      app.UseForwardedHeaders(new ForwardedHeadersOptions()
      {
        RequireHeaderSymmetry = true,
        ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedHost | Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto
      });
    }
  }
}
