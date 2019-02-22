using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Base.Tests.Extensions;

namespace Base.Tests.HttpMessageHandlers
{
    public class RedirectToTestHostMessageHandler : HttpMessageHandler
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
