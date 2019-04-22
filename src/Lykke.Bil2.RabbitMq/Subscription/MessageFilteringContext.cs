using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Lykke.Bil2.RabbitMq.Subscription
{
    [PublicAPI]
    public class MessageFilteringContext
    {
        /// <summary>
        /// User's state.
        /// </summary>
        public object State { get; }

        /// <summary>
        /// The message
        /// </summary>
        public object Message { get; }

        /// <summary>
        /// Message headers.
        /// </summary>
        public MessageHeaders Headers { get; }

        /// <summary>
        /// Handling context.
        /// </summary>
        public MessageHandlingContext HandlingContext { get; }

        private readonly Func<Task<MessageHandlingResult>> _nativeHandler;
        private readonly IEnumerator<IMessageFilter> _filtersEnumerator;

        internal MessageFilteringContext(
            IEnumerator<IMessageFilter> filtersEnumerator, 
            object state, 
            object message,
            MessageHeaders headers,
            MessageHandlingContext handlingContext,
            Func<Task<MessageHandlingResult>> nativeHandler)
        {
            State = state;
            Message = message;
            Headers = headers;
            HandlingContext = handlingContext;
            _nativeHandler = nativeHandler;
            _filtersEnumerator = filtersEnumerator;
        }

        /// <summary>
        /// Invokes next filter in the chain or actual message handler if there are no more filters.
        /// </summary>
        public Task<MessageHandlingResult> InvokeNextAsync()
        {
            if (_filtersEnumerator.MoveNext())
            {
                return _filtersEnumerator.Current.HandleMessageAsync(this);
            }

            return _nativeHandler.Invoke();
        }
    }
}
