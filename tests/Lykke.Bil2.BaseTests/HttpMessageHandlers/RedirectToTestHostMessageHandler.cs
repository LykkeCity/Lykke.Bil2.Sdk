using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Lykke.Bil2.BaseTests.Extensions;

namespace Lykke.Bil2.BaseTests.HttpMessageHandlers
{
    public class RedirectToTestHostMessageHandler : DelegatingHandler
    {
        private readonly HttpClient _memoryPipelineClient;

        public RedirectToTestHostMessageHandler(HttpClient memoryPipelineClient)
        {
            _memoryPipelineClient = memoryPipelineClient;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var clonedRequest = await request.CloneAsync();
            var response = await _memoryPipelineClient.SendAsync(clonedRequest);

            return response;
        }
    }
}
