using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Serilog;
using Serilog.Events;
using StarterKit.Shared.Options;

namespace StarterKit.Framework.Logging
{
  public class RequestResponseLoggingMiddleware
  {
    private readonly RequestDelegate _next;
    private readonly AppSettings _appSettings;

    public RequestResponseLoggingMiddleware(RequestDelegate next, IOptions<AppSettings> appSettings)
    {
      _next = next ??
              throw new ArgumentNullException(
                $"{nameof(RequestResponseLoggingMiddleware)}.Ctr parameter {nameof(next)} cannot be null.");
      _appSettings = appSettings.Value ?? throw new ArgumentNullException(
        $"{nameof(RequestResponseLoggingMiddleware)}.Ctr parameter {nameof(appSettings)} cannot be null.");
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
      //First, get the incoming request
      await LogRequest(context.Request);

      //Copy a pointer to the original response body stream
      var originalBody = context.Response.Body;
      try
      {
        await using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        await _next(context);
        responseBody.Position = 0;
        await LogResponse(context.Response, context.Request, sw);
        responseBody.Position = 0;
        await responseBody.CopyToAsync(originalBody);
      }
      finally
      {
        //Copy the contents of the new memory stream (which contains the response) to the original stream, which is then returned to the client.
        context.Response.Body = originalBody;
      }
    }

    private async Task LogRequest(HttpRequest request)
    {
      // if debug is enabled we need to log the payload
      if (_appSettings.RequestLogging.LogPayload)
      {
        string bodyAsText;
        if (request.ContentType != null
            && request.ContentType.ToLower().Contains("multipart/form-data"))
        {
          bodyAsText = "Logging of multipart content body disabled";
        }
        else
        {
          var body = request.Body;

          request.EnableBuffering();
          var buffer = new byte[Convert.ToInt32(request.ContentLength)];
          await request.Body.ReadAsync(buffer, 0, buffer.Length);
          bodyAsText = Encoding.UTF8.GetString(buffer);
          request.Body = body;
        }

        Log.ForContext("Method", request.Method)
          .ForContext("Host", request.Host)
          .ForContext("Path", request.Path)
          .ForContext("Headers", JsonConvert.SerializeObject(request.Headers))
          .ForContext("Payload", bodyAsText)
          .ForContext("Protocol", request.Protocol)
          .Information("API-call incoming log Request");
      }
      else
      {
        Log.ForContext("Method", request.Method)
          .ForContext("Host", request.Host)
          .ForContext("Path", request.Path)
          .ForContext("Headers", JsonConvert.SerializeObject(request.Headers))
          .ForContext("Protocol", request.Protocol)
          .Information("API-call incoming log Request");
      }
    }

    private async Task LogResponse(HttpResponse response, HttpRequest request, Stopwatch sw)
    {
      sw.Stop();
      // if debug is enabled or if status code is 4xx we need to log the payload
      if (_appSettings.RequestLogging.LogPayload
          || (response.StatusCode.ToString().StartsWith("4") && _appSettings.RequestLogging.LogPayloadOnError))
      {
        string bodyAsText;
        if (response.ContentType != null
            && response.ContentType.ToLower().Contains("multipart/form-data"))
        {
          bodyAsText = "Logging of multipart content body disabled";
        }
        else
        {
          response.Body.Seek(0, SeekOrigin.Begin);
          bodyAsText = await new StreamReader(response.Body).ReadToEndAsync();
          response.Body.Seek(0, SeekOrigin.Begin);
        }

        Log.ForContext("Method", request.Method)
          .ForContext("Host", request.Host)
          .ForContext("Path", request.Path)
          .ForContext("Headers", JsonConvert.SerializeObject(response.Headers))
          .ForContext("Payload", bodyAsText)
          .ForContext("Protocol", response.HttpContext.Request.Protocol)
          .ForContext("Status", response.StatusCode)
          .ForContext("Duration", sw.ElapsedMilliseconds)
          .Information("API-call incoming log Response");
      }
      else
      {
        Log.ForContext("Method", request.Method)
          .ForContext("Host", request.Host)
          .ForContext("Path", request.Path)
          .ForContext("Headers", JsonConvert.SerializeObject(response.Headers))
          .ForContext("Protocol", response.HttpContext.Request.Protocol)
          .ForContext("Status", response.StatusCode)
          .ForContext("Duration", sw.ElapsedMilliseconds)
          .Information("API-call incoming log Response");
      }
    }
  }
}
