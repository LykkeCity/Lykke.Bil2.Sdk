using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Lykke.Bil2.BaseTests;
using Lykke.Bil2.RabbitMq.Subscription;
using Lykke.Bil2.RabbitMq.Tests.Subscription.Mocks;
using Lykke.Common.Log;
using Lykke.Logs;
using Lykke.Logs.Loggers.LykkeConsole;
using Microsoft.Extensions.DependencyInjection;
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
        private ServiceCollection _services;

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
            _services = new ServiceCollection();

            _services.AddSingleton(LogFactory.Create().AddUnbufferedConsole());
        }

        [Test]
        public void Test_that_message_handler_dependencies_disposed_after_message_processing()
        {
            // Arrange

            _services.AddTransient<TestMessageHandler>();

            using (var disposalEvent = new ManualResetEventSlim())
            {
                var dependencyCalls = new HashSet<string>();
                
                // ReSharper disable once AccessToDisposedClosure
                _services.AddTransient(s => new DisposableDependency(dependencyCalls, disposalEvent));
                
                using (var serviceProvider = _services.BuildServiceProvider())
                using (var endpoint = InitializeRabbitMqEndpoint(serviceProvider))
                {
                    var subscriptionsRegistry = new MessageSubscriptionsRegistry()
                        .Handle<TestMessage>(o => o.WithHandler<TestMessageHandler>());

                    endpoint.Subscribe(subscriptionsRegistry, ExchangeName, RouteName);
                    endpoint.StartListening();

                    var publisher = endpoint.CreatePublisher(ExchangeName);

                    // Act

                    publisher.Publish(new TestMessage
                    {
                        Id = Guid.NewGuid()
                    });

                    disposalEvent.Wait(Waiting.Timeout);

                    // Assert

                    Assert.True(dependencyCalls.Contains(nameof(DisposableDependency.FooAsync)), "Message without state should be processed");
                    Assert.False(dependencyCalls.Contains(nameof(DisposableDependency.FooWithStateAsync)), "Message with state should be not processed");
                    Assert.True(disposalEvent.IsSet, "Disposable dependency should be disposed after message processing");
                }
            }
        }

        [Test]
        public void Test_that_message_handler_with_state_dependencies_disposed_after_message_processing()
        {
            // Arrange

            _services.AddTransient<TestMessageHandlerWithState>();

            using (var disposalEvent = new ManualResetEventSlim())
            {              
                var dependencyCalls = new HashSet<string>();

                // ReSharper disable once AccessToDisposedClosure
                _services.AddTransient(s => new DisposableDependency(dependencyCalls, disposalEvent));

                using (var serviceProvider = _services.BuildServiceProvider())
                using (var endpoint = InitializeRabbitMqEndpoint(serviceProvider))
                {
                    var subscriptionsRegistry = new MessageSubscriptionsRegistry()
                        .Handle<TestMessage, string>(o =>
                        {
                            o.WithHandler<TestMessageHandlerWithState>();
                            o.WithState("123");
                        });

                    endpoint.Subscribe(subscriptionsRegistry, ExchangeName, RouteName);
                    endpoint.StartListening();

                    var publisher = endpoint.CreatePublisher(ExchangeName);

                    // Act

                    publisher.Publish(new TestMessage
                    {
                        Id = Guid.NewGuid()
                    });

                    disposalEvent.Wait(Waiting.Timeout);

                    // Assert

                    Assert.False(dependencyCalls.Contains(nameof(DisposableDependency.FooAsync)), "Message without state should be not processed");
                    Assert.True(dependencyCalls.Contains(nameof(DisposableDependency.FooWithStateAsync)), "Message with state should be processed");
                    Assert.True(disposalEvent.IsSet, "Disposable dependency should be disposed after message processing");
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

