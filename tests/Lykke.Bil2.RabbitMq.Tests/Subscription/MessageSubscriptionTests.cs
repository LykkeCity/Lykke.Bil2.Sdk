using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Bil2.RabbitMq.Publication;
using Lykke.Bil2.RabbitMq.Subscription;
using Lykke.Bil2.RabbitMq.Tests.Subscription.Mocks;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;

namespace Lykke.Bil2.RabbitMq.Tests.Subscription
{
    [TestFixture]
    public class MessageSubscriptionTests
    {
        private class TestException : Exception
        {
        }

        private ServiceProvider _serviceProvider;
        private Mock<IMessagePublisher> _messagePublisherMock;
        private Mock<ITestMessageHandlerImplementation> _testHandlerImplMock;
        private Mock<ITestMessageHandlerWithStateImplementation> _testHandlerWithStateImplMock;

        [SetUp]
        public void SetUp()
        {
            _testHandlerImplMock = new Mock<ITestMessageHandlerImplementation>();
            _testHandlerWithStateImplMock = new Mock<ITestMessageHandlerWithStateImplementation>();
            _messagePublisherMock = new Mock<IMessagePublisher>();

            var services = new ServiceCollection();

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

        #region Invocation without filters

        [Test]
        public async Task Test_that_handler_invoked_without_filters()
        {
            // Arrange

            var messageId = Guid.NewGuid();
            const string correlationId = "correlationId";
            const string exchange = "exchange";
            const string routingKey = "routingKey";
            const int retryCount = 123;

            var options = new MessageSubscriptionOptions<TestMessage>(routingKey);

            options.WithHandler<TestMessageHandler>();

            var subscription = new MessageSubscription<TestMessage>(options, Array.Empty<IMessageFilter>());

            _testHandlerImplMock
                .Setup(x => x.HandleAsync
                (
                    It.IsAny<TestMessage>(),
                    It.IsAny<MessageHeaders>(),
                    It.IsAny<IMessagePublisher>()
                ))
                .Returns<TestMessage, MessageHeaders, IMessagePublisher>((m, h, p) => Task.FromResult(MessageHandlingResult.Success()));
            
            // Action

            var result = await subscription.InvokeHandlerAsync
            (
                _serviceProvider,
                new TestMessage
                {
                    Id = messageId
                },
                new MessageHeaders(correlationId, DateTime.UtcNow),
                new MessageHandlingContext(exchange, retryCount, routingKey),
                _messagePublisherMock.Object
            );

            // Assert

            Assert.AreEqual(MessageHandlingResult.Success(), result);

            _testHandlerImplMock.Verify
            (
                x => x.HandleAsync
                (
                    It.Is<TestMessage>(m => m.Id == messageId),
                    It.IsNotNull<MessageHeaders>(),
                    It.IsNotNull<IMessagePublisher>()
                ),
                times: Times.Once
            );
        }

        [Test]
        public async Task Test_that_handler_with_state_invoked_without_filters()
        {
            // Arrange

            var messageId = Guid.NewGuid();
            const string state = "state";
            const string correlationId = "correlationId";
            const string exchange = "exchange";
            const string routingKey = "routingKey";
            const int retryCount = 123;

            var options = new MessageSubscriptionOptions<TestMessage, string>(routingKey);

            options.WithHandler<TestMessageHandlerWithState>();
            options.WithState(state);

            var subscription = new MessageSubscription<TestMessage, string>(options, Array.Empty<IMessageFilter>());
            
            _testHandlerWithStateImplMock
                .Setup(x => x.HandleAsync
                (
                    It.IsAny<string>(),
                    It.IsAny<TestMessage>(),
                    It.IsAny<MessageHeaders>(),
                    It.IsAny<IMessagePublisher>()
                ))
                .Returns<string, TestMessage, MessageHeaders, IMessagePublisher>((s, m, h, p) => Task.FromResult(MessageHandlingResult.Success()));

            // Action

            var result = await subscription.InvokeHandlerAsync
            (
                _serviceProvider,
                new TestMessage
                {
                    Id = messageId
                },
                new MessageHeaders(correlationId, DateTime.UtcNow),
                new MessageHandlingContext(exchange, retryCount, routingKey),
                _messagePublisherMock.Object
            );

            // Assert

            Assert.AreEqual(MessageHandlingResult.Success(), result);

            _testHandlerWithStateImplMock.Verify
            (
                x => x.HandleAsync
                (
                    It.Is<string>(s => s == state),
                    It.Is<TestMessage>(m => m.Id == messageId),
                    It.IsNotNull<MessageHeaders>(),
                    It.IsNotNull<IMessagePublisher>()
                ),
                times: Times.Once
            );
        }

        #endregion


        #region Invocation with single filter

        [Test]
        public async Task Test_that_handler_invoked_with_single_filter()
        {           
            // Arrange

            var messageId = Guid.NewGuid();
            const string correlationId = "correlationId";
            const string exchange = "exchange";
            const string routingKey = "routingKey";
            const int retryCount = 123;

            var options = new MessageSubscriptionOptions<TestMessage>(routingKey);

            options.WithHandler<TestMessageHandler>();

            var filterMock = new Mock<IMessageFilter>();

            filterMock
                .Setup(x => x.HandleMessageAsync(It.IsNotNull<MessageFilteringContext>()))
                .Returns<MessageFilteringContext>(context => context.InvokeNextAsync());

            var globalFilters = new List<IMessageFilter>
            {
                filterMock.Object
            };

            var subscription = new MessageSubscription<TestMessage>(options, globalFilters);

            _testHandlerImplMock
                .Setup(x => x.HandleAsync
                (
                    It.IsAny<TestMessage>(),
                    It.IsAny<MessageHeaders>(),
                    It.IsAny<IMessagePublisher>()
                ))
                .Returns<TestMessage, MessageHeaders, IMessagePublisher>((m, h, p) => Task.FromResult(MessageHandlingResult.Success()));
            
            // Action

            var result = await subscription.InvokeHandlerAsync
            (
                _serviceProvider,
                new TestMessage
                {
                    Id = messageId
                },
                new MessageHeaders(correlationId, DateTime.UtcNow),
                new MessageHandlingContext(exchange, retryCount, routingKey),
                _messagePublisherMock.Object
            );

            // Assert

            Assert.AreEqual(MessageHandlingResult.Success(), result);

            _testHandlerImplMock.Verify
            (
                x => x.HandleAsync
                (
                    It.Is<TestMessage>(m => m.Id == messageId),
                    It.IsNotNull<MessageHeaders>(),
                    It.IsNotNull<IMessagePublisher>()
                ),
                times: Times.Once
            );
            filterMock.Verify(x => x.HandleMessageAsync(It.IsNotNull<MessageFilteringContext>()), Times.Once());
        }

        [Test]
        public async Task Test_that_handler_with_state_invoked_with_single_filter()
        {           
            // Arrange

            var messageId = Guid.NewGuid();
            const string state = "state";
            const string correlationId = "correlationId";
            const string exchange = "exchange";
            const string routingKey = "routingKey";
            const int retryCount = 123;

            var options = new MessageSubscriptionOptions<TestMessage, string>(routingKey);

            options.WithHandler<TestMessageHandlerWithState>();
            options.WithState(state);

            var filterMock = new Mock<IMessageFilter>();

            filterMock
                .Setup(x => x.HandleMessageAsync(It.IsNotNull<MessageFilteringContext>()))
                .Returns<MessageFilteringContext>(context => context.InvokeNextAsync());

            var globalFilters = new List<IMessageFilter>
            {
                filterMock.Object
            };

            var subscription = new MessageSubscription<TestMessage, string>(options, globalFilters);
            
            _testHandlerWithStateImplMock
                .Setup(x => x.HandleAsync
                (
                    It.IsAny<string>(),
                    It.IsAny<TestMessage>(),
                    It.IsAny<MessageHeaders>(),
                    It.IsAny<IMessagePublisher>()
                ))
                .Returns<string, TestMessage, MessageHeaders, IMessagePublisher>((s, m, h, p) => Task.FromResult(MessageHandlingResult.Success()));

            // Action

            var result = await subscription.InvokeHandlerAsync
            (
                _serviceProvider,
                new TestMessage
                {
                    Id = messageId
                },
                new MessageHeaders(correlationId, DateTime.UtcNow),
                new MessageHandlingContext(exchange, retryCount, routingKey),
                _messagePublisherMock.Object
            );

            // Assert

            Assert.AreEqual(MessageHandlingResult.Success(), result);

            _testHandlerWithStateImplMock.Verify
            (
                x => x.HandleAsync
                (
                    It.Is<string>(s => s == state),
                    It.Is<TestMessage>(m => m.Id == messageId),
                    It.IsNotNull<MessageHeaders>(),
                    It.IsNotNull<IMessagePublisher>()
                ),
                times: Times.Once
            );
            filterMock.Verify(x => x.HandleMessageAsync(It.IsNotNull<MessageFilteringContext>()), Times.Once());
        }

        #endregion


        #region Invocation with two filters

        [Test]
        public async Task Test_that_handler_invoked_with_two_filter()
        {
            // Arrange

            var messageId = Guid.NewGuid();
            const string correlationId = "correlationId";
            const string exchange = "exchange";
            const string routingKey = "routingKey";
            const int retryCount = 123;

            var options = new MessageSubscriptionOptions<TestMessage>(routingKey);

            options.WithHandler<TestMessageHandler>();

            var filterMock = new Mock<IMessageFilter>();

            filterMock
                .Setup(x => x.HandleMessageAsync(It.IsNotNull<MessageFilteringContext>()))
                .Returns<MessageFilteringContext>(context => context.InvokeNextAsync());

            var globalFilters = new List<IMessageFilter>
            {
                filterMock.Object,
                filterMock.Object
            };

            var subscription = new MessageSubscription<TestMessage>(options, globalFilters);
            
            _testHandlerImplMock
                .Setup(x => x.HandleAsync
                (
                    It.IsAny<TestMessage>(),
                    It.IsAny<MessageHeaders>(),
                    It.IsAny<IMessagePublisher>()
                ))
                .Returns<TestMessage, MessageHeaders, IMessagePublisher>((m, h, p) => Task.FromResult(MessageHandlingResult.Success()));

            // Action

            var result = await subscription.InvokeHandlerAsync
            (
                _serviceProvider,
                new TestMessage
                {
                    Id = messageId
                },
                new MessageHeaders(correlationId, DateTime.UtcNow),
                new MessageHandlingContext(exchange, retryCount, routingKey),
                _messagePublisherMock.Object
            );

            // Assert

            Assert.AreEqual(MessageHandlingResult.Success(), result);

            _testHandlerImplMock.Verify
            (
                x => x.HandleAsync
                (
                    It.Is<TestMessage>(m => m.Id == messageId),
                    It.IsNotNull<MessageHeaders>(),
                    It.IsNotNull<IMessagePublisher>()
                ),
                times: Times.Once
            );
            filterMock.Verify(x => x.HandleMessageAsync(It.IsNotNull<MessageFilteringContext>()), Times.Exactly(2));
        }

        [Test]
        public async Task Test_that_handler_with_state_invoked_with_two_filter()
        {           
            // Arrange

            var messageId = Guid.NewGuid();
            const string state = "state";
            const string correlationId = "correlationId";
            const string exchange = "exchange";
            const string routingKey = "routingKey";
            const int retryCount = 123;

            var options = new MessageSubscriptionOptions<TestMessage, string>(routingKey);

            options.WithHandler<TestMessageHandlerWithState>();
            options.WithState(state);

            var filterMock = new Mock<IMessageFilter>();

            filterMock
                .Setup(x => x.HandleMessageAsync(It.IsNotNull<MessageFilteringContext>()))
                .Returns<MessageFilteringContext>(context => context.InvokeNextAsync());

            var globalFilters = new List<IMessageFilter>
            {
                filterMock.Object,
                filterMock.Object
            };

            var subscription = new MessageSubscription<TestMessage, string>(options, globalFilters);

            _testHandlerWithStateImplMock
                .Setup(x => x.HandleAsync
                (
                    It.IsAny<string>(),
                    It.IsAny<TestMessage>(),
                    It.IsAny<MessageHeaders>(),
                    It.IsAny<IMessagePublisher>()
                ))
                .Returns<string, TestMessage, MessageHeaders, IMessagePublisher>((s, m, h, p) => Task.FromResult(MessageHandlingResult.Success()));
            
            // Action

            var result = await subscription.InvokeHandlerAsync
            (
                _serviceProvider,
                new TestMessage
                {
                    Id = messageId
                },
                new MessageHeaders(correlationId, DateTime.UtcNow),
                new MessageHandlingContext(exchange, retryCount, routingKey),
                _messagePublisherMock.Object
            );

            // Assert

            Assert.AreEqual(MessageHandlingResult.Success(), result);

            _testHandlerWithStateImplMock.Verify
            (
                x => x.HandleAsync
                (
                    It.Is<string>(s => s == state),
                    It.Is<TestMessage>(m => m.Id == messageId),
                    It.IsNotNull<MessageHeaders>(),
                    It.IsNotNull<IMessagePublisher>()
                ),
                times: Times.Once
            );
            filterMock.Verify(x => x.HandleMessageAsync(It.IsNotNull<MessageFilteringContext>()), Times.Exactly(2));
        }

        #endregion


        #region Invocation chain iterruption

        [Test]
        public async Task Test_that_filter_can_interrupt_invocation_chain()
        {
            // Arrange

            var messageId = Guid.NewGuid();
            const string correlationId = "correlationId";
            const string exchange = "exchange";
            const string routingKey = "routingKey";
            const int retryCount = 123;

            var options = new MessageSubscriptionOptions<TestMessage>(routingKey);

            options.WithHandler<TestMessageHandler>();

            var filterMock = new Mock<IMessageFilter>();

            filterMock
                .Setup(x => x.HandleMessageAsync(It.IsNotNull<MessageFilteringContext>()))
                .Returns<MessageFilteringContext>(context => Task.FromResult(MessageHandlingResult.TransientFailure()));

            var globalFilters = new List<IMessageFilter>
            {
                filterMock.Object
            };

            var subscription = new MessageSubscription<TestMessage>(options, globalFilters);
            
            // Action

            var result = await subscription.InvokeHandlerAsync
            (
                _serviceProvider,
                new TestMessage
                {
                    Id = messageId
                },
                new MessageHeaders(correlationId, DateTime.UtcNow),
                new MessageHandlingContext(exchange, retryCount, routingKey),
                _messagePublisherMock.Object
            );

            // Assert

            Assert.AreEqual(MessageHandlingResult.TransientFailure(), result);

            _testHandlerImplMock.Verify
            (
                x => x.HandleAsync
                (
                    It.IsAny<TestMessage>(),
                    It.IsAny<MessageHeaders>(),
                    It.IsAny<IMessagePublisher>()
                ),
                times: Times.Never
            );
            filterMock.Verify(x => x.HandleMessageAsync(It.IsNotNull<MessageFilteringContext>()), Times.Once());
        }

        [Test]
        public async Task Test_that_filter_can_interrupt_invocation_chain_with_state()
        {
            // Arrange

            var messageId = Guid.NewGuid();
            const string state = "state";
            const string correlationId = "correlationId";
            const string exchange = "exchange";
            const string routingKey = "routingKey";
            const int retryCount = 123;

            var options = new MessageSubscriptionOptions<TestMessage, string>(routingKey);

            options.WithHandler<TestMessageHandlerWithState>();
            options.WithState(state);

            var filterMock = new Mock<IMessageFilter>();

            filterMock
                .Setup(x => x.HandleMessageAsync(It.IsNotNull<MessageFilteringContext>()))
                .Returns<MessageFilteringContext>(context => Task.FromResult(MessageHandlingResult.TransientFailure()));

            var globalFilters = new List<IMessageFilter>
            {
                filterMock.Object
            };

            var subscription = new MessageSubscription<TestMessage, string>(options, globalFilters);
            
            // Action

            var result = await subscription.InvokeHandlerAsync
            (
                _serviceProvider,
                new TestMessage
                {
                    Id = messageId
                },
                new MessageHeaders(correlationId, DateTime.UtcNow),
                new MessageHandlingContext(exchange, retryCount, routingKey),
                _messagePublisherMock.Object
            );

            // Assert

            Assert.AreEqual(MessageHandlingResult.TransientFailure(), result);

            _testHandlerWithStateImplMock.Verify
            (
                x => x.HandleAsync
                (
                    It.IsAny<string>(),
                    It.IsAny<TestMessage>(),
                    It.IsAny<MessageHeaders>(),
                    It.IsAny<IMessagePublisher>()
                ),
                times: Times.Never
            );
            filterMock.Verify(x => x.HandleMessageAsync(It.IsNotNull<MessageFilteringContext>()), Times.Once());
        }

        #endregion


        #region Exception propagation

        [Test]
        public void Test_that_exception_propagated_through_the_filters()
        {
            // Arrange

            var messageId = Guid.NewGuid();
            const string correlationId = "correlationId";
            const string exchange = "exchange";
            const string routingKey = "routingKey";
            const int retryCount = 123;

            var options = new MessageSubscriptionOptions<TestMessage>(routingKey);

            options.WithHandler<TestMessageHandler>();

            var filterMock = new Mock<IMessageFilter>();
            Exception caughtException = null;

            filterMock
                .Setup(x => x.HandleMessageAsync(It.IsNotNull<MessageFilteringContext>()))
                .Returns<MessageFilteringContext>(async context =>
                {
                    try
                    {
                        return await context.InvokeNextAsync();
                    }
                    catch (Exception ex)
                    {
                        caughtException = ex;
                        throw;
                    }
                });

            _testHandlerImplMock
                .Setup(x => x.HandleAsync
                (
                    It.IsAny<TestMessage>(),
                    It.IsAny<MessageHeaders>(),
                    It.IsAny<IMessagePublisher>()
                ))
                .Returns<TestMessage, MessageHeaders, IMessagePublisher>((m, h, p) => throw new TestException());

            var globalFilters = new List<IMessageFilter>
            {
                filterMock.Object
            };

            var subscription = new MessageSubscription<TestMessage>(options, globalFilters);
            
            // Action

            Assert.ThrowsAsync<TestException>(async () =>
            {
                await subscription.InvokeHandlerAsync
                (
                    _serviceProvider,
                    new TestMessage
                    {
                        Id = messageId
                    },
                    new MessageHeaders(correlationId, DateTime.UtcNow),
                    new MessageHandlingContext(exchange, retryCount, routingKey),
                    _messagePublisherMock.Object
                );
            });

            // Assert

            Assert.IsInstanceOf<TestException>(caughtException);

            _testHandlerImplMock.Verify
            (
                x => x.HandleAsync
                (
                    It.IsAny<TestMessage>(),
                    It.IsAny<MessageHeaders>(),
                    It.IsAny<IMessagePublisher>()
                ),
                times: Times.Once
            );
            filterMock.Verify(x => x.HandleMessageAsync(It.IsNotNull<MessageFilteringContext>()), Times.Once());
        }

        [Test]
        public void Test_that_exception_propagated_through_the_filters_with_state()
        {
            // Arrange

            var messageId = Guid.NewGuid();
            const string state = "state";
            const string correlationId = "correlationId";
            const string exchange = "exchange";
            const string routingKey = "routingKey";
            const int retryCount = 123;

            var options = new MessageSubscriptionOptions<TestMessage, string>(routingKey);

            options.WithHandler<TestMessageHandlerWithState>();
            options.WithState(state);

            var filterMock = new Mock<IMessageFilter>();
            Exception caughtException = null;

            filterMock
                .Setup(x => x.HandleMessageAsync(It.IsNotNull<MessageFilteringContext>()))
                .Returns<MessageFilteringContext>(async context =>
                {
                    try
                    {
                        return await context.InvokeNextAsync();
                    }
                    catch (Exception ex)
                    {
                        caughtException = ex;
                        throw;
                    }
                });

            _testHandlerWithStateImplMock
                .Setup(x => x.HandleAsync
                (
                    It.IsAny<string>(),
                    It.IsAny<TestMessage>(),
                    It.IsAny<MessageHeaders>(),
                    It.IsAny<IMessagePublisher>()
                ))
                .Returns<string, TestMessage, MessageHeaders, IMessagePublisher>((s, m, h, p) => throw new TestException());

            var globalFilters = new List<IMessageFilter>
            {
                filterMock.Object
            };

            var subscription = new MessageSubscription<TestMessage, string>(options, globalFilters);
            
            // Action

            Assert.ThrowsAsync<TestException>(async () =>
            {
                await subscription.InvokeHandlerAsync
                (
                    _serviceProvider,
                    new TestMessage
                    {
                        Id = messageId
                    },
                    new MessageHeaders(correlationId, DateTime.UtcNow),
                    new MessageHandlingContext(exchange, retryCount, routingKey),
                    _messagePublisherMock.Object
                );
            });

            // Assert

            Assert.IsInstanceOf<TestException>(caughtException);

            _testHandlerWithStateImplMock.Verify
            (
                x => x.HandleAsync
                (
                    It.IsAny<string>(),
                    It.IsAny<TestMessage>(),
                    It.IsAny<MessageHeaders>(),
                    It.IsAny<IMessagePublisher>()
                ),
                times: Times.Once
            );
            filterMock.Verify(x => x.HandleMessageAsync(It.IsNotNull<MessageFilteringContext>()), Times.Once());
        }

        #endregion
    }
}
