using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using StarterKit.Shared.Constants;

namespace StarterKit.ServiceAgents._base.Handlers
{
	public class MediaTypeJsonHandler : DelegatingHandler
	{
		protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
			CancellationToken cancellationToken)
		{
			request.Headers.Accept.Clear();
			request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaType.Json));
			return base.SendAsync(request, cancellationToken);
		}
	}
}