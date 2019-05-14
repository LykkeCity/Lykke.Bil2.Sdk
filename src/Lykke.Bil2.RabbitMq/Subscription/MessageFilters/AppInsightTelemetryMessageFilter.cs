using System;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;

namespace Lykke.Bil2.RabbitMq.Subscription.MessageFilters
{
    public class AppInsightTelemetryMessageFilter : IMessageFilter
    {
        private static readonly TelemetryClient TelemetryClient = new TelemetryClient();

        public async Task<MessageHandlingResult> HandleMessageAsync(MessageFilteringContext context)
        {
            var operation = TelemetryClient.StartOperation<RequestTelemetry>
            (
                $"Message processing: {context.HandlingContext.RoutingKey}",
                context.Headers.CorrelationId
            );

            try
            {
                return await context.InvokeNextAsync();
            }
            catch (Exception ex)
            {
                operation.Telemetry.Success = false;
                TelemetryClient.TrackException(ex);

                throw;
            }
            finally
            {
                TelemetryClient.StopOperation(operation);
            }
        }
    }
}
