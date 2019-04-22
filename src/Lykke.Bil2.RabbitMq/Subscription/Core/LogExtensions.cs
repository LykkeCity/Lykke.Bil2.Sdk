using System;
using Common.Log;
using Lykke.Common.Log;

namespace Lykke.Bil2.RabbitMq.Subscription.Core
{
    internal static class LogExtensions
    {
        public static void Error(
            this ILog log,
            EnvelopedMessage context,
            string message,
            Exception exception = null)
        {
            var process = context.RoutingKey; 
            
            log.Error(process, exception, message, context);
        }
       
        public static void Warning(
            this ILog log,
            EnvelopedMessage context,
            string message,
            Exception exception = null)
        {
            var process = context.RoutingKey; 
            
            log.Warning(process, message, exception, context);
        }
        
        public static void Warning(
            this ILog log,
            EnvelopedMessage context,
            object payload,
            MessageHeaders headers,
            string message,
            Exception exception = null)
        {
            var process = context.RoutingKey; 
            
            log.Warning(process, message, exception, new { Headers = headers, Message = payload });
        }
    }
}
