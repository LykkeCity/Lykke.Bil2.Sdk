using Lykke.Bil2.BaseTests;
using Lykke.Bil2.Client.BlocksReader.Services;
using Lykke.Bil2.Client.BlocksReader.Tests.Configuration;
using Lykke.Bil2.Contract.BlocksReader.Commands;
using Lykke.Bil2.Contract.BlocksReader.Events;
using Lykke.Bil2.Contract.Common;
using Lykke.Bil2.Contract.TransactionsExecutor;
using Lykke.Bil2.Sdk.BlocksReader;
using Lykke.Bil2.Sdk.BlocksReader.Repositories;
using Lykke.Bil2.Sdk.BlocksReader.Services;
using Lykke.Bil2.Sdk.BlocksReader.Settings;
using Lykke.Sdk.Settings;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Lykke.Bil2.Client.BlocksReader.Tests.Tests
{
    [TestFixture]
    public class BlocksReaderClientTests : BlocksReaderClientBase
    {
        private static readonly string _integrationName = "TestIntegration";
        private static readonly string _pathToSettings = "appsettings.tests.json";
        private LaunchSettingsFixture _fixture;
        private RabbitMqConfigurator _rabbitMqConfiguration;
        private SettingsMock _settingsMock;

        [OneTimeSetUp]
        public async Task GlobalSetup()
        {
            _fixture = new LaunchSettingsFixture();
            _rabbitMqConfiguration = new RabbitMqConfigurator(_fixture);
            await _rabbitMqConfiguration.ConfigureRabbitMqAsync();
             _settingsMock = new SettingsMock(_pathToSettings);

            var connStringRabbit = _rabbitMqConfiguration.RabbitMqConnString;
            var prepareSettings = new AppSettings()
            {
                RabbitConnStrng = connStringRabbit,
                MessageListeningParallelism = 1,
                LastIrreversibleBlockMonitoringPeriod = TimeSpan.FromSeconds(60),
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

            _settingsMock.PrepareSettings(prepareSettings);
        }

        [OneTimeTearDown]
        public async Task GlobalTeardown()
        {
            await _rabbitMqConfiguration.CleanRabbitAsync();
        }

        [Test]
        public async Task Test_that_read_block_command_is_processed()
        {
            //ARRANGE
            CountdownEvent countdown = new CountdownEvent(2);
            Mock<IBlockReader> blockReader = null;
            var blockEventsHandlerMock = BlockEventsHandlerCreateMock((intName, evt) =>
            {
            });

            var (httpApi, client, apiFactory, testServer) = PrepareClient<AppSettings>(
                serverOptions =>
                {
                    CreateMocks(
                        out blockReader,
                        out var blockProvider);

                    serverOptions.IntegrationName = _integrationName;
                    blockReader
                        .Setup(x => x.ReadBlockAsync(2, It.IsAny<IBlockListener>()))
                        .Returns(Task.CompletedTask)
                        .Callback(() => { countdown.Signal(); });

                    blockReader
                        .Setup(x => x.ReadBlockAsync(1, It.IsAny<IBlockListener>()))
                        .Returns(Task.CompletedTask)
                        .Callback(() => { countdown.Signal(); });
                    blockProvider.Setup(x => x.GetLastAsync()).ReturnsAsync(new LastIrreversibleBlockUpdatedEvent(1, "1"));
                    ConfigureFactories(serverOptions, blockReader, blockProvider);
                },
                clientOptions =>
                {
                    clientOptions.BlockEventsHandlerFactory =
                        (context) => blockEventsHandlerMock.Object;
                    clientOptions.RabbitVhost = _fixture.RabbitMqTestSettings.Vhost;
                    clientOptions.RabbitMqConnString = _rabbitMqConfiguration.RabbitMqConnString;
                    clientOptions.AddIntegration(_integrationName);
                });

            //ACT
            using (testServer)
            using (client)
            {
                client.Start();
                var apiBlocksReader = apiFactory.Create(_integrationName);

                await apiBlocksReader.SendAsync(new ReadBlockCommand(1));
                await apiBlocksReader.SendAsync(new ReadBlockCommand(2));
                countdown.Wait(TimeSpan.FromMinutes(1));
            }

            //ASSERT

            blockReader.Verify(x => x.ReadBlockAsync(1, It.IsNotNull<IBlockListener>()), Times.AtLeastOnce);
            blockReader.Verify(x => x.ReadBlockAsync(2, It.IsNotNull<IBlockListener>()), Times.AtLeastOnce);
            blockReader.Verify(x => x.ReadBlockAsync(It.IsNotIn(1, 2), It.IsNotNull<IBlockListener>()), Times.Never);
        }

        [Test]
        public async Task Test_that_last_irreversible_block_updated_event_is_processed_pulling()
        {
            //ARRANGE
            CountdownEvent countdown = new CountdownEvent(2);
            ManualResetEventSlim irreversibleEvent = new ManualResetEventSlim();
            Mock<IBlockReader> blockReader = null;
            var blockEventsHandlerMock = BlockEventsHandlerCreateMock((intName, evt) =>
            {
                if (evt is LastIrreversibleBlockUpdatedEvent @event)
                {
                    irreversibleEvent.Set();
                }
            });

            var (httpApi, client, apiFactory, testServer) = PrepareClient<AppSettings>(
                serverOptions =>
                {
                    CreateMocks(
                        out blockReader,
                        out var blockProvider);

                    serverOptions.IntegrationName = _integrationName;
                    blockProvider.Setup(x => x.GetLastAsync()).ReturnsAsync(new LastIrreversibleBlockUpdatedEvent(1, "1"));
                    ConfigureFactories(serverOptions, blockReader, blockProvider);
                },
                clientOptions =>
                {
                    clientOptions.BlockEventsHandlerFactory =
                        (context) => blockEventsHandlerMock.Object;
                    clientOptions.RabbitVhost = _fixture.RabbitMqTestSettings.Vhost;
                    clientOptions.RabbitMqConnString = _rabbitMqConfiguration.RabbitMqConnString;
                    clientOptions.AddIntegration(_integrationName);
                });

            //ACT
            using (testServer)
            using (client)
            {
                client.Start();
                irreversibleEvent.Wait(TimeSpan.FromMinutes(1));
            }

            //ASSERT
            blockEventsHandlerMock
                .Verify(x => x.Handle(_integrationName, It.IsNotNull<LastIrreversibleBlockUpdatedEvent>()), Times.AtLeastOnce);
        }

        [Test]
        public async Task Test_that_last_irreversible_block_updated_event_is_processed_pushing()
        {
            //ARRANGE
            CountdownEvent countdown = new CountdownEvent(2);
            ManualResetEventSlim irreversibleEvent = new ManualResetEventSlim();
            Mock<IBlockReader> blockReader = null;
            var blockEventsHandlerMock = BlockEventsHandlerCreateMock((intName, evt) =>
            {
                if (evt is LastIrreversibleBlockUpdatedEvent @event)
                {
                    irreversibleEvent.Set();
                }
            });

            var (httpApi, client, apiFactory, testServer) = PrepareClient<AppSettings>(
                serverOptions =>
                {
                    CreateMocks(
                        out blockReader,
                        out var blockProvider);

                    serverOptions.IntegrationName = _integrationName;

                    ConfigureFactories(serverOptions,
                        blockReader,
                        blockProvider,
                        false);//pushing is set here
                },
                clientOptions =>
                {
                    clientOptions.BlockEventsHandlerFactory =
                        (context) => blockEventsHandlerMock.Object;
                    clientOptions.RabbitVhost = _fixture.RabbitMqTestSettings.Vhost;
                    clientOptions.RabbitMqConnString = _rabbitMqConfiguration.RabbitMqConnString;
                    clientOptions.AddIntegration(_integrationName);
                });

            var irreversibleBlockListener = StartupDependencyFactorySingleton.Instance.ServerServiceProvider.GetService<IIrreversibleBlockListener>();
            //ACT
            using (testServer)
            using (client)
            {
                client.Start();
                await irreversibleBlockListener.HandleNewLastIrreversibleBlockAsync(
                    new LastIrreversibleBlockUpdatedEvent(2, "2"));
                irreversibleEvent.Wait(TimeSpan.FromMinutes(1));
            }

            //ASSERT
            blockEventsHandlerMock
                .Verify(x => x.Handle(_integrationName, It.IsNotNull<LastIrreversibleBlockUpdatedEvent>()), Times.AtLeastOnce);
        }

        [Test]
        public async Task Block_listener_test()
        {
            //ARRANGE
            Mock<IBlockReader> blockReader = null;
            Mock<IRawTransactionWriteOnlyRepository> rawTransactionWriteOnlyRepository = null;
            var typeWaitHandles = new Dictionary<Type, ManualResetEventSlim>()
            {
                { typeof(TransactionFailedEvent), new ManualResetEventSlim()},
                { typeof(BlockHeaderReadEvent), new ManualResetEventSlim()},
                { typeof(TransferAmountTransactionExecutedEvent), new ManualResetEventSlim()},
                { typeof(TransferCoinsTransactionExecutedEvent), new ManualResetEventSlim()},
            };
            var blockEventsHandlerMock = BlockEventsHandlerCreateMock((intName, evt) =>
            {
                if (typeWaitHandles.TryGetValue(evt.GetType(), out var eventWaitHandle))
                {
                    eventWaitHandle.Set();
                }
            });

            var (httpApi, client, apiFactory, testServer) = PrepareClient<AppSettings>(
                serverOptions =>
                {
                    CreateMocks(
                        out blockReader,
                        out var blockProvider);
                    rawTransactionWriteOnlyRepository = new Mock<IRawTransactionWriteOnlyRepository>();
                    rawTransactionWriteOnlyRepository
                        .Setup(x => x.SaveAsync(It.IsNotNull<string>(), It.IsNotNull<Base58String>()))
                        .Returns(Task.CompletedTask)
                        .Verifiable();

                    Action<long, IBlockListener> callBack = async (blockNumber, blockListener) =>
                    {
                        var assetId = new AssetId("assetId");

                        await blockListener.HandleHeaderAsync(new BlockHeaderReadEvent(
                            1,
                            "1",
                            DateTime.UtcNow,
                            256,
                            1,
                            null));

                        await blockListener.HandleExecutedTransactionAsync(Base58String.Encode("transaction.raw"),
                            new TransferAmountTransactionExecutedEvent("1",
                                1,
                                "tr1",
                                new BalanceChange[]
                                {
                                    new BalanceChange("1",
                                        assetId,
                                        CoinsChange.FromDecimal(1000, 4),
                                        new Address("0x2"),
                                        new AddressTag("tag"),
                                        AddressTagType.Text,
                                        1),
                                },
                                new Dictionary<AssetId, CoinsAmount>()
                                {
                                    {assetId, CoinsAmount.FromDecimal(10, 4)}
                                },
                                true));

                        await blockListener.HandleExecutedTransactionAsync(Base58String.Encode("transaction.raw"),
                            new TransferCoinsTransactionExecutedEvent("1",
                                1,
                                "2",
                                new ReceivedCoin[]
                                {
                                    new ReceivedCoin(1,
                                        assetId,
                                        CoinsAmount.FromDecimal(1000, 4),
                                        new Address("0x1"),
                                        new AddressTag("tag"),
                                        AddressTagType.Text,
                                        1)
                                },
                                new CoinReference[]
                                {
                                    new CoinReference("tr1", 0),
                                },
                                new Dictionary<AssetId, CoinsAmount>()
                                {
                                    {assetId, CoinsAmount.FromDecimal(10, 4)}
                                },
                                true));

                        await blockListener.HandleFailedTransactionAsync(Base58String.Encode("transaction.raw"),
                            new TransactionFailedEvent("1",
                                1,
                                "tr1",
                                TransactionBroadcastingError.TransientFailure,
                                "some error message",
                                new Dictionary<AssetId, CoinsAmount>()
                                {
                                    {assetId, CoinsAmount.FromDecimal(10, 4)}
                                }));
                    };
                    serverOptions.IntegrationName = _integrationName;
                    blockReader
                        .Setup(x => x.ReadBlockAsync(It.IsAny<long>(), It.IsAny<IBlockListener>()))
                        .Returns(Task.CompletedTask)
                        .Callback(callBack);

                    serverOptions.UseSettings = (services, set) =>
                        {
                            services.AddSingleton(rawTransactionWriteOnlyRepository.Object);
                        };
                    ConfigureFactories(serverOptions,
                        blockReader,
                        blockProvider,
                        false);//pushing is set here
                },
                clientOptions =>
                {
                    clientOptions.BlockEventsHandlerFactory =
                        (context) => blockEventsHandlerMock.Object;
                    clientOptions.RabbitVhost = _fixture.RabbitMqTestSettings.Vhost;
                    clientOptions.RabbitMqConnString = _rabbitMqConfiguration.RabbitMqConnString;
                    clientOptions.AddIntegration(_integrationName);
                });

            //ACT
            using (testServer)
            using (client)
            {
                client.Start();

                var apiBlocksReader = apiFactory.Create(_integrationName);
                await apiBlocksReader.SendAsync(new ReadBlockCommand(1));

                foreach (var manualResetEventSlim in typeWaitHandles)
                {
                    manualResetEventSlim.Value.Wait(TimeSpan.FromSeconds(30));
                }
            }

            //ASSERT
            rawTransactionWriteOnlyRepository
                .Verify(x => x.SaveAsync(It.IsNotNull<string>(), It.IsNotNull<Base58String>()), Times.AtLeast(3));
            blockEventsHandlerMock
                .Verify(x => x.Handle(_integrationName, It.IsNotNull<BlockHeaderReadEvent>()), Times.AtLeastOnce);
            blockEventsHandlerMock
                .Verify(x => x.Handle(_integrationName, It.IsNotNull<TransferAmountTransactionExecutedEvent>()), Times.AtLeastOnce);
            blockEventsHandlerMock
                .Verify(x => x.Handle(_integrationName, It.IsNotNull<TransactionFailedEvent>()), Times.AtLeastOnce);
            blockEventsHandlerMock
                .Verify(x => x.Handle(_integrationName, It.IsNotNull<TransferCoinsTransactionExecutedEvent>()), Times.AtLeastOnce);
        }

        private static Mock<IBlockEventsHandler> BlockEventsHandlerCreateMock(Action<string, object> callBack)
        {
            Mock<IBlockEventsHandler> blockEventsHandler = new Mock<IBlockEventsHandler>();
            blockEventsHandler.Setup(x => x.Handle(It.IsAny<string>(), It.IsAny<BlockHeaderReadEvent>()))
                .Returns(Task.CompletedTask)
                .Callback(callBack)
                .Verifiable();
            blockEventsHandler.Setup(x => x.Handle(It.IsAny<string>(), It.IsAny<TransferAmountTransactionExecutedEvent>()))
                .Returns(Task.CompletedTask)
                .Callback(callBack)
                .Verifiable();
            blockEventsHandler.Setup(x => x.Handle(It.IsAny<string>(), It.IsAny<TransferCoinsTransactionExecutedEvent>()))
                .Returns(Task.CompletedTask)
                .Callback(callBack)
                .Verifiable();
            blockEventsHandler.Setup(x => x.Handle(It.IsAny<string>(), It.IsAny<TransactionFailedEvent>()))
                .Returns(Task.CompletedTask)
                .Callback(callBack)
                .Verifiable();
            blockEventsHandler.Setup(x => x.Handle(It.IsAny<string>(), It.IsAny<LastIrreversibleBlockUpdatedEvent>()))
                .Returns(Task.CompletedTask)
                .Callback(callBack)
                .Verifiable();

            return blockEventsHandler;
        }

        private static void CreateMocks(out Mock<IBlockReader> blockReader,
            out Mock<IIrreversibleBlockProvider> irreversibleBlockProvider)
        {
            blockReader = new Mock<IBlockReader>();
            irreversibleBlockProvider = new Mock<IIrreversibleBlockProvider>();
        }

        private void ConfigureFactories(BlocksReaderServiceOptions<AppSettings> options,
            Mock<IBlockReader> blockReader,
            Mock<IIrreversibleBlockProvider> irreversibleBlockProvider,
            bool shouldWePull = true)
        {
            options.DisableLogging = true;
            options.BlockReaderFactory = c => blockReader.Object;
            if (shouldWePull)
            {
                options.AddIrreversibleBlockPulling(c => irreversibleBlockProvider.Object);
            }
            else
            {
                options.AddIrreversibleBlockPushing();
            }

            options.RabbitVhost = _fixture.RabbitMqTestSettings.Vhost;
        }

        private (IBlocksReaderHttpApi,
            IBlocksReaderClient,
            IBlocksReaderApiFactory,
            IDisposable) PrepareClient<TAppSettings>(
            Action<BlocksReaderServiceOptions<TAppSettings>> configureServer,
            Action<BlocksReaderClientOptions> configureClient)
            where TAppSettings : BaseBlocksReaderSettings<DbSettings>
        {
            StartupDependencyFactorySingleton.Instance = new StartupDependencyFactory<TAppSettings>(configureServer);
            var (httpApi, blocksReaderClient, apiFactory, testServer) = CreateClientApi<StartupTemplate>("http://localhost:5000", configureClient);

            return (httpApi, blocksReaderClient, apiFactory, testServer);
        }
    }
}
