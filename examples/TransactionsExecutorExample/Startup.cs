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
                options.AddressValidatorFactory = s => new AddressValidator();
                options.HealthProviderFactory = s => new HealthProvider();
                options.IntegrationInfoServiceFactory = s => new IntegrationInfoService();
                options.TransactionEstimatorFactory = s => new TransactionsEstimator();
                options.TransactionExecutorFactory = s => new TransactionsExecutor();
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
