using System;
using System.Net;
using AutoMapper;
using Digipolis.Correlation;
using Digipolis.Errors;
using Digipolis.Errors.Exceptions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StarterKit.Framework.Extensions;

namespace StarterKit.Startup
{
  public class ApiExceptionMapper : ExceptionMapper
  {
    public ApiExceptionMapper(ILogger<ApiExceptionMapper> logger, IWebHostEnvironment environment, ICorrelationService correlationService) : base()
    {
      Logger = logger ?? throw new ArgumentException($"{GetType().Name}.Ctr parameter {nameof(logger)} cannot be null.");
      _environment = environment ?? throw new ArgumentException($"{GetType().Name}.Ctr parameter {nameof(environment)} cannot be null.");
      _correlationService = correlationService ?? throw new ArgumentException($"{GetType().Name}.Ctr parameter {nameof(environment)} cannot be null.");
    }

    protected ILogger<ApiExceptionMapper> Logger { get; private set; }
    private readonly IWebHostEnvironment _environment;
    private readonly ICorrelationService _correlationService;

    protected override void Configure()
    {
      CreateMap<UnauthorizedAccessException>((error, exception) =>
      {
        CreateUnauthorizedMap(error, new UnauthorizedException(exception: exception));
      });
      CreateMap<System.ComponentModel.DataAnnotations.ValidationException>((error, exception) =>
      {
        CreateValidationMap(error, new ValidationException(exception: exception));
      });
      CreateMap<ForbiddenException>((error, exception) =>
      {
        error.Status = (int)HttpStatusCode.Forbidden;
        error.Title = "Forbidden";
        error.Identifier = GetIdentifier();
        error.ExtraInfo = exception.Messages;
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
          else if (exception.InnerException is ForbiddenException)
            CreateMap<ForbiddenException>((e, ex) =>
            {
              error.Status = (int)HttpStatusCode.Forbidden;
              error.Title = "Forbidden";
              error.Identifier = GetIdentifier();
              error.ExtraInfo = ((ForbiddenException)exception.InnerException).Messages;
            });
          else
            CreateDefaultMap(error, exception.InnerException);
        }
      });
    }

    protected override void CreateNotFoundMap(Error error, NotFoundException exception)
    {
      base.CreateNotFoundMap(error, exception);
      error.Identifier = GetIdentifier();
      Logger.LogWarning($"Not found: {exception.Message}", exception);
    }

    protected override void CreateUnauthorizedMap(Error error, UnauthorizedException exception)
    {
      base.CreateUnauthorizedMap(error, exception);
      error.Identifier = GetIdentifier();
      Logger.LogWarning($"Access denied: {exception.Message}", exception);
    }

    protected override void CreateDefaultMap(Error error, Exception exception)
    {
      error.Identifier = GetIdentifier();
      if (_environment.IsDevelopment())
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

      Logger.LogError("Internal server error: {exceptionMessage}", exception.Message, exception);
    }

    protected override void CreateValidationMap(Error error, ValidationException exception)
    {
      error.ExtraInfo = exception.Messages;
      base.CreateValidationMap(error, exception);

      error.Identifier = GetIdentifier();
      error.Title = exception.Message;
      Logger.LogWarning($"Validation error: {exception.GetExceptionMessages()}, {exception}");
    }

    private void AddInnerExceptions(Error error, Exception exception, int level = 0)
    {
      if (exception.InnerException == null) return;

      error.ExtraInfo[$"InnerException{level:00}"] = new[] { $"{exception.InnerException.GetType().Name}: {exception.InnerException.Message}" };
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
  }
}
