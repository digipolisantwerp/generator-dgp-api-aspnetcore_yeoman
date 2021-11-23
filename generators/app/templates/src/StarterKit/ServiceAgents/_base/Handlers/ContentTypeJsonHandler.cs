using StarterKit.Shared.Constants;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace StarterKit.ServiceAgents._base.Handlers
{
    public class ContentTypeJsonHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.Content != null && !(request.Content is MultipartFormDataContent))
                request.Content.Headers.ContentType = new MediaTypeHeaderValue(MediaType.Json);

            return base.SendAsync(request, cancellationToken);
        }
    }
}
