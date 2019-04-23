using System;
using System.Threading;
using System.Threading.Tasks;
using Lykke.Bil2.BaseTests;
using Lykke.Bil2.RabbitMq.Publication;
using Lykke.Bil2.RabbitMq.Subscription;
using Lykke.Bil2.RabbitMq.Tests.Subscription.Mocks;
using Lykke.Common.Log;
using Lykke.Logs;
using Lykke.Logs.Loggers.LykkeConsole;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;

namespace Lykke.Bil2.RabbitMq.Tests.Subscription
{
    [TestFixture]
    public class MessageHandlerTests
    {
        private const string ExchangeName = "test-exchange";
        private const string RouteName = "test-dependencies-disposal";

        private RabbitMqVhostInitializer _rabbitMqConfiguration;
        private RabbitMqTestSettings _rabbitMqSettings;
        private Mock<ITestMessageHandlerImplementation> _testHandlerImplMock;
        private Mock<ITestMessageHandlerWithStateImplementation> _testHandlerWithStateImplMock;
        private ServiceProvider _serviceProvider;

        [OneTimeSetUp]
        public async Task GlobalSetup()
        {
            LaunchSettingsReader.Read();

            _rabbitMqSettings = RabbitMqSettingsReader.Read();
            _rabbitMqConfiguration = new RabbitMqVhostInitializer(_rabbitMqSettings);

            await _rabbitMqConfiguration.InitializeAsync();
        }

        [OneTimeTearDown]
        public async Task GlobalTeardown()
        {
            await _rabbitMqConfiguration.CleanAsync();
        }

        [SetUp]
        public void SetUp()
        {
            _testHandlerImplMock = new Mock<ITestMessageHandlerImplementation>();
            _testHandlerWithStateImplMock = new Mock<ITestMessageHandlerWithStateImplementation>();

            _testHandlerImplMock
                .Setup(x => x.HandleAsync
                (
                    It.IsAny<TestMessage>(),
                    It.IsAny<MessageHeaders>(),
                    It.IsAny<IMessagePublisher>()
                ))
                .Returns<TestMessage, MessageHeaders, IMessagePublisher>((m, h, p) => Task.FromResult(MessageHandlingResult.Success()));

            _testHandlerWithStateImplMock
                .Setup(x => x.HandleAsync
                (
                    It.IsAny<string>(),
                    It.IsAny<TestMessage>(),
                    It.IsAny<MessageHeaders>(),
                    It.IsAny<IMessagePublisher>()
                ))
                .Returns<string, TestMessage, MessageHeaders, IMessagePublisher>((s, m, h, p) => Task.FromResult(MessageHandlingResult.Success()));

            var services = new ServiceCollection();

            services.AddSingleton(LogFactory.Create().AddUnbufferedConsole());
            services.AddTransient<TestMessageHandler>();
            services.AddTransient<TestMessageHandlerWithState>();
            services.AddTransient(c => _testHandlerImplMock.Object);
            services.AddTransient(c => _testHandlerWithStateImplMock.Object);
            
            _serviceProvider = services.BuildServiceProvider();
        }

        [TearDown]
        public void TearDown()
        {
            _serviceProvider.Dispose();
        }

        [Test]
        public void Test_that_message_handler_dependencies_disposed_after_message_processing()
        {
            // Arrange

            using (var disposalEvent = new ManualResetEventSlim())
            {
                // ReSharper disable once AccessToDisposedClosure
                _testHandlerImplMock
                    .Setup(x => x.Dispose())
                    .Callback(() => { disposalEvent.Set(); });

                using (var endpoint = InitializeRabbitMqEndpoint(_serviceProvider))
                {
                    var subscriptionsRegistry = new MessageSubscriptionsRegistry()
                        .Handle<TestMessage>(o => o.WithHandler<TestMessageHandler>());

                    endpoint.Subscribe(subscriptionsRegistry, ExchangeName, RouteName);
                    endpoint.StartListening();

                    var publisher = endpoint.CreatePublisher(ExchangeName);

                    var messageId = Guid.NewGuid();

                    // Act

                    publisher.Publish(new TestMessage
                    {
                        Id = messageId
                    });

                    disposalEvent.Wait(Waiting.Timeout);

                    // Assert

                    Assert.True(disposalEvent.IsSet, "Disposable dependency should be disposed after message processing");

                    _testHandlerImplMock.Verify
                    (
                        x => x.HandleAsync
                        (
                            It.Is<TestMessage>(m => m.Id == messageId),
                            It.IsNotNull<MessageHeaders>(),
                            It.IsNotNull<IMessagePublisher>()
                        ),
                        Times.Once
                    );
                }
            }
        }

        [Test]
        public void Test_that_message_handler_with_state_dependencies_disposed_after_message_processing()
        {
            // Arrange

            using (var disposalEvent = new ManualResetEventSlim())
            {              
                // ReSharper disable once AccessToDisposedClosure
                _testHandlerWithStateImplMock
                    .Setup(x => x.Dispose())
                    .Callback(() => { disposalEvent.Set(); });

                using (var endpoint = InitializeRabbitMqEndpoint(_serviceProvider))
                {
                    const string userState = "123";

                    var subscriptionsRegistry = new MessageSubscriptionsRegistry()
                        .Handle<TestMessage, string>(o =>
                        {
                            o.WithHandler<TestMessageHandlerWithState>();
                            o.WithState(userState);
                        });

                    endpoint.Subscribe(subscriptionsRegistry, ExchangeName, RouteName);
                    endpoint.StartListening();

                    var publisher = endpoint.CreatePublisher(ExchangeName);

                    var messageId = Guid.NewGuid();

                    // Act

                    publisher.Publish(new TestMessage
                    {
                        Id = messageId
                    });

                    disposalEvent.Wait(Waiting.Timeout);

                    // Assert

                    Assert.True(disposalEvent.IsSet, "Disposable dependency should be disposed after message processing");

                    _testHandlerWithStateImplMock.Verify
                    (
                        x => x.HandleAsync
                        (
                            It.Is<string>(s => s == userState),
                            It.Is<TestMessage>(m => m.Id == messageId),
                            It.IsNotNull<MessageHeaders>(),
                            It.IsNotNull<IMessagePublisher>()
                        ),
                        times: Times.Once
                    );
                }
            }
        }

        private RabbitMqEndpoint InitializeRabbitMqEndpoint(ServiceProvider serviceProvider)
        {
            var endpoint = new RabbitMqEndpoint(
                serviceProvider,
                serviceProvider.GetRequiredService<ILogFactory>(),
                new Uri(_rabbitMqSettings.GetConnectionString()),
                _rabbitMqSettings.Vhost);

            endpoint.Initialize();
            endpoint.DeclareExchange(ExchangeName);

            return endpoint;
        }
    }
}

