using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Digipolis.Errors;

namespace StarterKit.Shared.Exceptions
{
  // From https://github.com/digipolisantwerp/web_aspnetcore/blob/master/src/Digipolis.Web/Exceptions/ExceptionLogMessage.cs
  public class ExceptionLogMessage
  {
    public Error Error { get; set; }
    public Exception Exception { get; set; }
    public string ExceptionInfo { get; set; }
  }
}
