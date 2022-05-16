using System;
using System.Linq;
using Digipolis.Errors.Exceptions;

namespace StarterKit.Framework.Extensions
{
	public static class ExceptionExtensions
	{
		public static string GetExceptionMessages(this Exception ex, string message = "")
		{
			if (ex == null) return String.Empty;
			if (String.IsNullOrWhiteSpace(message)) message = ex.Message;
			if (ex.InnerException != null)
				message += "\r\nInnerException: " + GetExceptionMessages(ex.InnerException);
			return message;
		}

		public static string GetExceptionMessages(this ValidationException ex, string message = "")
		{
			if (ex == null) return String.Empty;
			if (String.IsNullOrWhiteSpace(message)) message = ex.Message;
			if (ex.Messages != null && ex.Messages.Any())
			{
				message += " Details : " + String.Join(", ", ex.Messages.SelectMany(x => x.Value));
			}

			return message;
		}
	}
}