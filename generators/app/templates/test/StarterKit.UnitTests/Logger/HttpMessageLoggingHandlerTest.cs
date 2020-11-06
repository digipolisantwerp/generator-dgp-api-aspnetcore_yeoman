using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json;
using Serilog;
using Serilog.Events;
using Serilog.Parsing;
using Serilog.Sinks.TestCorrelator;
using StarterKit.Api.Controllers;
using StarterKit.Logging;
using StarterKit.Shared.Options;
using Xunit;

namespace StarterKit.UnitTests.Logger
{
  public class HttpMessageLoggingHandlerTest
  {

    [Fact]
    public void ShouldLogOutgoingRequest()
    {
      Log.Logger = new LoggerConfiguration()
        .WriteTo
        .TestCorrelator()
        .CreateLogger();

      IOptions<AppSettings> settings =
        Options.Create(new AppSettings
      {
        RequestLogging = new RequestLogging
        {
          IncomingEnabled = true,
          OutgoingEnabled = true
        }
      });

      var handler = new HttpMessageLoggingHandler(settings);
      handler.InnerHandler = new HttpClientHandler();
      var httpRequestMessage = new HttpRequestMessage(
        HttpMethod.Get, "https://jsonplaceholder.typicode.com/todos/1");
      var invoker = new HttpMessageInvoker(handler);

      using (TestCorrelator.CreateContext())
      {
        var result = invoker.SendAsync(httpRequestMessage, new CancellationToken()).Result;

        Assert.NotNull(result);

        var logs = TestCorrelator.GetLogEventsFromCurrentContext();

        Assert.Equal(2, logs.Count());
        Assert.Contains(logs, l => l.MessageTemplate.Text == "API-call outgoing log Request");
        Assert.Contains(logs, l => l.MessageTemplate.Text == "API-call outgoing log Response");

      }

    }
  }
}
