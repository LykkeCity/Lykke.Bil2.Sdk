using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Lykke.Bil2.BaseTests
{
    public class RabbitMqVhostInitializer
    {
        private readonly RabbitMqTestSettings _settings;
        private readonly string _url;

        public RabbitMqVhostInitializer(RabbitMqTestSettings settings)
        {
            _settings = settings;
            _url = $"http://{_settings.Host}:15672/api";
        }

        public async Task InitializeAsync()
        {
            using (var httpClient = new HttpClient())
            {
                await CleanAsync(httpClient);
                await CreateVhostAsync(httpClient);
            }
        }

        public async Task CleanAsync()
        {
            using (var httpClient = new HttpClient())
            {
                await CleanAsync(httpClient);
            }
        }

        private async Task CleanAsync(HttpClient httpClient)
        {
            var queueNames = await GetExistingQueuesAsync(httpClient);
            
            await RemoveQueuesAsync(httpClient, queueNames);
            await RemoveVhostAsync(httpClient);
        }

        private async Task RemoveVhostAsync(HttpClient httpClient)
        {
            var username = _settings.Username;
            var password = _settings.Password;
            var vhost = _settings.Vhost;

            var httpRequest = new HttpRequestMessage(HttpMethod.Delete, $@"{_url}/vhosts/{vhost}");

            httpRequest.SetBasicAuthentication(username, password);

            await httpClient.SendAsync(httpRequest);
        }

        private async Task RemoveQueuesAsync(HttpClient httpClient, IEnumerable<string> queueNames)
        {
            var username = _settings.Username;
            var password = _settings.Password;
            var vhost = _settings.Vhost;

            foreach (var queueName in queueNames)
            {
                var httpRequest = new HttpRequestMessage(HttpMethod.Delete, $@"{_url}/queues/{vhost}/{queueName}");

                httpRequest.SetBasicAuthentication(username, password);

                await httpClient.SendAsync(httpRequest);
            }
        }

        private async Task<string[]> GetExistingQueuesAsync(HttpClient httpClient)
        {
            var username = _settings.Username;
            var password = _settings.Password;
            var vhost = _settings.Vhost;
            
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, $@"{_url}/queues/{vhost}");

            httpRequest.SetBasicAuthentication(username, password);

            var httpResponse = await httpClient.SendAsync(httpRequest);
            
            if (!httpResponse.IsSuccessStatusCode)
            {
                return Array.Empty<string>();
            }
            
            var data = await httpResponse.Content.ReadAsStringAsync();
            var queues = Newtonsoft.Json.JsonConvert.DeserializeObject<RabbitQueue[]>(data);

            return queues?.Select(x => x.Name).ToArray();
        }

        private async Task CreateVhostAsync(HttpClient httpClient)
        {
            var username = _settings.Username;
            var password = _settings.Password;
            var vhost = _settings.Vhost;

            var httpRequest = new HttpRequestMessage(HttpMethod.Put, $@"{_url}/vhosts/{vhost}");

            httpRequest.SetBasicAuthentication(username, password);

            await httpClient.SendAsync(httpRequest);
        }
    }
}
