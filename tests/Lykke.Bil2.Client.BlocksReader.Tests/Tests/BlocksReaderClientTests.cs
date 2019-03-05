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
using Lykke.Bil2.Client.BlocksReader.Tests.RabbitMq;

namespace Lykke.Bil2.Client.BlocksReader.Tests.Tests
{
    /*
     */
    [TestFixture]
    public class BlocksReaderClientTests : BlocksReaderClientBase
    {
        private static readonly string _pathToSettings = "appsettings.tests.json";
        private string _rabbitMqConnString;

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
        public async Task Test_that_read_block_command_is_processed()
        {
            //ARRANGE
            var integrationName = "TestIntegration";
            
            Mock<IBlockReader> blockReader = null;

            var (httpApi, client, apiFactory) = PrepareClient<AppSettings>(
                serverOptions =>
                {
                    CreateMocks(
                        out blockReader,
                        out var blockProvider);

                    serverOptions.IntegrationName = integrationName;
                    blockReader.Setup(x => x.ReadBlockAsync(2, It.IsAny<IBlockListener>())).Returns(Task.CompletedTask);
                    blockReader.Setup(x => x.ReadBlockAsync(1, It.IsAny<IBlockListener>())).Returns(Task.CompletedTask);
                    blockProvider.Setup(x => x.GetLastAsync()).ReturnsAsync(new LastIrreversibleBlockUpdatedEvent(1, "1"));
                    ConfigureFactories(serverOptions, blockReader, blockProvider);
                },
                clientOptions =>
                {
                    clientOptions.BlockEventsHandlerFactory =
                        (context) => new FakeBlocksEventHandler(); //blockEventsHandler.Object;
                    clientOptions.RabbitVhost = GetVhost();
                    clientOptions.RabbitMqConnString = _rabbitMqConnString;
                    clientOptions.AddIntegration(integrationName);
                });

            client.Start();
            var apiBlocksReader = apiFactory.Create(integrationName);

            //ACT

            await apiBlocksReader.SendAsync(new ReadBlockCommand(1));
            await apiBlocksReader.SendAsync(new ReadBlockCommand(2));

            await Task.Delay(TimeSpan.FromSeconds(10));
            
            //ASSERT

            blockReader.Verify(x => x.ReadBlockAsync(1, It.IsNotNull<IBlockListener>()), Times.AtLeastOnce);
            blockReader.Verify(x => x.ReadBlockAsync(2, It.IsNotNull<IBlockListener>()), Times.AtLeastOnce);
            blockReader.Verify(x => x.ReadBlockAsync(It.IsNotIn(1, 2), It.IsNotNull<IBlockListener>()), Times.Never);
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
            _rabbitMqConnString = $"amqp://{username}:{password}@{host}:{port}";

            using (HttpClient httpClient = new HttpClient())
            {
                string [] queueNames;
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

        private (IBlocksReaderHttpApi, IBlocksReaderClient, IBlocksReaderApiFactory) PrepareClient<TAppSettings>(
            Action<BlocksReaderServiceOptions<TAppSettings>> configureServer,
            Action<BlocksReaderClientOptions> configureClient)
            where TAppSettings : BaseBlocksReaderSettings<DbSettings>
        {
            StartupDependencyFactorySingleton.Instance = new StartupDependencyFactory<TAppSettings>(configureServer);
            var (httpApi, blocksReaderClient, apiFactory) = CreateClientApi<StartupTemplate>("http://localhost:5000", configureClient);

            return (httpApi, blocksReaderClient, apiFactory);
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
