using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Common.Log;
using Newtonsoft.Json;

namespace Lykke.Blockchains.Integrations.HttpClient
{
    internal class LogHttpRequestErrorHandler : DelegatingHandler
    {
        private readonly ILog _log;

        public LogHttpRequestErrorHandler(ILogFactory logFactory)
        {
            if (logFactory == null)
            {
                throw new ArgumentNullException(nameof(logFactory));
            }

            _log = logFactory.CreateLog(this);
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var response = await base.SendAsync(request, cancellationToken);
            var id = Guid.NewGuid();

            if (!response.IsSuccessStatusCode)
            {
                var url = request.RequestUri.ToString();

                await LogRequestAsync(url, request, id);
                await LogResponseAsync(url, response, id);
            }
            
            return response;
        }

        private async Task LogRequestAsync(string url, HttpRequestMessage request, Guid id)
        {
            var message = new StringBuilder();

            message.AppendLine($"Failed request -> {id}: {request.Method.ToString().ToUpper()}");

            foreach (var header in request.Headers)
            {
                message.AppendLine($"{header.Key}: {string.Join(", ", header.Value)}");
            }

            object context = null;

            if (request.Content != null)
            {
                foreach (var header in request.Content.Headers)
                {
                    message.AppendLine($"{header.Key}: {string.Join(", ", header.Value)}");
                }

                if (request.Content is StringContent ||
                    IsTextBasedContentType(request.Headers) ||
                    IsTextBasedContentType(request.Content.Headers))
                {
                    var content = await request.Content.ReadAsStringAsync();
                    context = JsonConvert.DeserializeObject(content);

                    message.AppendLine(content);
                }
            }

            _log.Warning(url, message.ToString(), context: context);
        }

        private async Task LogResponseAsync(string url, HttpResponseMessage response, Guid id)
        {
            var message = new StringBuilder();

            message.AppendLine($"Failed request <- {id}: {(int)response.StatusCode} {response.StatusCode} - {response.ReasonPhrase}");

            foreach (var header in response.Headers)
            {
                message.AppendLine($"{header.Key}: {string.Join(", ", header.Value)}");
            }

            if (response.Content != null)
            {
                foreach (var header in response.Content.Headers)
                {
                    message.AppendLine($"{header.Key}: {string.Join(", ", header.Value)}");
                }

                if (response.Content is StringContent ||
                    IsTextBasedContentType(response.Headers) ||
                    IsTextBasedContentType(response.Content.Headers))
                {
                    var content = await response.Content.ReadAsStringAsync();

                    message.AppendLine(content);
                }
            }

            _log.Warning(url, message.ToString());
        }

        private static bool IsTextBasedContentType(HttpHeaders headers)
        {
            string[] types = { "html", "text", "xml", "json", "txt", "x-www-form-urlencoded" };

            if (!headers.TryGetValues("Content-Type", out var values))
            {
                return false;
            }
            var header = string.Join(" ", values).ToLowerInvariant();

            return types.Any(t => header.Contains(t));
        }
    }
}
