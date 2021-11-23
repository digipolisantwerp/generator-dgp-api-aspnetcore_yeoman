using StarterKit.Shared.Caching;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace StarterKit.ServiceAgents._base.Auth
{
    public class OAuthAgent : IOAuthAgent
    {
        private readonly ICacheHandler _cacher;
        private readonly HttpClient _client;

        public OAuthAgent(ICacheHandler cacher, HttpClient client)
        {
            _cacher = cacher ?? throw new ArgumentNullException(nameof(cacher), $"{nameof(cacher)} cannot be null");
            _client = client;
        }

        public async Task<string> ReadOrRetrieveAccessToken<ServiceAgentSettings>(ServiceAgentSettings settings) where ServiceAgentSettings : OAuthAgentSettingsBase
        {
            var cacheKey = $"{settings.OAuthClientId}{settings.OAuthClientSecret}{settings.OAuthScope}{settings.OAuthTokenEndpoint}";

            var (succeeded, tokenReplyResult) = await _cacher.GetFromCacheAsync<TokenReply>(cacheKey).ConfigureAwait(false);

            if (!succeeded || tokenReplyResult == null)
            {
                tokenReplyResult = await RetrieveTokenReply(settings.OAuthClientId, settings.OAuthClientSecret, settings.OAuthScope, settings.OAuthTokenEndpoint);

                if (tokenReplyResult == null)
                    throw new Exception($"Unable to retrieve token reply for agent with key {cacheKey}");

                int cacheExpiration = CalculateCacheExpiration(tokenReplyResult);

                if (cacheExpiration > 0)
                {
                    await _cacher.SaveToCacheAsync(cacheKey, tokenReplyResult);
                }
            }
            return tokenReplyResult.access_token;
        }

        private int CalculateCacheExpiration(TokenReply tokenReply)
        {
            var cacheExpiration = tokenReply.expires_in / 60.0 - 1;
            return Convert.ToInt32(Math.Floor(cacheExpiration > 0 ? Math.Truncate(cacheExpiration) : 0));
        }


        private async Task<TokenReply> RetrieveTokenReply(string clientID, string clientSecret, string scope, string tokenEndpoint)
        {
            var content = $"client_id={clientID}&client_secret={clientSecret}&grant_type=client_credentials{(String.IsNullOrWhiteSpace(scope) ? "" : $"&scope={scope}")}";

            using (StringContent stringContent = new StringContent(content, Encoding.UTF8, "application/x-www-form-urlencoded"))
            {
                using (var response = await _client.PostAsync(tokenEndpoint, stringContent))
                {
                    if (!response.IsSuccessStatusCode)
                        throw new Exception("Error retrieving OAuth access token, response status code: " + response.StatusCode);
                    try
                    {
                        using (var stream = await response.Content.ReadAsStreamAsync())
                        {
                            return await Utf8Json.JsonSerializer.DeserializeAsync<TokenReply>(stream);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Error parsing OAuth access token: " + ex.Message, ex);
                    }
                }
            }
        }
    }
}