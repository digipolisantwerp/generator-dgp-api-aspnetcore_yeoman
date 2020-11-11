using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Events;
using StarterKit.Shared.Options;

namespace StarterKit.Framework.Logging
{
  public class HttpMessageLoggingHandler : DelegatingHandler
  {
    private readonly AppSettings _appSettings;

    public HttpMessageLoggingHandler(IOptions<AppSettings> appSettings)
    {
      _appSettings = appSettings.Value ??
                     throw new ArgumentNullException(
                       $"{nameof(HttpMessageLoggingHandler)}.Ctr parameter {nameof(appSettings)} cannot be null.");
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
      CancellationToken cancellationToken)
    {
      if (!_appSettings.RequestLogging.OutgoingEnabled)
      {
        return await base.SendAsync(request, cancellationToken);
      }

      var sw = new Stopwatch();
      sw.Start();
      if (request.Content != null && Log.IsEnabled(LogEventLevel.Debug))
      {
        string bodyAsText;
        if (request.Content.Headers?.ContentType != null
            && request.Content.Headers.ContentType.MediaType.ToLower().Contains("multipart/form-data"))
        {
          bodyAsText = "Logging of multipart content body disabled";
        }
        else
        {
          bodyAsText = await request.Content.ReadAsStringAsync();
        }

        Log.ForContext("Host", request.RequestUri.Host)
          .ForContext("Headers", request.Headers)
          .ForContext("Path", request.RequestUri.AbsolutePath)
          .ForContext("Payload", bodyAsText)
          .ForContext("Protocol", request.RequestUri.Scheme)
          .ForContext("Method", request.Method)
          .Information("API-call outgoing log Request");
      }
      else
      {
        Log.ForContext("Host", request.RequestUri.Host)
          .ForContext("Headers", request.Headers)
          .ForContext("Path", request.RequestUri.AbsolutePath)
          .ForContext("Protocol", request.RequestUri.Scheme)
          .ForContext("Method", request.Method)
          .Information("API-call outgoing log Request");
      }

      HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

      sw.Stop();
      if (response.Content != null &&
          (Log.IsEnabled(LogEventLevel.Debug) || response.StatusCode.ToString().StartsWith("4")))
      {
        string bodyAsText;
        if (response.Content.Headers?.ContentType != null
            && response.Content.Headers.ContentType.MediaType.ToLower().Contains("multipart/form-data"))
        {
          bodyAsText = "Logging of multipart content body disabled";
        }
        else
        {
          bodyAsText = await response.Content.ReadAsStringAsync();
        }

        Log.ForContext("Headers", response.Headers)
          .ForContext("Payload", bodyAsText)
          .ForContext("Protocol", response.RequestMessage.RequestUri.Scheme)
          .ForContext("Status", response.StatusCode)
          .ForContext("Duration", sw.ElapsedMilliseconds)
          .Information("API-call outgoing log Response");
      }
      else
      {
        Log.ForContext("Headers", response.Headers)
          .ForContext("Protocol", response.RequestMessage.RequestUri.Scheme)
          .ForContext("Status", response.StatusCode)
          .ForContext("Duration", sw.ElapsedMilliseconds)
          .Information("API-call outgoing log Response");
      }


      return response;
    }
  }
}
