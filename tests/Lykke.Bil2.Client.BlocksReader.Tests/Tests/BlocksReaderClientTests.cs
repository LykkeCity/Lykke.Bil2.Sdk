using Lykke.Bil2.BaseTests;
using Lykke.Bil2.Client.BlocksReader.Services;
using Lykke.Bil2.Client.BlocksReader.Tests.Configuration;
using Lykke.Bil2.Contract.BlocksReader.Commands;
using Lykke.Bil2.Contract.BlocksReader.Events;
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
using Lykke.Bil2.Client.BlocksReader.Options;
using Lykke.Bil2.RabbitMq.Publication;
using Lykke.Bil2.Sdk.Repositories;
using Lykke.Bil2.RabbitMq.Subscription;
using Lykke.Bil2.SharedDomain;
using Lykke.Numerics;

namespace Lykke.Bil2.Client.BlocksReader.Tests.Tests
{
    [TestFixture]
    public class BlocksReaderClientTests : BlocksReaderClientBase
    {
        private static readonly string _integrationName = "TestIntegration";
        private static readonly string _pathToSettings = "appsettings.tests.json";
        private RabbitMqVhostInitializer _rabbitMqInitializer;
        private SettingsMock _settingsMock;
        private RabbitMqTestSettings _rabbitMqSettings;

        [OneTimeSetUp]
        public async Task GlobalSetup()
        {
            LaunchSettingsReader.Read();

            _rabbitMqSettings = RabbitMqSettingsReader.Read();
            _rabbitMqInitializer = new RabbitMqVhostInitializer(_rabbitMqSettings);

            await _rabbitMqInitializer.InitializeAsync();

            _settingsMock = new SettingsMock(_pathToSettings);

            var prepareSettings = new AppSettings
            {
                Db = new DbSettings
                {
                    AzureDataConnString = "empty",
                    LogsConnString = "empty"
                },
                RabbitMq = new RabbitMqSettings
                {
                    ConnString = _rabbitMqSettings.GetConnectionString(),
                    MessageConsumersCount = 1,
                    MessageProcessorsCount = 1
                },
                LastIrreversibleBlockMonitoringPeriod = TimeSpan.FromSeconds(5),
                NodeUrl = "http://localhost:7777/api",
                NodeUser = "user",
                NodePassword = "password",
                MonitoringServiceClient = new MonitoringServiceClientSettings
                {
                    MonitoringServiceUrl = "http://localhost:5431"
                }
            };

            _settingsMock.PrepareSettings(prepareSettings);
        }

        [OneTimeTearDown]
        public async Task GlobalTeardown()
        {
            await _rabbitMqInitializer.CleanAsync();
        }

        [Test]
        public async Task Test_that_read_block_command_is_processed()
        {
            //ARRANGE
            var countdown = new CountdownEvent(2);
            Mock<IBlockReader> blockReader = null;
            var blockEventsHandlerMock = BlockEventsHandlerCreateMock((intName, evt, headers, messagePublisher) =>
            {
            });

            var (client, apiFactory, testServer) = PrepareClient<AppSettings>(
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
                    clientOptions.RabbitVhost = _rabbitMqSettings.Vhost;
                    clientOptions.RabbitMqConnString = _rabbitMqSettings.GetConnectionString();
                    clientOptions.AddIntegration(_integrationName);
                });

            //ACT
            using (testServer)
            using (client)
            {
                client.Initialize();
                
                var apiBlocksReader = apiFactory.Create(_integrationName);

                await apiBlocksReader.SendAsync(new ReadBlockCommand(1), null);
                await apiBlocksReader.SendAsync(new ReadBlockCommand(2), null);
                countdown.Wait(Waiting.Timeout);
            }

            //ASSERT

            blockReader.Verify(x => x.ReadBlockAsync(1, It.IsNotNull<IBlockListener>()), Times.AtLeastOnce);
            blockReader.Verify(x => x.ReadBlockAsync(2, It.IsNotNull<IBlockListener>()), Times.AtLeastOnce);
            blockReader.Verify(x => x.ReadBlockAsync(It.IsNotIn(1, 2), It.IsNotNull<IBlockListener>()), Times.Never);
        }

        [Test]
        public void Test_that_last_irreversible_block_updated_event_is_processed_pulling()
        {
            //ARRANGE
            var irreversibleEvent = new ManualResetEventSlim();
            var blockEventsHandlerMock = BlockEventsHandlerCreateMock((intName, evt, headers, messagePublisher) =>
            {
                if (evt is LastIrreversibleBlockUpdatedEvent)
                {
                    irreversibleEvent.Set();
                }
            });

            var (client, _, testServer) = PrepareClient<AppSettings>(
                serverOptions =>
                {
                    CreateMocks(
                        out var blockReader,
                        out var blockProvider);

                    serverOptions.IntegrationName = _integrationName;
                    blockProvider.Setup(x => x.GetLastAsync()).ReturnsAsync(new LastIrreversibleBlockUpdatedEvent(1, "1"));
                    ConfigureFactories(serverOptions, blockReader, blockProvider);
                },
                clientOptions =>
                {
                    clientOptions.BlockEventsHandlerFactory =
                        (context) => blockEventsHandlerMock.Object;
                    clientOptions.RabbitVhost = _rabbitMqSettings.Vhost;
                    clientOptions.RabbitMqConnString = _rabbitMqSettings.GetConnectionString();
                    clientOptions.AddIntegration(_integrationName);
                });

            //ACT
            using (testServer)
            using (client)
            {
                client.Initialize();
                client.StartListening();

                irreversibleEvent.Wait(Waiting.Timeout);
            }

            //ASSERT
            blockEventsHandlerMock
                .Verify(x => x.HandleAsync(_integrationName, It.IsNotNull<LastIrreversibleBlockUpdatedEvent>(), It.IsNotNull<MessageHeaders>(), It.IsNotNull<IMessagePublisher>()), Times.AtLeastOnce);
        }

        [Test]
        public async Task Test_that_last_irreversible_block_updated_event_is_processed_pushing()
        {
            //ARRANGE
            var irreversibleEvent = new ManualResetEventSlim();
            var blockEventsHandlerMock = BlockEventsHandlerCreateMock((intName, evt, headers, messagePublisher) =>
            {
                if (evt is LastIrreversibleBlockUpdatedEvent)
                {
                    irreversibleEvent.Set();
                }
            });

            var (client, _, testServer) = PrepareClient<AppSettings>(
                serverOptions =>
                {
                    CreateMocks(
                        out var blockReader,
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
                    clientOptions.RabbitVhost = _rabbitMqSettings.Vhost;
                    clientOptions.RabbitMqConnString = _rabbitMqSettings.GetConnectionString();
                    clientOptions.AddIntegration(_integrationName);
                });

            var irreversibleBlockListener = StartupDependencyFactorySingleton.Instance.ServerServiceProvider.GetService<IIrreversibleBlockListener>();
            //ACT
            using (testServer)
            using (client)
            {
                client.Initialize();
                client.StartListening();

                await irreversibleBlockListener.HandleNewLastIrreversibleBlockAsync(
                    new LastIrreversibleBlockUpdatedEvent(2, "2"));
                irreversibleEvent.Wait(Waiting.Timeout);
            }

            //ASSERT
            blockEventsHandlerMock
                .Verify(x => x.HandleAsync(_integrationName, It.IsNotNull<LastIrreversibleBlockUpdatedEvent>(), It.IsNotNull<MessageHeaders>(), It.IsNotNull<IMessagePublisher>()), Times.AtLeastOnce);
        }

        [Test]
        public async Task Block_listener_test()
        {
            //ARRANGE
            Mock<IRawObjectWriteOnlyRepository> rawObjectsRepository = null;
            var typeWaitHandles = new Dictionary<Type, ManualResetEventSlim>()
            {
                { typeof(TransactionFailedEvent), new ManualResetEventSlim()},
                { typeof(BlockHeaderReadEvent), new ManualResetEventSlim()},
                { typeof(BlockNotFoundEvent), new ManualResetEventSlim()},
                { typeof(TransferAmountTransactionExecutedEvent), new ManualResetEventSlim()},
                { typeof(TransferCoinsTransactionExecutedEvent), new ManualResetEventSlim()},
            };
            var blockEventsHandlerMock = BlockEventsHandlerCreateMock((intName, evt, headers, messagePublisher) =>
            {
                if (typeWaitHandles.TryGetValue(evt.GetType(), out var eventWaitHandle))
                {
                    eventWaitHandle.Set();
                }
            });

            var (client, apiFactory, testServer) = PrepareClient<AppSettings>(
                serverOptions =>
                {
                    CreateMocks(
                        out var blockReader,
                        out var blockProvider);
                    rawObjectsRepository = new Mock<IRawObjectWriteOnlyRepository>();
                    rawObjectsRepository
                        .Setup(x => x.SaveAsync(RawObjectType.Transaction, It.IsNotNull<string>(), It.IsNotNull<Base58String>()))
                        .Returns(Task.CompletedTask)
                        .Verifiable();

                    async void CallBack(long blockNumber, IBlockListener blockListener)
                    {
                        if(blockNumber == 2)
                        {
                            await blockListener.HandleBlockNotFoundAsync(new BlockNotFoundEvent(blockNumber));
                            return;
                        }

                        var asset = new Asset("assetId");

                        await blockListener.HandleHeaderAsync
                        (
                            new BlockHeaderReadEvent
                            (
                                1,
                                "1",
                                DateTime.UtcNow,
                                256,
                                1
                            )
                        );

                        await blockListener.HandleRawBlockAsync(Base58String.Encode("raw-block"), "1");

                        await blockListener.HandleExecutedTransactionAsync
                        (
                            Base58String.Encode("transaction.raw"),
                            new TransferAmountTransactionExecutedEvent
                            (
                                "1",
                                1,
                                "tr1",
                                new[]
                                {
                                    new BalanceChange
                                    (
                                        "1",
                                        asset,
                                        Money.Create(1000, 4),
                                        new Address("0x2"),
                                        new AddressTag("tag"),
                                        AddressTagType.Text,
                                        1)
                                },
                                new[]
                                {
                                    new Fee(asset, UMoney.Create(10, 4))
                                },
                                true
                            )
                        );

                        await blockListener.HandleExecutedTransactionAsync
                        (
                            Base58String.Encode("transaction.raw"),
                            new TransferCoinsTransactionExecutedEvent
                            (
                                "1",
                                1,
                                "2",
                                new[]
                                {
                                    new ReceivedCoin
                                    (
                                        1,
                                        asset,
                                        UMoney.Create(1000, 4),
                                        new Address("0x1"),
                                        new AddressTag("tag"),
                                        AddressTagType.Text, 1
                                    )
                                },
                                new[]
                                {
                                    new CoinId("tr1", 0),
                                },
                                new[]
                                {
                                    new Fee(asset, UMoney.Create(10, 4))
                                },
                                true
                            )
                        );

                        await blockListener.HandleFailedTransactionAsync
                        (
                            Base58String.Encode("transaction.raw"),
                            new TransactionFailedEvent
                            (
                                "1",
                                1,
                                "tr1",
                                TransactionBroadcastingError.TransientFailure,
                                "some error message",
                                new[]
                                {
                                    new Fee(asset, UMoney.Create(10, 4))
                                }
                            )
                        );
                    }

                    serverOptions.IntegrationName = _integrationName;
                    blockReader
                        .Setup(x => x.ReadBlockAsync(It.IsAny<long>(), It.IsAny<IBlockListener>()))
                        .Returns(Task.CompletedTask)
                        .Callback((Action<long, IBlockListener>) CallBack);

                    serverOptions.UseSettings = (services, set) =>
                        {
                            services.AddSingleton(rawObjectsRepository.Object);
                        };
                    ConfigureFactories(serverOptions,
                        blockReader,
                        blockProvider,
                        false);//pushing is set here
                },
                clientOptions =>
                {
                    clientOptions.BlockEventsHandlerFactory = context => blockEventsHandlerMock.Object;
                    clientOptions.RabbitVhost = _rabbitMqSettings.Vhost;
                    clientOptions.RabbitMqConnString = _rabbitMqSettings.GetConnectionString();
                    clientOptions.AddIntegration(_integrationName);
                });

            var block1CorrelationId = "correlation-id-1";
            var block2CorrelationId = "correlation-id-2";

            //ACT
            using (testServer)
            using (client)
            {
                client.Initialize();
                client.StartListening();

                var apiBlocksReader = apiFactory.Create(_integrationName);
                await apiBlocksReader.SendAsync(new ReadBlockCommand(1), block1CorrelationId);
                await apiBlocksReader.SendAsync(new ReadBlockCommand(2), block2CorrelationId);

                foreach (var manualResetEventSlim in typeWaitHandles)
                {
                    if (!manualResetEventSlim.Value.Wait(Waiting.Timeout))
                    {
                        Console.WriteLine($"Event {manualResetEventSlim.Key} has been missed!");
                    }
                }
            }

            //ASSERT
            rawObjectsRepository
                .Verify(x => x.SaveAsync(RawObjectType.Transaction, It.IsNotNull<string>(), It.IsNotNull<Base58String>()), Times.AtLeast(3));

            rawObjectsRepository
                .Verify(x => x.SaveAsync(RawObjectType.Block, It.IsNotNull<string>(), It.IsNotNull<Base58String>()), Times.AtLeast(1));

            blockEventsHandlerMock
                .Verify(x => 
                    x.HandleAsync(
                        _integrationName, 
                        It.Is<BlockHeaderReadEvent>(b => b.BlockId == "1"), 
                        It.Is<MessageHeaders>(h => h.CorrelationId == block1CorrelationId),
                        It.IsNotNull<IMessagePublisher>()), 
                    Times.AtLeastOnce);
            blockEventsHandlerMock
                .Verify(x => 
                    x.HandleAsync(
                        _integrationName, 
                        It.Is<BlockHeaderReadEvent>(b => b.BlockId != "1"), 
                        It.IsNotNull<MessageHeaders>(),
                        It.IsNotNull<IMessagePublisher>()), 
                    Times.Never);

            blockEventsHandlerMock
                .Verify(x => 
                    x.HandleAsync(
                        _integrationName, 
                        It.Is<BlockNotFoundEvent>(b => b.BlockNumber == 2), 
                        It.Is<MessageHeaders>(h => h.CorrelationId == block2CorrelationId),
                        It.IsNotNull<IMessagePublisher>()), 
                    Times.AtLeastOnce);
            blockEventsHandlerMock
                .Verify(x => 
                    x.HandleAsync(
                        _integrationName, 
                        It.Is<BlockNotFoundEvent>(b => b.BlockNumber != 2),
                        It.IsNotNull<MessageHeaders>(), 
                        It.IsNotNull<IMessagePublisher>()), 
                    Times.Never);

            blockEventsHandlerMock
                .Verify(x => x.HandleAsync(
                    _integrationName, 
                    It.Is<TransferAmountTransactionExecutedEvent>(t => t.BlockId == "1"),
                    It.Is<MessageHeaders>(h => h.CorrelationId == block1CorrelationId),
                    It.IsNotNull<IMessagePublisher>()), 
                    Times.AtLeastOnce);
            blockEventsHandlerMock
                .Verify(x => x.HandleAsync(
                    _integrationName, 
                    It.Is<TransferAmountTransactionExecutedEvent>(t => t.BlockId == "1"),
                    It.Is<MessageHeaders>(h => h.CorrelationId == block1CorrelationId),
                    It.IsNotNull<IMessagePublisher>()), 
                    Times.AtLeastOnce);
            blockEventsHandlerMock
                .Verify(x => x.HandleAsync(
                    _integrationName, 
                    It.Is<TransferAmountTransactionExecutedEvent>(t => t.BlockId == "1"),
                    It.Is<MessageHeaders>(h => h.CorrelationId == block1CorrelationId),
                    It.IsNotNull<IMessagePublisher>()),
                    Times.AtLeastOnce);
        }

        private static Mock<IBlockEventsHandler> BlockEventsHandlerCreateMock(Action<string, object, MessageHeaders, IMessagePublisher> callBack)
        {
            Mock<IBlockEventsHandler> blockEventsHandler = new Mock<IBlockEventsHandler>();
            blockEventsHandler.Setup(x => x.HandleAsync(It.IsAny<string>(), It.IsAny<BlockHeaderReadEvent>(), It.IsAny<MessageHeaders>(), It.IsAny<IMessagePublisher>()))
                .ReturnsAsync(MessageHandlingResult.Success())
                .Callback(callBack)
                .Verifiable();
            blockEventsHandler.Setup(x => x.HandleAsync(It.IsAny<string>(), It.IsAny<BlockNotFoundEvent>(), It.IsAny<MessageHeaders>(), It.IsAny<IMessagePublisher>()))
                .ReturnsAsync(MessageHandlingResult.Success())
                .Callback(callBack)
                .Verifiable();
            blockEventsHandler.Setup(x => x.HandleAsync(It.IsAny<string>(), It.IsAny<TransferAmountTransactionExecutedEvent>(), It.IsAny<MessageHeaders>(), It.IsAny<IMessagePublisher>()))
                .ReturnsAsync(MessageHandlingResult.Success())
                .Callback(callBack)
                .Verifiable();
            blockEventsHandler.Setup(x => x.HandleAsync(It.IsAny<string>(), It.IsAny<TransferCoinsTransactionExecutedEvent>(), It.IsAny<MessageHeaders>(), It.IsAny<IMessagePublisher>()))
                .ReturnsAsync(MessageHandlingResult.Success())
                .Callback(callBack)
                .Verifiable();
            blockEventsHandler.Setup(x => x.HandleAsync(It.IsAny<string>(), It.IsAny<TransactionFailedEvent>(), It.IsAny<MessageHeaders>(), It.IsAny<IMessagePublisher>()))
                .ReturnsAsync(MessageHandlingResult.Success())
                .Callback(callBack)
                .Verifiable();
            blockEventsHandler.Setup(x => x.HandleAsync(It.IsAny<string>(), It.IsAny<LastIrreversibleBlockUpdatedEvent>(), It.IsAny<MessageHeaders>(), It.IsAny<IMessagePublisher>()))
                .ReturnsAsync(MessageHandlingResult.Success())
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

            options.RabbitVhost = _rabbitMqSettings.Vhost;
        }

        private (IBlocksReaderClient,
            IBlocksReaderApiFactory,
            IDisposable) 
            PrepareClient<TAppSettings>(
                Action<BlocksReaderServiceOptions<TAppSettings>> configureServer,
                Action<BlocksReaderClientOptions> configureClient)

            where TAppSettings : BaseBlocksReaderSettings<DbSettings, RabbitMqSettings>
        {
            StartupDependencyFactorySingleton.Instance = new StartupDependencyFactory<TAppSettings>(configureServer);
            var (blocksReaderClient, apiFactory, testServer) = CreateClientApi<StartupTemplate>("TestIntegration", configureClient);

            return (blocksReaderClient, apiFactory, testServer);
        }
    }
}
