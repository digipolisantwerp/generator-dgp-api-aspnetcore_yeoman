using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace StarterKit.ServiceAgents._base
{
	public abstract class AgentBase<TAgent> : IDisposable where TAgent : class
	{
		protected AgentBase(ILogger<TAgent> logger,
							HttpClient httpClient,
							IServiceProvider serviceProvider)
		{
			Client = httpClient;
			_logger = logger;
			JsonSerializerSettings = serviceProvider.GetRequiredService<IOptions<MvcNewtonsoftJsonOptions>>()?.Value
				?.SerializerSettings;
		}

		protected readonly JsonSerializerSettings JsonSerializerSettings;

		private const HttpCompletionOption DefaultHttpCompletionOption = HttpCompletionOption.ResponseHeadersRead;

		protected readonly ILogger<TAgent> _logger;
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
			using var response = await Client.PostAsync(requestUri, CreateContentFromObject(item));
			return await ParseResult<T>(response);
		}

		protected async Task<TResponse> PostAsync<TRequest, TResponse>(string requestUri, TRequest item)
		{
			using var response = await Client.PostAsync(requestUri, CreateContentFromObject(item));
			return await ParseResult<TResponse>(response);
		}

		protected async Task<T> PutAsync<T>(string requestUri, T item)
		{
			using var response = await Client.PutAsync(requestUri, CreateContentFromObject(item));
			return await ParseResult<T>(response);
		}

		protected async Task<TResponse> PutAsync<TRequest, TResponse>(string requestUri, TRequest item)
		{
			using var response = await Client.PutAsync(requestUri, CreateContentFromObject(item));
			return await ParseResult<TResponse>(response);
		}

		protected async Task PutWithEmptyResultAsync<T>(string requestUri, T item)
		{
			using (await Client.PutAsync(requestUri, CreateContentFromObject(item)))
			{ }
		}

		protected async Task DeleteAsync(string requestUri)
		{
			using (await Client.DeleteAsync(requestUri))
			{ }
		}

		protected async Task<T> PatchAsync<T>(string requestUri, T item)
		{
			var method = new HttpMethod("PATCH");
			using var request = new HttpRequestMessage(method, requestUri) { Content = CreateContentFromObject(item) };
			using var response = await Client.SendAsync(request, DefaultHttpCompletionOption);
			return await ParseResult<T>(response);
		}

		protected async Task<TResponse> PatchAsync<TRequest, TResponse>(string requestUri, TRequest item)
		{
			var method = new HttpMethod("PATCH");
			using var request = new HttpRequestMessage(method, requestUri) { Content = CreateContentFromObject(item) };
			using var response = await Client.SendAsync(request, DefaultHttpCompletionOption);
			return await ParseResult<TResponse>(response);
		}

		protected async Task<T> ParseResult<T>(HttpResponseMessage response)
		{
			await using var stream = await response.Content.ReadAsStreamAsync();			
			return DeserializeJsonFromStream<T>(stream);
		}

		protected HttpContent CreateContentFromObject<T>(T item)
		{
			HttpContent contentPost = new StringContent(JsonConvert.SerializeObject(item, JsonSerializerSettings),
				Encoding.UTF8, "application/json");
			if (contentPost.Headers.ContentType != null) contentPost.Headers.ContentType.CharSet = string.Empty;

			return contentPost;
		}

		protected static async Task<string> StreamToStringAsync(Stream stream)
		{
			if (stream == null) return null;
			using var streamReader = new StreamReader(stream);
			var content = await streamReader.ReadToEndAsync();
			return content;
		}

		protected static T DeserializeJsonFromStream<T>(Stream stream)
		{
			if (stream == null || stream.CanRead == false)
				return default;

			using var streamReader = new StreamReader(stream);
			using var jsonReader = new JsonTextReader(streamReader);
			var serializer = new JsonSerializer();
			return serializer.Deserialize<T>(jsonReader);
		}
	}
}
