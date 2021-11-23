using Digipolis.Correlation;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace StarterKit.ServiceAgents._base.Handlers
{
    public class EventQueueCorrelationIdHandler : DelegatingHandler
    {
        private readonly ICorrelationService _correlationService;

        public EventQueueCorrelationIdHandler(ICorrelationService correlationService)
        {
            _correlationService = correlationService ?? throw new ArgumentNullException(nameof(correlationService));
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.Headers.All(x => !string.Equals(x.Key, CorrelationHeader.Key, StringComparison.InvariantCultureIgnoreCase)))
                request.Headers.TryAddWithoutValidation(CorrelationHeader.Key, _correlationService.GetContext().DgpHeader);
            return base.SendAsync(request, cancellationToken);
        }
    }
}