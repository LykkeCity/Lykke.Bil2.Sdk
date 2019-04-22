using System;
using System.Threading.Tasks;
using Lykke.Common.Log;
using Microsoft.Extensions.Logging;

namespace Lykke.Bil2.RabbitMq.Subscription.MessageFilters
{
    public class TraceMessageFilter : IMessageFilter
    {
        private readonly ILogFactory _logFactory;
        private readonly LogLevel _logLevel;

        public TraceMessageFilter(
            ILogFactory logFactory,
            LogLevel logLevel)
        {
            _logFactory = logFactory;
            _logLevel = logLevel;
        }

        public Task<MessageHandlingResult> HandleMessageAsync(MessageFilteringContext context)
        {
            const string text = "Message being handled";

            var process = context.HandlingContext.RoutingKey;
            var log = _logFactory.CreateLog(this, context.HandlingContext.Exchange);

            switch (_logLevel)
            {
                case LogLevel.Trace:
                    log.Trace(process, text, new
                    {
                        Headers = context.Headers, 
                        Message = context.Message,
                        FirstLevelRetryNumber = context.HandlingContext.RetryCount
                    });
                    break;

                case LogLevel.Debug:
                    log.Debug(process, text, new
                    {
                        Headers = context.Headers, 
                        Message = context.Message,
                        FirstLevelRetryNumber = context.HandlingContext.RetryCount
                    });
                    break;

                case LogLevel.Information:
                    log.Info(process, text, new
                    {
                        Headers = context.Headers, 
                        Message = context.Message,
                        FirstLevelRetryNumber = context.HandlingContext.RetryCount
                    });
                    break;

                case LogLevel.Warning:
                    log.Warning(process, text, null, new
                    {
                        Headers = context.Headers, 
                        Message = context.Message,
                        FirstLevelRetryNumber = context.HandlingContext.RetryCount
                    });
                    break;

                case LogLevel.Error:
                    log.Warning(process, text, null, new
                    {
                        Headers = context.Headers, 
                        Message = context.Message,
                        FirstLevelRetryNumber = context.HandlingContext.RetryCount
                    });
                    break;
                    
                case LogLevel.Critical:
                    log.Warning(process, "Message being handled", null, new
                    {
                        Headers = context.Headers, 
                        Message = context.Message,
                        FirstLevelRetryNumber = context.HandlingContext.RetryCount
                    });
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(_logLevel), _logLevel, string.Empty);
            }
            
            return context.InvokeNextAsync();
        }
    }
}
