using System;
using System.Net;
using AutoMapper;
using Digipolis.Correlation;
using Digipolis.Errors;
using Digipolis.Errors.Exceptions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StarterKit.Framework.Extensions;

namespace StarterKit.Startup
{
	public class ApiExceptionMapper : ExceptionMapper
	{
		private readonly Shared.Options.AppSettings _appSettings;
		private readonly ICorrelationService _correlationService;
		private readonly IHostEnvironment _environment;

		private readonly ILogger<ApiExceptionMapper> _logger;

		public ApiExceptionMapper(ILogger<ApiExceptionMapper> logger,
			IHostEnvironment environment,
			IOptions<Shared.Options.AppSettings> appSettings,
			ICorrelationService correlationService)
		{
			_logger = logger ??
			          throw new ArgumentException($"{GetType().Name}.Ctr parameter {nameof(logger)} cannot be null.");
			_environment = environment ??
			               throw new ArgumentException(
				               $"{GetType().Name}.Ctr parameter {nameof(environment)} cannot be null.");
			_appSettings = appSettings?.Value ??
			               throw new ArgumentException(
				               $"{GetType().Name}.Ctr parameter {nameof(appSettings)} cannot be null.");
			_correlationService = correlationService ??
			                      throw new ArgumentException(
				                      $"{GetType().Name}.Ctr parameter {nameof(environment)} cannot be null.");
		}

		protected override void Configure()
		{
			// map UnauthorizedAccessException to Digipolis.Errors.Exceptions.UnauthorizedException
			CreateMap<UnauthorizedAccessException>((error, exception) =>
			{
				CreateUnauthorizedMap(error, new UnauthorizedException(exception: exception));
			});

			// map System.ComponentModel.DataAnnotations.ValidationException to Digipolis.Errors.Exceptions.ValidationException
			CreateMap<System.ComponentModel.DataAnnotations.ValidationException>((error, exception) =>
			{
				CreateValidationMap(error, new ValidationException(exception: exception));
			});

			CreateMap<AutoMapperMappingException>((error, exception) =>
			{
				if (exception.InnerException?.Message == null)
					CreateDefaultMap(error, exception);
				else
				{
					if (exception.InnerException is System.ComponentModel.DataAnnotations.ValidationException
					    || exception.InnerException is ValidationException)
						CreateValidationMap(error, new ValidationException(exception: exception.InnerException));
					else if (exception.InnerException is NotFoundException notFoundException)
						CreateNotFoundMap(error, notFoundException);
					else if (exception.InnerException is UnauthorizedException innerException)
						CreateUnauthorizedMap(error, innerException);
					else if (exception.InnerException is ForbiddenException forbiddenException)
						CreateForbiddenMap(error, forbiddenException);
					else
						CreateDefaultMap(error, exception.InnerException);
				}
			});
		}

		protected override void CreateNotFoundMap(Error error, NotFoundException exception)
		{
			base.CreateNotFoundMap(error, exception);
			error.Identifier = GetIdentifier();
			SetErrorTypeReferenceUri(error);

			_logger.LogWarning($"Not found: {exception.Message}", exception);
		}

		protected override void CreateUnauthorizedMap(Error error, UnauthorizedException exception)
		{
			base.CreateUnauthorizedMap(error, exception);
			error.Identifier = GetIdentifier();
			SetErrorTypeReferenceUri(error);

			_logger.LogWarning($"Access denied: {exception.Message}", exception);
		}

		protected override void CreateForbiddenMap(Error error, ForbiddenException exception)
		{
			base.CreateForbiddenMap(error, exception);

			error.Identifier = GetIdentifier();
			SetErrorTypeReferenceUri(error);

			_logger.LogWarning($"Forbidden: {exception.Message}", exception);
		}

		protected override void CreateTooManyRequestsMap(Error error, TooManyRequestsException exception)
		{
			base.CreateTooManyRequestsMap(error, exception);

			error.Identifier = GetIdentifier();
			SetErrorTypeReferenceUri(error);

			_logger.LogWarning($"Too many requests: {exception.Message}", exception);
		}

		protected override void CreateDefaultMap(Error error, Exception exception)
		{
			error.Code = "SRVRER001";
			error.Identifier = GetIdentifier();
			SetErrorTypeReferenceUri(error);

			if (_environment.IsDevelopment() || _environment.IsLocal())
			{
				error.Status = (int)HttpStatusCode.InternalServerError;
				error.Title = $"{exception.GetType().Name}: {exception.Message}";
				AddInnerExceptions(error, exception);
			}
			else
			{
				error.Status = (int)HttpStatusCode.InternalServerError;
				error.Title = "We are experiencing some technical difficulties.";
			}

			_logger.LogError("Internal server error: {exceptionMessage}: {exception}", exception.Message, exception);
		}

		protected override void CreateValidationMap(Error error, ValidationException exception)
		{
			base.CreateValidationMap(error, exception);

			error.Identifier = GetIdentifier();
			error.Title = exception.Message;
			SetErrorTypeReferenceUri(error);

			_logger.LogWarning($"Validation error: {exception.GetExceptionMessages()}, {exception}");
		}

		private void AddInnerExceptions(Error error, Exception exception, int level = 0)
		{
			if (exception.InnerException == null) return;

			error.ExtraInfo[$"InnerException{level:00}"] = new[]
				{ $"{exception.InnerException.GetType().Name}: {exception.InnerException.Message}" };
			AddInnerExceptions(error, exception.InnerException, level++);
		}

		private Guid GetIdentifier()
		{
			if (!Guid.TryParse(_correlationService.GetContext().Id, out var identifier))
			{
				identifier = Guid.NewGuid();
			}

			return identifier;
		}

		private void SetErrorTypeReferenceUri(Error error)
		{
			if (!string.IsNullOrWhiteSpace(_appSettings.ErrorReferenceUri))
			{
				error.Type = new Uri(_appSettings.ErrorReferenceUri);
			}
		}
	}
}