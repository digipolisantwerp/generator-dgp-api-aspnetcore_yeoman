using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Options;
using Serilog;
using StarterKit.Shared.Constants;
using StarterKit.Shared.Options.Logging;

namespace StarterKit.Framework.Logging.Middleware
{
	/// <summary>
	/// log incoming request via middleware
	/// </summary>
	public class IncomingRequestLogger
	{
		private readonly LogSettings _logSettings;
		private readonly RequestDelegate _next;

		public IncomingRequestLogger(RequestDelegate next, IOptions<LogSettings> logSettings)
		{
			_next = next ??
			        throw new ArgumentNullException(
				        $"{nameof(IncomingRequestLogger)}.Ctr parameter {nameof(next)} cannot be null.");
			_logSettings = logSettings.Value ??
			               throw new ArgumentNullException(
				               $"{nameof(IncomingRequestLogger)}.Ctr parameter {nameof(logSettings)} cannot be null.");
		}

		public async Task Invoke(HttpContext context)
		{
			if (NeedsToLog(context.Request.GetEncodedUrl()))
			{
				if (!_logSettings.RequestLogging.IncomingEnabled)
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

				await using var responseBody = new MemoryStream();
				context.Response.Body = responseBody;

				await _next(context);

				responseBody.Seek(0, SeekOrigin.Begin);

				await LogRequest(context.Response, sw);

				//Copy the contents of the new memory stream (which contains the response) to the original stream, which is then returned to the client
				await responseBody.CopyToAsync(originalResponseBody);
			}
			else
			{
				await _next(context);
			}
		}

		private async Task LogRequest(HttpResponse response, Stopwatch sw)
		{
			sw.Stop();

			Log
				.ForContext("Type", new[] { "application" })
				.ForContext("Request", await GetRequestLog(response.HttpContext.Request))
				.ForContext("Response", await GetResponseLog(response, sw.ElapsedMilliseconds))
				.Information("Incoming API call response");
		}

		private static bool ShouldLogHeader(string name, IEnumerable<string> allowedHeaders)
		{
			return allowedHeaders.ToList().Any(x => x.Equals(name, StringComparison.OrdinalIgnoreCase));
		}

		private Dictionary<string, string> GetHeaders(IHeaderDictionary headers, IEnumerable<string> allowedHeaders)
		{
			var result = new Dictionary<string, string>();

			var allowedHeadersList = allowedHeaders?.ToList();
			if (allowedHeadersList == null || !allowedHeadersList.Any())
			{
				return result;
			}

			headers
				.Where(x => ShouldLogHeader(x.Key, allowedHeadersList))
				.ToList()
				.ForEach(x => result.Add(x.Key, x.Value));

			return result;
		}

		private bool ShouldLogRequestPayload()
		{
			return _logSettings.RequestLogging.LogPayload;
		}

		private static async Task<string> GetRequestBody(HttpRequest request)
		{
			var contentType = request.ContentType ?? string.Empty;

			if (contentType.ToLower().Contains(MediaType.MultipartFormData))
			{
				return "Logging of multipart content body disabled";
			}

			request.EnableBuffering();
			request.Body.Seek(0, SeekOrigin.Begin);

			var text = await new StreamReader(request.Body).ReadToEndAsync();

			request.Body.Seek(0, SeekOrigin.Begin);
			return text;
		}

		private async Task<Dictionary<string, object>> GetRequestLog(HttpRequest request)
		{
			var requestLog = new Dictionary<string, object>
			{
				{ "method", request.Method },
				{ "host", request.Host.ToString() },
				{ "path", $"{request.Path}{request.QueryString}" },
				{ "protocol", request.Scheme },
				{ "headers", GetHeaders(request.Headers, _logSettings.RequestLogging.AllowedIncomingRequestHeaders) }
			};

			if (ShouldLogRequestPayload())
			{
				requestLog.Add("payload", await GetRequestBody(request));
			}

			return requestLog;
		}

		private bool ShouldLogResponsePayload(HttpResponse response)
		{
			return _logSettings.RequestLogging.LogPayload
			       || (_logSettings.RequestLogging.LogPayloadOnError && response.StatusCode >= 400 &&
			           response.StatusCode < 500);
		}

		private static async Task<string> GetResponseBody(HttpResponse response)
		{
			var text = await new StreamReader(response.Body).ReadToEndAsync();

			response.Body.Seek(0, SeekOrigin.Begin);

			return $"{response.StatusCode}: {text}";
		}

		private async Task<Dictionary<string, object>> GetResponseLog(HttpResponse response, long durationInMs)
		{
			var responseLog = new Dictionary<string, object>
			{
				{ "headers", GetHeaders(response.Headers, _logSettings.RequestLogging.AllowedIncomingResponseHeaders) },
				{ "status", response.StatusCode },
				{ "duration", durationInMs }
			};

			if (ShouldLogResponsePayload(response))
			{
				response.Body.Seek(0, SeekOrigin.Begin);
				responseLog.Add("payload", await GetResponseBody(response));
			}

			return responseLog;
		}

		private static bool NeedsToLog(string url)
		{
			return !url.Contains("/status/") &&
			       !url.Contains("/swagger/");
		}
	}
}
