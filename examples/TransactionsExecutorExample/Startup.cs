using System;
using JetBrains.Annotations;
using Lykke.Blockchains.Integrations.Sdk.TransactionsExecutor;
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
                options.IntegrationInfoServiceFactory = c => new IntegrationInfoService(c.Settings.CurrentValue);
                options.TransactionEstimatorFactory = c => new TransactionsEstimator();
                options.TransactionExecutorFactory = c => new TransactionsExecutor();

                options.UseSettings = settings =>
                {
                    services.AddSingleton<INodeClient>(new NodeClient(settings.CurrentValue.NodeUrl));
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
