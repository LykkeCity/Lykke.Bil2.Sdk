using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Lykke.Bil2.Client.BlocksReader.Tests.Configuration;

namespace Lykke.Bil2.Client.BlocksReader.Tests.Tests
{
    public class RabbitMqConfigurator
    {
        private readonly LaunchSettingsFixture _fixture;
        private readonly string _connString;

        public RabbitMqConfigurator(LaunchSettingsFixture fixture)
        {
            _fixture = fixture;
            _connString = fixture.RabbitMqTestSettings.GetConnectionString();
        }

        public string RabbitMqConnString
        {
            get { return _connString; }
        }

        public async Task ConfigureRabbitMqAsync()
        {
            await CleanRabbitAsync(true);
        }

        public async Task CleanRabbitAsync(bool createVhost = false)
        {
            string username = _fixture.RabbitMqTestSettings.Username;
            string password = _fixture.RabbitMqTestSettings.Password;
            string host = _fixture.RabbitMqTestSettings.Host;
            string vhost = _fixture.RabbitMqTestSettings.Vhost;

            using (HttpClient httpClient = new HttpClient())
            {
                string[] queueNames;
                string url = $"http://{host}:15672/api";

                {
                    //get previous queues
                    var httpRequest = new HttpRequestMessage(HttpMethod.Get, url + $@"/queues/{vhost}");
                    httpRequest.SetBasicAuthentication(username, password);

                    var httpResponse = await httpClient.SendAsync(httpRequest);
                    if (httpResponse.IsSuccessStatusCode)
                    {
                        string data = await httpResponse.Content.ReadAsStringAsync();
                        var queues = Newtonsoft.Json.JsonConvert.DeserializeObject<RabbitQueue[]>(data);
                        queueNames = queues?.Select(x => x.Name).ToArray();
                    }
                    else
                    {
                        queueNames = null;
                    }
                }

                {
                    //Delete old queues
                    if (queueNames != null)
                    {
                        foreach (var queueName in queueNames)
                        {
                            var httpRequest = new HttpRequestMessage(HttpMethod.Delete, url + $@"/queues/{vhost}/{queueName}");
                            httpRequest.SetBasicAuthentication(username, password);

                            await httpClient.SendAsync(httpRequest);
                        }
                    }
                }

                {
                    //Delete Vhost
                    var httpRequest = new HttpRequestMessage(HttpMethod.Delete, url + $@"/vhosts/{vhost}");
                    httpRequest.SetBasicAuthentication(username, password);

                    await httpClient.SendAsync(httpRequest);
                }

                if (createVhost)
                    await CreateVhostAsync(httpClient, url, username, password, vhost);
            }
        }

        private async Task CreateVhostAsync(HttpClient httpClient, string url, string username, string password, string vhost)
        {
            //Create vhost
            var httpRequest = new HttpRequestMessage(HttpMethod.Put, url + $@"/vhosts/{vhost}");
            httpRequest.SetBasicAuthentication(username, password);

            await httpClient.SendAsync(httpRequest);
        }
    }
}
