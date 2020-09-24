using System;
using System.Collections.Generic;

namespace StarterKit.Shared.Exceptions.Models
{
    public class InvalidArgumentException : Digipolis.Errors.Exceptions.BaseException
    {
        public InvalidArgumentException(string message = Defaults.InvalidationArgumentException.Title, string code =Defaults.InvalidationArgumentException.Code, Exception exception = null, Dictionary < string, IEnumerable<string>> messages = null )
            : base(message,code, exception, messages)
        {}
    }
}
