using System;
using JetBrains.Annotations;
using Lykke.Bil2.Sdk.TransactionsExecutor;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using TransactionsExecutorExample.Services;
using TransactionsExecutorExample.Settings;

namespace TransactionsExecutorExample
{
    [UsedImplicitly]
    public class Startup
    {
        private const string IntegrationName = "Example";

        [UsedImplicitly]
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            return services.BuildBlockchainTransactionsExecutorServiceProvider<AppSettings>(options =>
            {
                options.IntegrationName = IntegrationName;
                
                options.AddressValidatorFactory = c => new AddressValidator();
                options.HealthProviderFactory = c => new HealthProvider
                (
                    c.Services.GetRequiredService<INodeClient>()
                );
                options.IntegrationInfoServiceFactory = c => new IntegrationInfoService();
                options.TransferAmountTransactionEstimatorFactory = c => new TransactionsEstimator();
                options.TransferAmountTransactionsBuilderFactory = c => new TransferTransactionsBuilder();
                options.TransactionBroadcasterFactory = c => new TransactionsBroadcaster();
                options.AddressFormatsProviderFactory = c => new AddressFormatsProvider();

                options.UseSettings = (s, settings) =>
                {
                    s.AddSingleton<INodeClient>(new NodeClient(settings.CurrentValue.NodeUrl));
                };
            });
        }

        [UsedImplicitly]
        public void Configure(IApplicationBuilder app)
        {
            app.UseBlockchainTransactionsExecutor(options =>
            {
                options.IntegrationName = IntegrationName;
            });
        }
    }
}
