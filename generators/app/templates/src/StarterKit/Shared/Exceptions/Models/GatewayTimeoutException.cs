using System;
using System.Collections.Generic;

namespace StarterKit.Shared.Exceptions.Models
{
    public class GatewayTimeoutException : Digipolis.Errors.Exceptions.BaseException
    {
        public GatewayTimeoutException() : base()
        {}

        public GatewayTimeoutException(string message = Defaults.GatewayTimeoutException.Title) : base(message)
        { }

        public GatewayTimeoutException(string message = Defaults.GatewayTimeoutException.Title, string code= Defaults.GatewayTimeoutException.Code, Exception ex = null, Dictionary<string, IEnumerable<string>> messages = null)
            : base(message, code, ex, messages)
        {

        }
    }
}
