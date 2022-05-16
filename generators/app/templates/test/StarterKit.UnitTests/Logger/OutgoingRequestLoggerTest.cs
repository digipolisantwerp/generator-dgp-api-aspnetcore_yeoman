using System;
using System.Net.Http;
using System.Threading;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Sinks.TestCorrelator;
using StarterKit.Framework.Logging.DelegatingHandler;
using StarterKit.Shared.Options.Logging;
using Xunit;

namespace StarterKit.UnitTests.Logger
{
	public class OutgoingRequestLoggerTest
	{
		[Fact]
		public void ShouldLogOutgoingRequest()
		{
			Log.Logger = new LoggerConfiguration()
				.WriteTo
				.TestCorrelator()
				.CreateLogger();

			var settings =
				Options.Create(new LogSettings
				{
					RequestLogging = new RequestLogging
					{
						IncomingEnabled = true,
						OutgoingEnabled = true
					}
				});

			var handler = new OutgoingRequestLogger(settings, nameof(DummyServiceAgent));
			handler.InnerHandler = new HttpClientHandler();
			var httpRequestMessage =
				new HttpRequestMessage(HttpMethod.Get, "https://jsonplaceholder.typicode.com/todos/1");
			var invoker = new HttpMessageInvoker(handler);

			using (TestCorrelator.CreateContext())
			{
				var result = invoker.SendAsync(httpRequestMessage, new CancellationToken()).Result;

				Assert.NotNull(result);

				var logs = TestCorrelator.GetLogEventsFromCurrentContext();

				Assert.Single(logs);
				Assert.Contains(logs,
					l => l.MessageTemplate.Text.Equals(
						nameof(DummyServiceAgent) + " outgoing API call response",
						StringComparison.CurrentCultureIgnoreCase));
			}
		}
	}

	public class DummyServiceAgent
	{
	}
}