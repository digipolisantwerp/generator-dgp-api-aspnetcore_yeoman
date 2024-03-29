using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Digipolis.Errors;
using Digipolis.Errors.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using StarterKit.ServiceAgents._base.Helper;
using StarterKit.ServiceAgents._base.Settings;
using StarterKit.Shared.Exceptions.Models;
using BadGatewayException = StarterKit.Shared.Exceptions.Models.BadGatewayException;
using GatewayTimeoutException = StarterKit.Shared.Exceptions.Models.GatewayTimeoutException;

namespace StarterKit.ServiceAgents._base
{
	public abstract class ConfigInjectedAgentBase<TAgent> : AgentBase<TAgent>, IDisposable where TAgent : class
	{
		protected readonly IRequestHeaderHelper _requestHeaderHelper;
		protected readonly AgentSettingsBase Settings;

		protected ConfigInjectedAgentBase(ILogger<TAgent> logger, HttpClient httpClient, IServiceProvider serviceProvider) : base(logger, httpClient, serviceProvider)
		{
			var settings = serviceProvider.GetRequiredService<IOptions<ServiceAgentSettings>>();
			if (settings.Value == null)
				throw new ArgumentNullException(nameof(ServiceAgentSettings),
					$"{nameof(ServiceAgentSettings)} cannot be null.");

			Settings = settings.Value.GetServiceSettings(typeof(TAgent)?.Name);

			_requestHeaderHelper = serviceProvider.GetRequiredService<IRequestHeaderHelper>();	
		}

		protected new async Task<T> GetAsync<T>(string requestUri)
		{
			await _requestHeaderHelper.ValidateAuthHeaders(Client, Settings);
			using var response = await Client.GetAsync(requestUri, HttpCompletionOption.ResponseHeadersRead);
			return await ParseResult<T>(response);
		}

		protected new async Task<string> GetStringAsync(string requestUri)
		{
			await _requestHeaderHelper.ValidateAuthHeaders(Client, Settings);
			using var response = await GetResponseAsync(requestUri);
			await using var stream = await response.Content.ReadAsStreamAsync();
			return await StreamToStringAsync(stream) ?? string.Empty;
		}

		protected async Task<HttpResponseMessage> GetResponseAsync(string requestUri)
		{
			await _requestHeaderHelper.ValidateAuthHeaders(Client, Settings);
			var response = await Client.GetAsync(requestUri, HttpCompletionOption.ResponseHeadersRead);
			if (!response.IsSuccessStatusCode)
				await ParseJsonError(response); // only return responses with status code success

			return response;
		}

		protected new async Task<T> PostAsync<T>(string requestUri, T item)
		{
			await _requestHeaderHelper.ValidateAuthHeaders(Client, Settings);
			var response = await Client.PostAsync(requestUri, CreateContentFromObject(item));
			return await ParseResult<T>(response);
		}

		protected new async Task<TResponse> PostAsync<TRequest, TResponse>(string requestUri, TRequest item)
		{
			await _requestHeaderHelper.ValidateAuthHeaders(Client, Settings);
			var response = await Client.PostAsync(requestUri, CreateContentFromObject(item));
			return await ParseResult<TResponse>(response);
		}

		protected new async Task<T> PutAsync<T>(string requestUri, T item)
		{
			await _requestHeaderHelper.ValidateAuthHeaders(Client, Settings);
			var response = await Client.PutAsync(requestUri, CreateContentFromObject(item));
			return await ParseResult<T>(response);
		}

		protected new async Task<TResponse> PutAsync<TRequest, TResponse>(string requestUri, TRequest item)
		{
			await _requestHeaderHelper.ValidateAuthHeaders(Client, Settings);
			var response = await Client.PutAsync(requestUri, CreateContentFromObject(item));
			return await ParseResult<TResponse>(response);
		}

		protected new async Task PutWithEmptyResultAsync<T>(string requestUri, T item)
		{
			await _requestHeaderHelper.ValidateAuthHeaders(Client, Settings);
			var response = await Client.PutAsync(requestUri, CreateContentFromObject(item));

			if (!response.IsSuccessStatusCode)
				await ParseJsonError(response);
		}

		protected new async Task DeleteAsync(string requestUri)
		{
			await _requestHeaderHelper.ValidateAuthHeaders(Client, Settings);

			var response = await Client.DeleteAsync(requestUri);
			if (!response.IsSuccessStatusCode)
				await ParseJsonError(response);
		}

		protected new async Task<T> PatchAsync<T>(string requestUri, T item)
		{
			await _requestHeaderHelper.ValidateAuthHeaders(Client, Settings);
			var method = new HttpMethod("PATCH");
			var request = new HttpRequestMessage(method, requestUri)
			{
				Content = CreateContentFromObject(item)
			};
			var response = await Client.SendAsync(request);
			return await ParseResult<T>(response);
		}

		protected new async Task<TResponse> PatchAsync<TRequest, TResponse>(string requestUri, TRequest item)
		{
			await _requestHeaderHelper.ValidateAuthHeaders(Client, Settings);
			var method = new HttpMethod("PATCH");
			var request = new HttpRequestMessage(method, requestUri)
			{
				Content = CreateContentFromObject(item)
			};
			var response = await Client.SendAsync(request);
			return await ParseResult<TResponse>(response);
		}

		protected new async Task<T> ParseResult<T>(HttpResponseMessage response)
		{
			if (!response.IsSuccessStatusCode) await ParseJsonError(response);

			await using var stream = await response.Content.ReadAsStreamAsync();
			return DeserializeJsonFromStream<T>(stream);
		}

		protected virtual void OnParseJsonErrorException(Exception ex, HttpResponseMessage response)
		{
		}

		protected async Task ParseJsonError(HttpResponseMessage response)
		{
			var errorJson = await response.Content.ReadAsStringAsync();
			Error errorResponse = null;

			try
			{
				// If there is a response
				if (errorJson.Length > 0)
				{
					// Try to get Error object from JSON
					errorResponse = JsonConvert.DeserializeObject<Error>(errorJson, base.JsonSerializerSettings);
					if (errorResponse == null ||
						(string.IsNullOrWhiteSpace(errorResponse.Title) && errorResponse.Status == 0))
					{
						// Json couldn't be parsed -> create new error object with custom json
						throw new Exception();
					}

					errorResponse.ExtraInfo ??= new Dictionary<string, IEnumerable<string>>();
				}
			}
			catch (Exception ex)
			{
				OnParseJsonErrorException(ex, response);
				errorResponse = new Error
				{
					Title = "Json parse error exception.",
					Status = (int)response.StatusCode,
					ExtraInfo = new Dictionary<string, IEnumerable<string>> { { "json", new[] { errorJson } } }
				};
			}

			// Throw proper exception based on HTTP status
			throw response.StatusCode switch
			{
				HttpStatusCode.NotFound => new NotFoundException(message: errorResponse?.Title ?? "Not found",
					code: errorResponse?.Code ?? "NFOUND001", messages: errorResponse?.ExtraInfo),
				HttpStatusCode.BadRequest => new ValidationException(message: errorResponse?.Title ?? "Bad request",
					code: errorResponse?.Code ?? "UNVALI001", messages: errorResponse?.ExtraInfo),
				HttpStatusCode.Unauthorized => new UnauthorizedException(
					message: errorResponse?.Title ?? "Access denied", code: errorResponse?.Code ?? "UNAUTH001",
					messages: errorResponse?.ExtraInfo),
				HttpStatusCode.Forbidden => new ForbiddenException(message: errorResponse?.Title ?? "Forbidden",
					code: errorResponse?.Code ?? "FORBID001", messages: errorResponse?.ExtraInfo),
				HttpStatusCode.BadGateway => new BadGatewayException(message: "The server could not be located",
					code: "GTWAY001", messages: errorResponse?.ExtraInfo),
				HttpStatusCode.GatewayTimeout => new GatewayTimeoutException(
					message: "The connection to the server timed out", code: "GTWAY002",
					messages: errorResponse?.ExtraInfo),
				_ => new ServiceAgentException(message: errorResponse?.Title,
					code: errorResponse?.Code ?? $"Status: {response.StatusCode}", messages: errorResponse?.ExtraInfo)
			};
		}

	}
}
