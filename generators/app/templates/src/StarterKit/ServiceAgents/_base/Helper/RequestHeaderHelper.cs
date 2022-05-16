using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StarterKit.ServiceAgents._base.Auth;
using StarterKit.ServiceAgents._base.Settings;
using StarterKit.Shared.Constants;

namespace StarterKit.ServiceAgents._base.Helper
{
	public class RequestHeaderHelper : IRequestHeaderHelper
	{
		protected IServiceProvider ServiceProvider;
		protected IOAuthAgent TokenHelper;

		public RequestHeaderHelper(IServiceProvider serviceProvider)
		{
			ServiceProvider = serviceProvider;
		}

		public async Task InitializeHeaders(HttpClient client, AgentSettingsBase settings)
		{
			client.DefaultRequestHeaders.Accept.Clear();
			client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaType.Json));

			switch (settings.AuthScheme)
			{
				case AuthScheme.OAuthClientCredentials:
					await SetOAuthClientCredentialsAuthHeader(client, settings);
					break;
				case AuthScheme.Bearer:
					throw new Exception("Not implemented");
				case AuthScheme.Basic:
					throw new Exception("Not implemented");
			}

			if (settings.Headers != null)
			{
				foreach (var (key, value) in settings?.Headers)
				{
					client.DefaultRequestHeaders.Add(key, value);
				}
			}
		}

		public async Task ValidateAuthHeaders(HttpClient client, AgentSettingsBase settings)
		{
			switch (settings.AuthScheme)
			{
				case AuthScheme.OAuthClientCredentials:
					// validate lifetime OAuth access token 
					await SetOAuthClientCredentialsAuthHeader(client, settings);
					break;
				case AuthScheme.Bearer:
					// no change required
					break;
				case AuthScheme.Basic:
					// no change required
					break;
			}
		}

		private async Task SetOAuthClientCredentialsAuthHeader(HttpClient client, AgentSettingsBase settings)
		{
			// tokenHelper is only needed for OAuth
			if (TokenHelper == null)
			{
				TokenHelper = ServiceProvider.GetService<IOAuthAgent>();
				if (TokenHelper == null) throw new NullReferenceException($"{nameof(IOAuthAgent)} cannot be null.");
			}

			var token = await TokenHelper.ReadOrRetrieveAccessToken(settings);
			var accessToken = token;
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AuthScheme.Bearer, accessToken);
		}

		private bool IsDevelopmentEnvironment()
		{
			var hostingEnvironment = ServiceProvider.GetRequiredService<IWebHostEnvironment>();
			return hostingEnvironment.IsDevelopment();
		}
	}
}