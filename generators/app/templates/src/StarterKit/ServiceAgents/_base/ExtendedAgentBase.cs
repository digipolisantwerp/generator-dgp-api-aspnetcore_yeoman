using StarterKit.Shared.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace StarterKit.ServiceAgents._base
{
    public abstract class ExtendedAgentBase<TAgent> where TAgent : class
    {
        protected ExtendedAgentBase(ILogger<TAgent> logger, HttpClient httpClient)
        {
            Logger = logger ?? throw new ArgumentException($"{GetType().Name}.Ctor parameter {nameof(logger)} cannot be null.");
            HttpClient = httpClient ?? throw new ArgumentException($"{GetType().Name}.Ctor parameter {nameof(httpClient)} cannot be null.");
        }

        protected ILogger<TAgent> Logger { get; private set; }
        protected HttpClient HttpClient { get; set; }

        private readonly HttpCompletionOption _defaultHttpCompletionOption = HttpCompletionOption.ResponseHeadersRead;

        protected async Task<T> GetAsync<T>(string requestUri)
        {
            using (var response = await HttpClient.GetAsync(requestUri, _defaultHttpCompletionOption))
            {
                using (var stream = await response.Content.ReadAsStreamAsync())
                {
                    return await Utf8Json.JsonSerializer.DeserializeAsync<T>(stream);
                }
            }
        }

        protected async Task<string> GetStringAsync(string requestUri)
        {
            using (var response = await HttpClient.GetAsync(requestUri, _defaultHttpCompletionOption))
                return await response.Content.ReadAsStringAsync();
        }

        protected async Task<T> PostAsync<T>(string requestUri, T item)
        {
            using (HttpContent contentPost = new ByteArrayContent(Utf8Json.JsonSerializer.Serialize(item)))
            {
                using (var response = await HttpClient.PostAsync(requestUri, contentPost, _defaultHttpCompletionOption))
                    return await ParseResult<T>(response);
            }
        }

        protected async Task<TReponse> PostAsync<TRequest, TReponse>(string requestUri, TRequest item)
        {
            using (HttpContent contentPost = new ByteArrayContent(Utf8Json.JsonSerializer.Serialize(item)))
            using (var response = await HttpClient.PostAsync(requestUri, contentPost, _defaultHttpCompletionOption))
                return await ParseResult<TReponse>(response);
        }

        protected async Task<T> PutAsync<T>(string requestUri, T item)
        {
            using (HttpContent contentPost = new ByteArrayContent(Utf8Json.JsonSerializer.Serialize(item)))
            {
                using (var response = await HttpClient.PutAsync(requestUri, contentPost, _defaultHttpCompletionOption))
                {
                    return await ParseResult<T>(response);
                }
            }
        }

        protected async Task<TReponse> PutAsync<TRequest, TReponse>(string requestUri, TRequest item)
        {
            using (HttpContent contentPost = new ByteArrayContent(Utf8Json.JsonSerializer.Serialize(item)))
            {
                using (var response = await HttpClient.PutAsync(requestUri, contentPost, _defaultHttpCompletionOption))
                    return await ParseResult<TReponse>(response);
            }
        }

        protected async Task PutWithEmptyResultAsync<T>(string requestUri, T item)
        {
            using (HttpContent contentPost = new ByteArrayContent(Utf8Json.JsonSerializer.Serialize(item)))
            {
                using (await HttpClient.PutAsync(requestUri, contentPost, _defaultHttpCompletionOption))
                { }
            }
        }

        protected async Task DeleteAsync(string requestUri)
        {
            using (await HttpClient.DeleteAsync(requestUri, _defaultHttpCompletionOption))
            { }
        }

        protected async Task<T> PatchAsync<T>(string requestUri, T item)
        {
            using (HttpContent contentPatch = new ByteArrayContent(Utf8Json.JsonSerializer.Serialize(item)))
            {
                var method = new HttpMethod("PATCH");
                using (var request = new HttpRequestMessage(method, requestUri) { Content = contentPatch })
                {
                    using (var response = await HttpClient.SendAsync(request, _defaultHttpCompletionOption))
                        return await ParseResult<T>(response);
                }
            }
        }

        protected async Task<TReponse> PatchAsync<TRequest, TReponse>(string requestUri, TRequest item)
        {
            using (HttpContent contentPatch = new ByteArrayContent(Utf8Json.JsonSerializer.Serialize(item)))
            {
                var method = new HttpMethod("PATCH");
                using (var request = new HttpRequestMessage(method, requestUri) { Content = contentPatch })
                {
                    using (var response = await HttpClient.SendAsync(request, _defaultHttpCompletionOption))
                        return await ParseResult<TReponse>(response);
                }
            }
        }

        protected async Task<T> ParseResult<T>(HttpResponseMessage response)
        {
            using (var stream = await response.Content.ReadAsStreamAsync())
            {
                return await Utf8Json.JsonSerializer.DeserializeAsync<T>(stream);
            }
        }
    }
}
