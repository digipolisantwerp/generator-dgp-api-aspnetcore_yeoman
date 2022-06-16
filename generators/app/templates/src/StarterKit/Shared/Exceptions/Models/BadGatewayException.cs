using System;
using System.Collections.Generic;

namespace StarterKit.Shared.Exceptions.Models
{
	public class BadGatewayException : Digipolis.Errors.Exceptions.BaseException
	{
		public BadGatewayException(string message = Defaults.BadGatewayException.Title,
			string code = Defaults.BadGatewayException.Code, Exception ex = null,
			Dictionary<string, IEnumerable<string>> messages = null)
			: base(message, code, ex, messages)
		{
		}
	}
}