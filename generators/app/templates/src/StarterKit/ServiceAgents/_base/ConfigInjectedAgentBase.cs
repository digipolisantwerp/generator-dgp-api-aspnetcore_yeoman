using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StarterKit.ServiceAgents._base.Helper;

namespace StarterKit.ServiceAgents._base
{
	public abstract class ConfigInjectedAgentBase<TAgent> : AgentBase<TAgent>, IDisposable where TAgent : class
	{
		private readonly IRequestHeaderHelper _requestHeaderHelper;
		
		protected ConfigInjectedAgentBase(ILogger<TAgent> logger, HttpClient httpClient, IServiceProvider serviceProvider) : base(logger, httpClient, serviceProvider)
		{
			_requestHeaderHelper = serviceProvider.GetRequiredService<IRequestHeaderHelper>();	
		}

		protected new async Task<T> GetAsync<T>(string requestUri)
		{
			await _requestHeaderHelper.ValidateAuthHeaders(Client, Settings);
			return await base.GetAsync<T>(requestUri);
		}

		protected new async Task<string> GetStringAsync(string requestUri)
		{
			await _requestHeaderHelper.ValidateAuthHeaders(Client, Settings);
			return await base.GetStringAsync(requestUri);
		}

		protected new async Task<HttpResponseMessage> GetResponseAsync(string requestUri)
		{
			await _requestHeaderHelper.ValidateAuthHeaders(Client, Settings);
			return await base.GetResponseAsync(requestUri);
		}

		protected new async Task<T> PostAsync<T>(string requestUri, T item)
		{
			await _requestHeaderHelper.ValidateAuthHeaders(Client, Settings);
			return await base.PostAsync<T>(requestUri, item);
		}

		protected new async Task<TResponse> PostAsync<TRequest, TResponse>(string requestUri, TRequest item)
		{
			await _requestHeaderHelper.ValidateAuthHeaders(Client, Settings);
			return await base.PostAsync<TRequest, TResponse>(requestUri, item);
		}

		protected new async Task<T> PutAsync<T>(string requestUri, T item)
		{
			await _requestHeaderHelper.ValidateAuthHeaders(Client, Settings);
			return await base.PutAsync<T>(requestUri, item);
		}

		protected new async Task<TResponse> PutAsync<TRequest, TResponse>(string requestUri, TRequest item)
		{
			await _requestHeaderHelper.ValidateAuthHeaders(Client, Settings);
			return await base.PutAsync<TRequest, TResponse>(requestUri, item);
		}

		protected new async Task PutWithEmptyResultAsync<T>(string requestUri, T item)
		{
			await _requestHeaderHelper.ValidateAuthHeaders(Client, Settings);
			await base.PutWithEmptyResultAsync<T>(requestUri, item);
		}

		protected new async Task DeleteAsync(string requestUri)
		{
			await _requestHeaderHelper.ValidateAuthHeaders(Client, Settings);
			await base.DeleteAsync(requestUri);
		}

		protected new async Task<T> PatchAsync<T>(string requestUri, T item)
		{
			await _requestHeaderHelper.ValidateAuthHeaders(Client, Settings);
			return await base.PatchAsync<T>(requestUri, item);
		}

		protected new async Task<TResponse> PatchAsync<TRequest, TResponse>(string requestUri, TRequest item)
		{
			await _requestHeaderHelper.ValidateAuthHeaders(Client, Settings);
			return await base.PatchAsync<TRequest, TResponse>(requestUri, item);
		}
	}
}
