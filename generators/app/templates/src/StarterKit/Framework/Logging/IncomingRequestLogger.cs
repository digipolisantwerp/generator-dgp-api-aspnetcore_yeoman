using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Serilog;
using StarterKit.Shared.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarterKit.Framework.Logging
{
  public class IncomingRequestLogger
  {
    private readonly RequestDelegate _next;
    private readonly AppSettings _appSettings;

    public IncomingRequestLogger(RequestDelegate next, IOptions<AppSettings> appSettings)
    {
      _next = next ?? throw new ArgumentNullException($"{nameof(IncomingRequestLogger)}.Ctr parameter {nameof(next)} cannot be null.");
      _appSettings = appSettings.Value ?? throw new ArgumentNullException($"{nameof(IncomingRequestLogger)}.Ctr parameter {nameof(appSettings)} cannot be null.");
    }

    public async Task Invoke(HttpContext context)
    {
      if (!_appSettings.RequestLogging.IncomingEnabled)
      {
        await _next(context);
        return;
      }

      var sw = new Stopwatch();
      sw.Start();

      if (ShouldLogRequestPayload())
      {
        context.Request.EnableBuffering();
      }

      //Copy a pointer to the original response body stream
      var originalResponseBody = context.Response.Body;

      using (var responseBody = new MemoryStream())
      {
        context.Response.Body = responseBody;

        await _next(context);
        await LogRequest(context.Response, sw);
        responseBody.Seek(0, SeekOrigin.Begin);

        //Copy the contents of the new memory stream (which contains the response) to the original stream, which is then returned to the client
        await responseBody.CopyToAsync(originalResponseBody);
      }
    }

    private bool ShouldLogHeader(string name, IEnumerable<string> allowedHeaders)
    {
      return allowedHeaders.ToList().Any(x => x.Equals(name, StringComparison.OrdinalIgnoreCase));
    }

    private Dictionary<string, string> GetHeaders(IHeaderDictionary headers, IEnumerable<string> allowedHeaders)
    {
      var result = new Dictionary<string, string>();

      if (allowedHeaders == null || allowedHeaders.Count() == 0)
      {
        return result;
      }

      headers
        .Where(x => ShouldLogHeader(x.Key, allowedHeaders))
        .ToList()
        .ForEach(x => result.Add(x.Key, x.Value));

      return result;
    }

    private bool ShouldLogRequestPayload()
    {
      return _appSettings.RequestLogging.LogPayload;
    }

    private async Task<string> GetRequestBody(HttpRequest request)
    {
      if (request.ContentType != null
        && request.ContentType.ToLower().Contains("multipart/form-data"))
      {
        return "Logging of multipart content body disabled";
      }

      request.Body.Seek(0, SeekOrigin.Begin);
      var buffer = new byte[Convert.ToInt32(request.ContentLength)];
      await request.Body.ReadAsync(buffer, 0, buffer.Length);
      var requestBodyAsText = Encoding.UTF8.GetString(buffer);
      request.Body.Seek(0, SeekOrigin.Begin);

      return requestBodyAsText;
    }

    private async Task<Dictionary<string, Object>> GetRequestLog(HttpRequest request)
    {
      var requestLog = new Dictionary<string, Object>() {
        { "method", request.Method },
        { "host", request.Host.ToString() },
        { "path", $"{request.Path}{request.QueryString}" },
        { "protocol", request.Scheme.ToString() },
        { "headers", GetHeaders(request.Headers, _appSettings.RequestLogging.AllowedIncomingRequestHeaders) }
      };

      if (ShouldLogRequestPayload())
      {
        requestLog.Add("payload", await GetRequestBody(request));
      }

      return requestLog;
    }

    private bool ShouldLogResponsePayload(HttpResponse response)
    {
      return _appSettings.RequestLogging.LogPayload
          || (_appSettings.RequestLogging.LogPayloadOnError && response.StatusCode >= 400 && response.StatusCode < 500);
    }

    private async Task<string> GetResponseBody(HttpResponse response)
    {
      if (response.ContentType != null
        && response.ContentType.ToLower().Contains("multipart/form-data"))
      {
        return "Logging of multipart content body disabled";
      }

      response.Body.Seek(0, SeekOrigin.Begin);
      var result = await new StreamReader(response.Body).ReadToEndAsync();

      return result;
    }

    private async Task<Dictionary<string, Object>> GetResponseLog(HttpResponse response, long durationInMs)
    {
      var responseLog = new Dictionary<string, Object>() {
        { "headers", GetHeaders(response.Headers, _appSettings.RequestLogging.AllowedIncomingResponseHeaders) },
        { "status", response.StatusCode },
        { "duration", durationInMs }
      };

      if (ShouldLogResponsePayload(response))
      {
        responseLog.Add("payload", await GetResponseBody(response));
      }

      return responseLog;
    }

    private async Task LogRequest(HttpResponse response, Stopwatch sw)
    {
      sw.Stop();

      Log
        .ForContext("Type", new string[] { "application" })
        .ForContext("Request", await GetRequestLog(response.HttpContext.Request))
        .ForContext("Response", await GetResponseLog(response, sw.ElapsedMilliseconds))
        .Information("Incoming API call response");
    }
  }
}
