using Lykke.Bil2.Client.BlocksReader.Services;
using Lykke.Bil2.Client.BlocksReader.Tests.Configuration;
using Lykke.Bil2.Sdk.BlocksReader;
using Lykke.Bil2.Sdk.BlocksReader.Services;
using Lykke.Bil2.Sdk.BlocksReader.Settings;
using Lykke.Sdk.Settings;
using Moq;
using NUnit.Framework;
using System;
using System.IO;
using System.Threading.Tasks;
using Lykke.Bil2.Client.BlocksReader.Tests.RabbitMq;
using Lykke.Bil2.Contract.BlocksReader.Commands;
using Lykke.Bil2.Contract.BlocksReader.Events;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.Bil2.Client.BlocksReader.Tests.Tests
{
    [TestFixture]
    public class BlocksReaderClientTests : BlocksReaderClientBase
    {
        private static readonly string _pathToSettings = "appsettings.tests.json";

        [OneTimeSetUp]
        public void GlobalSetup()
        {
            PrepareSettings();
        }

        [OneTimeTearDown]
        public void GlobalTeardown()
        {
        }

        //[Test]
        public async Task Get_is_alive()
        {
            //ARRANGE
            var (api, client) = PrepareClient<AppSettings>((options) =>
            {
                 CreateMocks(
                    out var blockReader,
                    out var blockProvider);

                options.IntegrationName = $"{nameof(BlocksReaderClientTests)}+{nameof(Get_is_alive)}";
                blockReader.Setup(x => x.ReadBlockAsync(1, It.IsAny<IBlockListener>())).Returns(Task.CompletedTask);
                options.BlockReaderFactory = c => blockReader.Object;
                options.AddIrreversibleBlockPulling(c => blockProvider.Object);
                options.DisableLogging = true;
                options.UseSettings = (s, context) =>
                {
                    s.AddSingleton(new FakeRabbitMqEndpoint());
                };
            }, (clientOptions) =>
            {
                var blockEventsHandler = BlockEventsHandlerCreateMock();
                clientOptions.BlockEventsHandlerFactory = (context) => blockEventsHandler.Object;
            });

            //ACT
            client.Start();
            await api.SendAsync(new ReadBlockCommand(1));
            

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

        private static void ConfigureFactories(BlocksReaderServiceOptions<AppSettings> options)
        {
            options.DisableLogging = true;
        }

        private void PrepareSettings()
        {
            Environment.SetEnvironmentVariable("DisableAutoRegistrationInMonitoring", "true");
            Environment.SetEnvironmentVariable("SettingsUrl", _pathToSettings);

            var prepareSettings = new AppSettings()
            {
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

        private (IBlocksReaderApi, IBlocksReaderClient) PrepareClient<TAppSettings>(Action<BlocksReaderServiceOptions<TAppSettings>> config,
            Action<BlocksReaderClientOptions> clientOptions)
            where TAppSettings : BaseBlocksReaderSettings<DbSettings>
        {
            StartupDependencyFactorySingleton.Instance = new StartupDependencyFactory<TAppSettings>(config);
            var (api, blocksReaderClient) = base.CreateClientApi<StartupTemplate>("http://localhost:5000", clientOptions);

            return (api, blocksReaderClient);
        }
    }
}
