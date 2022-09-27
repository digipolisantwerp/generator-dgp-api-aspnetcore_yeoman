using System;
using Digipolis.Errors;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using StarterKit.Shared.Exceptions;
using StarterKit.Shared.Exceptions.Handler;
using StarterKit.Shared.Options;

namespace StarterKit.Shared.Extensions
{
	public static class ApiExtensionsBuilder
	{
		public static void AddGlobalErrorHandling<TExceptionMapper>(this IServiceCollection services)
			where TExceptionMapper : ExceptionMapper
		{
			services.TryAddSingleton<IExceptionMapper, TExceptionMapper>();
			services.TryAddSingleton<IExceptionHandler, ExceptionHandler>();
		}

		public static void UseApiExtensions(this IApplicationBuilder app)
		{
			var settings = app.ApplicationServices.GetService<IOptions<AppSettings>>();

			if (settings?.Value?.DisableGlobalErrorHandling == false)
			{
				app.UseMiddleware<ExceptionResponseMiddleWare>();
			}

			;

			//PageOptionsExtensions.Configure(httpContextAccessor);

			app.UseForwardedHeaders(new ForwardedHeadersOptions
			{
				RequireHeaderSymmetry = true,
				ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedHost |
				                   Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto
			});

			app.Use(async (context, next) =>
			{
				// This line reads the X-Forwarded-Proto header sent by NgInx
				// The first request is sent over HTTP/1 protocol and therefore it is needed to convert the scheme to HTTPS
				var xForwardedProto = context.Request.Headers["X-Forwarded-Proto"].ToString();
				if (xForwardedProto != null && xForwardedProto.StartsWith(Uri.UriSchemeHttps, StringComparison.InvariantCultureIgnoreCase))
				{
					context.Request.Scheme = Uri.UriSchemeHttps;
				}
				await next();
			});
		}
	}
}
