using System;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using Toolbox.Common.Helpers;
using Toolbox.Errors;
using Toolbox.Errors.Exceptions;
using Toolbox.WebApi.QueryString;

namespace StarterKit.Api.Controllers
{
	[EnableQueryStringMapping()]
	public abstract class ControllerBase : Controller
	{
		protected ControllerBase(ILogger logger)
		{
			this.Logger = logger;
		}

		protected ILogger Logger { get; private set; }

		protected virtual IActionResult BadRequestResult(ModelStateDictionary modelState)
		{
			// ToDo : log this error
			return new BadRequestObjectResult(modelState);
		}

		protected virtual IActionResult BadRequestResult(ValidationException validationEx)
		{
			// ToDo : log this error
			var modelState = new ModelStateDictionary();
			foreach ( var message in validationEx.Error.Messages )
			{
				modelState.AddModelError(message.Key, message.Message);
			}
			return new BadRequestObjectResult(modelState);
		}

		protected virtual IActionResult CreatedAtRouteResult(string routeName, object routeValues, object value)
		{
			return new CreatedAtRouteResult(routeName, routeValues, value);
		}

		protected virtual IActionResult InternalServerError(Exception ex, string message, params object[] args)
		{
			if ( ex == null )
			{
				Logger.LogError(message, args);
                var error = new Error();
                error.AddMessage(String.Format(message, args));
                return new ObjectResult(error) { StatusCode = 500 };
			}
			else
			{
				var errorMessage = String.Format(message, args);
				Logger.LogError("{0} : {1}", errorMessage, ExceptionHelper.GetAllToStrings(ex));
				var error = new Error();
                error.AddMessage(String.Format("{0} : {1}", errorMessage, ex.Message));
				return new ObjectResult(error) { StatusCode = 500 };
			}
		}

		protected virtual IActionResult InternalServerError(Error error)
		{
			if ( error == null ) throw new ArgumentNullException(nameof(error), nameof(error) + " is null");
			foreach ( var message in error.Messages )
				Logger.LogError("{0} : {1}", message.Key, message.Message);
			return new ObjectResult(error) { StatusCode = 500 };
		}

		protected virtual IActionResult OkResult()
		{
			return new HttpStatusCodeResult(200);
		}

		protected virtual IActionResult OkResult(object value)
		{
			return new ObjectResult(value) { StatusCode = 200 };
		}

		protected virtual IActionResult NotFoundResult(string message, params object[] args)
		{
			Logger.LogWarning(message, args);
            var error = new Error();
            error.AddMessage(String.Format(message, args));
			return new ObjectResult(error) { StatusCode = 404 };
		}
	}
}
