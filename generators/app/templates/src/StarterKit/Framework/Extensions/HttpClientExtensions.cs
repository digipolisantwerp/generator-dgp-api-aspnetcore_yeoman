using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace StarterKit.Shared.Extensions
{
	public static class HttpClientExtensions
	{
		public static async Task<HttpResponseMessage> GetAsync(this HttpClient client, string requestUri,
			HttpCompletionOption httpCompletionOptions, Dictionary<string, string> headers = null)
		{
			using (var request = new HttpRequestMessage(HttpMethod.Get, requestUri))
			{
				foreach (var header in headers ?? new Dictionary<string, string>())
					request.Headers.TryAddWithoutValidation(header.Key, header.Value);
				return await client.SendAsync(request, httpCompletionOptions);
			}
		}

		public static async Task<HttpResponseMessage> PostAsync(this HttpClient client, string requestUri,
			HttpContent content, HttpCompletionOption httpCompletionOptions, Dictionary<string, string> headers = null)
		{
			using (var request = new HttpRequestMessage(HttpMethod.Post, requestUri))
			{
				foreach (var header in headers ?? new Dictionary<string, string>())
					request.Headers.TryAddWithoutValidation(header.Key, header.Value);
				request.Content = content;
				return await client.SendAsync(request, httpCompletionOptions);
			}
		}

		public static async Task<HttpResponseMessage> PutAsync(this HttpClient client, string requestUri,
			HttpContent content, HttpCompletionOption httpCompletionOptions, Dictionary<string, string> headers = null)
		{
			using (var request = new HttpRequestMessage(HttpMethod.Put, requestUri))
			{
				foreach (var header in headers ?? new Dictionary<string, string>())
					request.Headers.TryAddWithoutValidation(header.Key, header.Value);
				request.Content = content;
				return await client.SendAsync(request, httpCompletionOptions);
			}
		}

		public static async Task<HttpResponseMessage> PatchAsync(this HttpClient client, string requestUri,
			HttpContent content, HttpCompletionOption httpCompletionOptions, Dictionary<string, string> headers = null)
		{
			using (var request = new HttpRequestMessage(HttpMethod.Patch, requestUri))
			{
				foreach (var header in headers ?? new Dictionary<string, string>())
					request.Headers.TryAddWithoutValidation(header.Key, header.Value);
				request.Content = content;
				return await client.SendAsync(request, httpCompletionOptions);
			}
		}

		public static async Task<HttpResponseMessage> DeleteAsync(this HttpClient client, string requestUri,
			HttpCompletionOption httpCompletionOptions, Dictionary<string, string> headers = null)
		{
			using (var request = new HttpRequestMessage(HttpMethod.Delete, requestUri))
			{
				foreach (var header in headers ?? new Dictionary<string, string>())
					request.Headers.TryAddWithoutValidation(header.Key, header.Value);
				return await client.SendAsync(request, httpCompletionOptions);
			}
		}
	}
}