using System;
using System.Collections.Generic;

namespace StarterKit.Shared.Exceptions.Models
{
	public class InvalidConfigurationException : Digipolis.Errors.Exceptions.BaseException
	{
		public InvalidConfigurationException(string message = Defaults.InvalidConfigurationException.Title,
			string code = Defaults.InvalidConfigurationException.Code, Exception exception = null,
			Dictionary<string, IEnumerable<string>> messages = null)
			: base(message, code, exception, messages)
		{
		}
	}
}