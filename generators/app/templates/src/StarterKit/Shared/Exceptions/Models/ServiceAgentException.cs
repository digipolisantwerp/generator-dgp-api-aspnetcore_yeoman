using System;
using System.Collections.Generic;
using Digipolis.Errors.Exceptions;

namespace StarterKit.Shared.Exceptions.Models
{
	public class ServiceAgentException : BaseException
	{
		public ServiceAgentException(string message = null, string code = null, Exception exception = null,
			Dictionary<string, IEnumerable<string>> messages = null)
			: base(message, code, exception, messages)
		{
		}
	}
}