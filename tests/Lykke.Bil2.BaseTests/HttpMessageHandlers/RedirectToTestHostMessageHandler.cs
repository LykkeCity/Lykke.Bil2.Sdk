using System;
using System.IO;
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
            HttpResponseMessage response = null;
            try
            {
                response = await _memoryPipelineClient.SendAsync(clonedRequest, cancellationToken);
            }
            catch (IOException e)
            {
                //TODO: Implement it in elegant way -_-
                //This is used to emulate OperationCanceledException (cause error from TestServer is IOException)
                //Real HttpClient throws OperationCanceledException.
                if (e.Message == "The request was aborted or the pipeline has finished")
                    throw new OperationCanceledException("TIMEOUT");
            }

            return response;
        }
    }
}
