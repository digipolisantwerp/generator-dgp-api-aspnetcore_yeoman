using Digipolis.Errors;
using Digipolis.Errors.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using StarterKit.Shared.Exceptions.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace StarterKit.ServiceAgents._base.Handlers
{
  public class ServiceAgentErrorHandler<T> : DelegatingHandler
    {
        private readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Include,
            DefaultValueHandling = DefaultValueHandling.Include
        };

        public ServiceAgentErrorHandler()
        {
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return SendAsync(request, cancellationToken);
        }

        protected async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken, bool parseError = true)
        {
            HttpResponseMessage response;

            response = await base.SendAsync(request, cancellationToken);

            if (parseError && (response?.StatusCode == null || !response.IsSuccessStatusCode))
                await ParseJsonError(response);

            return response;
        }

        private async Task ParseJsonError(HttpResponseMessage response)
        {
            var responseBody = await response.Content.ReadAsStringAsync();
            Error errorResponse = null;

            try
            {
                // If there is a response
                if (responseBody.Length > 0)
                {
                    // Try to get Error object in default format from JSON
                    errorResponse = JsonConvert.DeserializeObject<Error>(responseBody, _jsonSerializerSettings);

                    if (errorResponse == null || string.IsNullOrWhiteSpace(errorResponse.Title) && errorResponse.Status == 0)
                    {
                        // Json couldn't be parsed -> create new error object with custom json
                        throw new Exception();
                    }
                }
            }
            catch (Exception)
            {
                errorResponse = new Error
                {
                    Title = $"Service agent exception (parsing to Digipolis Error format failed) - {responseBody}",
                    Status = (int)response.StatusCode,
                    ExtraInfo = new Dictionary<string, IEnumerable<string>> { { "ResponseBody", new List<string> { responseBody } } }
                };
            }

            // Throw proper exception based on HTTP status
            var errorTitle = errorResponse?.Title;
            var errorCode = errorResponse?.Code;
            var extraParameters = errorResponse?.ExtraInfo ?? new Dictionary<string, IEnumerable<string>>();

            switch (response.StatusCode)
            {
                case HttpStatusCode.NotFound:
                    throw new NotFoundException(
                        errorTitle ?? "Not found",
                        errorCode ?? "NFOUND001",
                        messages: extraParameters);

                case HttpStatusCode.BadRequest:
                    throw new ValidationException(
                        errorTitle ?? "Bad request",
                        errorCode ?? "UNVALI001",
                        messages: extraParameters);

                case HttpStatusCode.Unauthorized:
                    throw new UnauthorizedException(
                        errorTitle ?? "Access denied",
                        errorCode ?? "UNAUTH001",
                        messages: extraParameters);

                case HttpStatusCode.Forbidden:
                    throw new ForbiddenException(
                        errorTitle ?? "Forbidden",
                        errorCode ?? "FORBID001",
                        messages: extraParameters);

                case HttpStatusCode.TooManyRequests:
                  throw new TooManyRequestsException(
                      errorTitle ?? "Too many requests",
                      errorCode ?? "TMREQU001",
                      messages: extraParameters);

                default:
                    throw new ServiceAgentException(
                       errorTitle,
                       errorCode ?? $"Status: {response.StatusCode}",
                       messages: extraParameters);
            }
        }
    }
}
