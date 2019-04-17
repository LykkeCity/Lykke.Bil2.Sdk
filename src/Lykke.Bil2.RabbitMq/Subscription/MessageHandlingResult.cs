using System;

namespace Lykke.Bil2.RabbitMq.Subscription
{
    public abstract class MessageHandlingResult
    {
        private static readonly NonTransientErrorResult NonTransientErrorResultInstance
            = new NonTransientErrorResult();
        
        private static readonly SuccessResult SuccessResultInstance
            = new SuccessResult();
        
        private static readonly TransientFailureResult TransientFailureResultInstance
            = new TransientFailureResult();
        
        
        
        public static MessageHandlingResult NonTransientError()
            => NonTransientErrorResultInstance;
        
        public static MessageHandlingResult Success()
            => SuccessResultInstance;
        
        public static MessageHandlingResult TransientFailure()
            => TransientFailureResultInstance;
        
        public static MessageHandlingResult TransientFailure(TimeSpan retryAfter)
            => new TransientFailureResult(retryAfter);
        

        public class NonTransientErrorResult : MessageHandlingResult
        {
            internal NonTransientErrorResult()
            {
                
            }
        }

        public class SuccessResult : MessageHandlingResult
        {
            internal SuccessResult()
            {
                
            }
        }
        
        public class TransientFailureResult : MessageHandlingResult
        {
            internal TransientFailureResult()
            {
                
            }

            public TransientFailureResult(TimeSpan retryAfter)
            {
                RetryAfter = retryAfter;
            }
            
            public TimeSpan? RetryAfter { get; }
        }
    }
}
