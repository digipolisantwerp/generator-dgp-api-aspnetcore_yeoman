using StarterKit.ServiceAgents._base.Auth;
using StarterKit.Shared.Constants;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace StarterKit.ServiceAgents._base.Handlers
{
    public class OAuthTokenHandler<TServiceAgentSettings> : DelegatingHandler where TServiceAgentSettings : OAuthAgentSettingsBase, new()
    {
        private readonly IOAuthAgent _oAuthAgent;
        private readonly TServiceAgentSettings _settings;

        public OAuthTokenHandler(IOAuthAgent oAuthAgent, IOptions<TServiceAgentSettings> serviceAgentOptions)
        {
            _oAuthAgent = oAuthAgent ?? throw new ArgumentNullException(nameof(oAuthAgent));
            _settings = serviceAgentOptions?.Value ?? throw new ArgumentNullException(nameof(serviceAgentOptions));
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            //if(_environment.IsProduction()) // ENABLE when debugging multiple microservices locally
            {
                var token = await _oAuthAgent.ReadOrRetrieveAccessToken(_settings);
                request.Headers.Authorization = new AuthenticationHeaderValue(AuthScheme.Bearer, token);
            }
            
            return await base.SendAsync(request, cancellationToken);
        }
    }
}