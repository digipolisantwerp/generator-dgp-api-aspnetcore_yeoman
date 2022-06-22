using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using StarterKit.ServiceAgents._base.Settings;

namespace StarterKit.ServiceAgents._base
{
	public abstract class AgentBase<TAgent> : IDisposable where TAgent : class
	{
		private readonly ILogger<TAgent> _logger;
		protected readonly AgentSettingsBase Settings;
		protected readonly JsonSerializerSettings JsonSerializerSettings;

		private const HttpCompletionOption DefaultHttpCompletionOption = HttpCompletionOption.ResponseHeadersRead;

		protected AgentBase(ILogger<TAgent> logger, HttpClient httpClient, IServiceProvider serviceProvider)
		{
			var settings = serviceProvider.GetRequiredService<IOptions<ServiceAgentSettings>>();
			if (settings.Value == null)
				throw new ArgumentNullException(nameof(ServiceAgentSettings),
					$"{nameof(ServiceAgentSettings)} cannot be null.");

			Client = httpClient;
			Settings = settings.Value.GetServiceSettings(typeof(TAgent)?.Name);
			_logger = logger;
			JsonSerializerSettings = serviceProvider.GetRequiredService<IOptions<MvcNewtonsoftJsonOptions>>()?.Value
				?.SerializerSettings;
		}

		protected HttpClient Client { get; set; }

		public void Dispose() => Client?.Dispose();

		protected async Task<T> GetAsync<T>(string requestUri)
		{
			using var response = await Client.GetAsync(requestUri, DefaultHttpCompletionOption);
			return await ParseResult<T>(response);
		}

		protected async Task<string> GetStringAsync(string requestUri)
		{
			using var response = await Client.GetAsync(requestUri, DefaultHttpCompletionOption);
			return await response.Content.ReadAsStringAsync();
		}

		protected async Task<T> PostAsync<T>(string requestUri, T item)
		{
			using HttpContent contentPost = new ByteArrayContent(Utf8Json.JsonSerializer.Serialize(item));
			using var response = await Client.PostAsync(requestUri, contentPost);
			return await ParseResult<T>(response);
		}

		protected async Task<TResponse> PostAsync<TRequest, TResponse>(string requestUri, TRequest item)
		{
			using HttpContent contentPost = new ByteArrayContent(Utf8Json.JsonSerializer.Serialize(item));
			using var response = await Client.PostAsync(requestUri, contentPost);
			return await ParseResult<TResponse>(response);
		}

		protected async Task<T> PutAsync<T>(string requestUri, T item)
		{
			using HttpContent contentPost = new ByteArrayContent(Utf8Json.JsonSerializer.Serialize(item));
			using var response = await Client.PutAsync(requestUri, contentPost);
			return await ParseResult<T>(response);
		}

		protected async Task<TResponse> PutAsync<TRequest, TResponse>(string requestUri, TRequest item)
		{
			using HttpContent contentPost = new ByteArrayContent(Utf8Json.JsonSerializer.Serialize(item));
			using var response = await Client.PutAsync(requestUri, contentPost);
			return await ParseResult<TResponse>(response);
		}

		protected async Task PutWithEmptyResultAsync<T>(string requestUri, T item)
		{
			using HttpContent contentPost = new ByteArrayContent(Utf8Json.JsonSerializer.Serialize(item));
			using (await Client.PutAsync(requestUri, contentPost))
			{ }
		}

		protected async Task DeleteAsync(string requestUri)
		{
			using (await Client.DeleteAsync(requestUri))
			{ }
		}

		protected async Task<T> PatchAsync<T>(string requestUri, T item)
		{
			using HttpContent contentPatch = new ByteArrayContent(Utf8Json.JsonSerializer.Serialize(item));
			var method = new HttpMethod("PATCH");
			using var request = new HttpRequestMessage(method, requestUri) { Content = contentPatch };
			using var response = await Client.SendAsync(request, DefaultHttpCompletionOption);
			return await ParseResult<T>(response);
		}

		protected async Task<TResponse> PatchAsync<TRequest, TResponse>(string requestUri, TRequest item)
		{
			using HttpContent contentPatch = new ByteArrayContent(Utf8Json.JsonSerializer.Serialize(item));
			var method = new HttpMethod("PATCH");
			using var request = new HttpRequestMessage(method, requestUri) { Content = contentPatch };
			using var response = await Client.SendAsync(request, DefaultHttpCompletionOption);
			return await ParseResult<TResponse>(response);
		}

		protected async Task<T> ParseResult<T>(HttpResponseMessage response)
		{
			await using var stream = await response.Content.ReadAsStreamAsync();
			using var reader = new StreamReader(stream);
			var content = await reader.ReadToEndAsync();

			return JsonConvert.DeserializeObject<T>(content);
		}
	}
}
