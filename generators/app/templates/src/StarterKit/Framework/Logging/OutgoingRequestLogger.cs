using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Serilog;
using StarterKit.Shared.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace StarterKit.Framework.Logging
{
  public class OutgoingRequestLogger : DelegatingHandler
  {
    private readonly AppSettings _appSettings;

    public OutgoingRequestLogger(IOptions<AppSettings> appSettings)
    {
      _appSettings = appSettings.Value ?? throw new ArgumentNullException($"{nameof(OutgoingRequestLogger)}.Ctr parameter {nameof(appSettings)} cannot be null.");
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
      if (!_appSettings.RequestLogging.OutgoingEnabled)
      {
        return await base.SendAsync(request, cancellationToken);
      }

      var sw = new Stopwatch();
      sw.Start();

      HttpResponseMessage response = await base.SendAsync(request, cancellationToken);
      await LogRequest(response, sw);

      return response;
    }

    private bool ShouldLogHeader(string name, IEnumerable<string> allowedHeaders)
    {
      return allowedHeaders.ToList().Any(x => x.Equals(name, StringComparison.OrdinalIgnoreCase));
    }

    private Dictionary<string, string> GetHeaders(HttpHeaders headers, IEnumerable<string> allowedHeaders)
    {
      var result = new Dictionary<string, string>();

      if (allowedHeaders == null || allowedHeaders.Count() == 0)
      {
        return result;
      }

      headers
        .Where(x => ShouldLogHeader(x.Key, allowedHeaders))
        .ToList()
        .ForEach(x => result.Add(x.Key, string.Join(",", x.Value)));

      return result;
    }

    private string GetContentType(HttpHeaders headers)
    {
      IEnumerable<string> contentTypeHeader;
      headers.TryGetValues(HeaderNames.ContentType, out contentTypeHeader);

      return contentTypeHeader?.FirstOrDefault();
    }

    private bool ShouldLogRequestPayload()
    {
      return _appSettings.RequestLogging.LogPayload;
    }

    private async Task<string> GetRequestBody(HttpRequestMessage request)
    {
      if (GetContentType(request.Headers)?.Contains("multipart/form-data") == true)
      {
        return "Logging of multipart content body disabled";
      }

      if (request.Content == null)
      {
        return null;
      }

      return await request.Content.ReadAsStringAsync();
    }

    private async Task<Dictionary<string, Object>> GetRequestLog(HttpRequestMessage request)
    {
      var requestLog = new Dictionary<string, Object>() {
        { "method", request.Method },
        { "host", request.RequestUri.Host.ToString() },
        { "path", request.RequestUri.PathAndQuery.ToString() },
        { "protocol", request.RequestUri.Scheme.ToString() },
        { "headers", GetHeaders(request.Headers, _appSettings.RequestLogging.AllowedOutgoingRequestHeaders) }
      };

      if (ShouldLogRequestPayload())
      {
        requestLog.Add("payload", await GetRequestBody(request));
      }

      return requestLog;
    }

    private bool ShouldLogResponsePayload(HttpResponseMessage response)
    {
      return _appSettings.RequestLogging.LogPayload
        || (_appSettings.RequestLogging.LogPayloadOnError && (int)response.StatusCode >= 400 && (int)response.StatusCode < 500);
    }

    private async Task<string> GetResponseBody(HttpResponseMessage response)
    {
      if (GetContentType(response.Headers)?.Contains("multipart/form-data") == true)
      {
        return "Logging of multipart content body disabled";
      }

      if (response.Content == null)
      {
        return null;
      }

      return await response.Content.ReadAsStringAsync();
    }

    private async Task<Dictionary<string, Object>> GetResponseLog(HttpResponseMessage response, long durationInMs)
    {
      var responseLog = new Dictionary<string, Object>() {
        { "headers", GetHeaders(response.Headers, _appSettings.RequestLogging.AllowedOutgoingResponseHeaders) },
        { "status", (int)response.StatusCode },
        { "duration", durationInMs }
      };

      if (ShouldLogResponsePayload(response))
      {
        responseLog.Add("payload", await GetResponseBody(response));
      }

      return responseLog;
    }

    private async Task LogRequest(HttpResponseMessage response, Stopwatch sw)
    {
      sw.Stop();

      Log
        .ForContext("Type", new string[] { "application" })
        .ForContext("Request", await GetRequestLog(response.RequestMessage))
        .ForContext("Response", await GetResponseLog(response, sw.ElapsedMilliseconds))
        .Information("Outgoing API call response");
    }
  }
}
