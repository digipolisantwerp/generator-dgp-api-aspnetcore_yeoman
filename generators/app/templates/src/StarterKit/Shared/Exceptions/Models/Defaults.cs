namespace StarterKit.Shared.Exceptions.Models
{
  internal class Defaults
  {
    internal class ErrorMessage
    {
      internal const string Key = "";
      internal const string NullString = "null";
    }

    internal class BaseException
    {
    }

    internal class ForbiddenException
    {
      internal const string Title = "Forbidden.";
      internal const string Code = "FORBID001";
    }

    internal class InvalidationArgumentException
    {
      internal const string Title = "Invalid argument.";
      internal const string Code = "INVARG001";
    }

    internal class InvalidConfigurationException
    {
      internal const string Title = "Invalid Configuration.";
      internal const string Code = "INCONF001";
    }

    internal class KeyNotFoundException
    {
      internal const string Title = "Key not found.";
      internal const string Code = "KEYNFOUND001";
    }

    internal class NotFoundException
    {
      internal const string Title = "Not found.";
      internal const string Code = "NFOUND001";
    }

    internal class UnauthorizedException
    {
      internal const string Title = "Access denied.";
      internal const string Code = "UNAUTH001";
    }

    internal class ValidationException
    {
      internal const string Title = "Bad request.";
      internal const string Code = "UNVALI001";
    }

    internal class BadGatewayException
    {
      internal const string Title = "Bad Gateway.";
      internal const string Code = "GTWAY001";
    }
    internal class GatewayTimeoutException
    {
      internal const string Title = "Gateway Timeout.";
      internal const string Code = "GTWAY002";
    }
  }
}
