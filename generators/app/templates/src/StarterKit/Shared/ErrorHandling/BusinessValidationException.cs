using System;
using System.Collections.Generic;

namespace Toolbox.Errors.Exceptions
{
    public class BusinessValidationException : Exception
    {
        public BusinessValidationException(Error error)
        {
            this.Error = error;
        }

        public BusinessValidationException(string message, params object[] args) : this(new Error())
        {
            this.Error = new Error();
            this.Error.AddMessage(String.Format(message, args));
        }

        public BusinessValidationException(IEnumerable<string> messages)
        {
            this.Error = new Error();
            this.Error.AddMessages(messages);
        }

        public Error Error { get; private set; }
    }
}
