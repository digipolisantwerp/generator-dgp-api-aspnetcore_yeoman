using Digipolis.Errors.Exceptions;
using System;
using System.Collections.Generic;

namespace StarterKit.Framework.Exceptions
{
  public class KeyNotFoundException : BaseException
  {
    public KeyNotFoundException(string message = Defaults.KeyNotFoundException.Title, string code = Defaults.KeyNotFoundException.Code, Exception exception = null, Dictionary<string, IEnumerable<string>> messages = null)
            : base(message, code, exception, messages)
    { }
  }
}
