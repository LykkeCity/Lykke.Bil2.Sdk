using Lykke.Bil2.Client.BlocksReader.Services;
using Lykke.Bil2.Client.BlocksReader.Tests.Configuration;
using Lykke.Bil2.Contract.BlocksReader.Commands;
using Lykke.Bil2.Contract.BlocksReader.Events;
using Lykke.Bil2.Sdk.BlocksReader;
using Lykke.Bil2.Sdk.BlocksReader.Services;
using Lykke.Bil2.Sdk.BlocksReader.Settings;
using Lykke.Sdk.Settings;
using Moq;
using NUnit.Framework;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using MongoDB.Bson.IO;
using Newtonsoft.Json;
using JsonConvert = MongoDB.Bson.IO.JsonConvert;

namespace Lykke.Bil2.Client.BlocksReader.Tests.Tests
{
    /*
     */
    [TestFixture]
    public class BlocksReaderClientTests : BlocksReaderClientBase
    {
        private static readonly string _pathToSettings = "appsettings.tests.json";
        private string _rabbitMqconnString;

        [OneTimeSetUp]
        public async Task GlobalSetup()
        {
            LaunchSettingsFixture fixture = new LaunchSettingsFixture();
            await PrepareRabbitMq();
            PrepareSettings();
        }

        [OneTimeTearDown]
        public void GlobalTeardown()
        {
        }

        [Test]
        public async Task Get_is_alive()
        {
            //ARRANGE
            var blockEventsHandler = BlockEventsHandlerCreateMock();
            var (api, client, apiFactory) = PrepareClient<AppSettings>((options) =>
            {
                 CreateMocks(
                    out var blockReader,
                    out var blockProvider);

                options.IntegrationName = $"{nameof(BlocksReaderClientTests)}+{nameof(Get_is_alive)}";
                blockReader.Setup(x => x.ReadBlockAsync(2, It.IsAny<IBlockListener>())).Returns(Task.CompletedTask);
                blockProvider.Setup(x => x.GetLastAsync()).ReturnsAsync(new LastIrreversibleBlockUpdatedEvent(1, "1"));
                ConfigureFactories(options, blockReader, blockProvider);
            }, (clientOptions) =>
            {
                clientOptions.BlockEventsHandlerFactory = (context) => blockEventsHandler.Object;
                clientOptions.RabbitVhost = GetVhost();
                clientOptions.RabbitMqConnString = _rabbitMqconnString;
                clientOptions.AddIntegration("TestCoin");
            });

            //ACT
            client.Start();
            var apiBlocksReader = apiFactory.Create("TestCoin");
            await apiBlocksReader.SendAsync(new ReadBlockCommand(1));

            await Task.Delay(TimeSpan.FromMinutes(1));
            //blockEventsHandler.Verify();
            //ASSERT
        }

        private static Mock<IBlockEventsHandler> BlockEventsHandlerCreateMock()
        {
            Mock<IBlockEventsHandler> blockEventsHandler = new Mock<IBlockEventsHandler>();
            blockEventsHandler.Setup(x => x.Handle(It.IsAny<string>(), It.IsAny<BlockHeaderReadEvent>()))
                .Returns(Task.CompletedTask).Verifiable();
            blockEventsHandler.Setup(x => x.Handle(It.IsAny<string>(), It.IsAny<TransferAmountTransactionExecutedEvent>()))
                .Returns(Task.CompletedTask).Verifiable();
            blockEventsHandler.Setup(x => x.Handle(It.IsAny<string>(), It.IsAny<TransferCoinsTransactionExecutedEvent>()))
                .Returns(Task.CompletedTask).Verifiable();
            blockEventsHandler.Setup(x => x.Handle(It.IsAny<string>(), It.IsAny<TransactionFailedEvent>()))
                .Returns(Task.CompletedTask).Verifiable();
            blockEventsHandler.Setup(x => x.Handle(It.IsAny<string>(), It.IsAny<LastIrreversibleBlockUpdatedEvent>()))
                .Returns(Task.CompletedTask).Verifiable();
            return blockEventsHandler;
        }

        private static void CreateMocks(out Mock<IBlockReader> blockReader,
            out Mock<IIrreversibleBlockProvider> irreversibleBlockProvider)
        {
            blockReader= new Mock<IBlockReader>();
            irreversibleBlockProvider = new Mock<IIrreversibleBlockProvider>();
        }

        private void ConfigureFactories(BlocksReaderServiceOptions<AppSettings> options,
            Mock<IBlockReader> blockReader,
            Mock<IIrreversibleBlockProvider> irreversibleBlockProvider)
        {
            options.DisableLogging = true;
            options.BlockReaderFactory = c => blockReader.Object;
            options.AddIrreversibleBlockPulling(c => irreversibleBlockProvider.Object);
            options.DisableLogging = true;
            options.RabbitVhost = GetVhost();
        }

        private string GetVhost()
        {
            var envInfo = Environment.GetEnvironmentVariable("ENV_INFO");

            return envInfo;
        }

        private async Task PrepareRabbitMq()
        {
            var vhost = GetVhost();
            var host = Environment.GetEnvironmentVariable("RabbitHost");
            var port = Environment.GetEnvironmentVariable("RabbitPort");
            var username = Environment.GetEnvironmentVariable("RabbitUsername");
            var password = Environment.GetEnvironmentVariable("RabbitPassword");
            _rabbitMqconnString = $"amqp://{username}:{password}@{host}:{port}";

            using (HttpClient httpClient = new HttpClient())
            {
                string [] queueNames;
                string url = $"http://{host}:15672/api";

                {
                    //get previous queues
                    var httpRequest = new HttpRequestMessage(HttpMethod.Get, url + $@"/queues/{vhost}");
                    httpRequest.SetBasicAuthentication(username, password);

                    var httpResponse = await httpClient.SendAsync(httpRequest);
                    string data = await httpResponse.Content.ReadAsStringAsync();
                    var queues = Newtonsoft.Json.JsonConvert.DeserializeObject<RabbitQueue[]>(data);
                    queueNames = queues?.Select(x => x.Name).ToArray();
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

                {
                    //Create vhost
                    var httpRequest = new HttpRequestMessage(HttpMethod.Put, url + $@"/vhosts/{vhost}");
                    httpRequest.SetBasicAuthentication(username, password);

                    await httpClient.SendAsync(httpRequest);
                }
            }
        }

        private void PrepareSettings()
        {
            var host = Environment.GetEnvironmentVariable("RabbitHost");
            var port = Environment.GetEnvironmentVariable("RabbitPort");
            var username = Environment.GetEnvironmentVariable("RabbitUsername");
            var password = Environment.GetEnvironmentVariable("RabbitPassword");

            Environment.SetEnvironmentVariable("DisableAutoRegistrationInMonitoring", "true");
            Environment.SetEnvironmentVariable("SettingsUrl", _pathToSettings);

            var prepareSettings = new AppSettings()
            {
                RabbitConnStrng = $"amqp://{username}:{password}@{host}:{port}",
                MessageListeningParallelism = 1,
                LastIrreversibleBlockMonitoringPeriod = TimeSpan.FromMinutes(1),
                Db = new DbSettings()
                {
                    AzureDataConnString = "empty",
                    LogsConnString = "empty"
                },
                NodeUrl = "http://localhost:7777/api",
                NodeUser = "user",
                NodePassword = "password",
                MonitoringServiceClient = new MonitoringServiceClientSettings()
                { MonitoringServiceUrl = "http://localhost:5431" },
            };

            string serializedSettings = Newtonsoft.Json.JsonConvert.SerializeObject(prepareSettings);

            try
            {
                File.Delete(_pathToSettings);
            }
            catch
            {
            }

            File.AppendAllText(_pathToSettings, serializedSettings);
        }

        private (IBlocksReaderHttpApi, IBlocksReaderClient, IBlocksReaderApiFactory) PrepareClient<TAppSettings>(Action<BlocksReaderServiceOptions<TAppSettings>> config,
            Action<BlocksReaderClientOptions> clientOptions)
            where TAppSettings : BaseBlocksReaderSettings<DbSettings>
        {
            StartupDependencyFactorySingleton.Instance = new StartupDependencyFactory<TAppSettings>(config);
            var (api, blocksReaderClient, apiFactory) = base.CreateClientApi<StartupTemplate>("http://localhost:5000", clientOptions);

            return (api, blocksReaderClient, apiFactory);
        }
    }

    [DataContract]
    public class RabbitQueuesMetadata
    {
        [DataMember(Name = "items")]
        public RabbitQueue[] Items { get; set; }
    }

    [DataContract]
    public class RabbitQueue
    {
        [DataMember( Name = "name")]
        public string Name  { get; set; }
    }
}
