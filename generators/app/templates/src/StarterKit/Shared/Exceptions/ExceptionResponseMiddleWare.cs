using System;
using System.Net;
using System.Threading.Tasks;
using Digipolis.Errors.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using StarterKit.Shared.Exceptions.Handler;
using StarterKit.Shared.Options;

namespace StarterKit.Shared.Exceptions
{
	public class ExceptionResponseMiddleWare
	{
		private readonly RequestDelegate _next;

		public ExceptionResponseMiddleWare(RequestDelegate next)
		{
			_next = next ?? throw new ArgumentNullException(nameof(next), $"{nameof(next)} cannot be null.");
		}

		public async Task Invoke(HttpContext context)
		{
			var handler = context.RequestServices.GetService<IExceptionHandler>();
			var options = context.RequestServices.GetService<IOptions<AppSettings>>();

			if (handler == null || options?.Value?.DisableGlobalErrorHandling == true)
			{
				await _next.Invoke(context);
				return;
			}

			try
			{
				await _next.Invoke(context);
				switch (context.Response.StatusCode)
				{
					case (int)HttpStatusCode.Unauthorized:
						await handler.HandleAsync(context, new UnauthorizedAccessException());
						break;
					case (int)HttpStatusCode.NotFound:
						await handler.HandleAsync(context, new NotFoundException());
						break;
				}
			}
			catch (Exception ex)
			{
				await handler.HandleAsync(context, ex);
			}
		}
	}
}