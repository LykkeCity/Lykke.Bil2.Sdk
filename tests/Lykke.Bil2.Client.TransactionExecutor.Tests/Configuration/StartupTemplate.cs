using System;
using JetBrains.Annotations;
using Lykke.Bil2.Sdk.SignService;
using Lykke.Bil2.Sdk.TransactionsExecutor;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.Bil2.Client.TransactionExecutor.Tests.Configuration
{
    [UsedImplicitly]
    //Startup class is used in configuration of web api 
    public class StartupTemplate
    {
        private const string IntegrationName = "Test";

        [UsedImplicitly]
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            var optionsConfiguration =
                StartupDependencyFactorySingleton.Instance.GetOptionsConfiguration<AppSettings>();
            return services.BuildBlockchainTransactionsExecutorServiceProvider<AppSettings>(optionsConfiguration);
        }

        [UsedImplicitly]
        public void Configure(IApplicationBuilder app)
        {
            app.UseBlockchainSignService(options =>
            {
                options.IntegrationName = IntegrationName;
            });
        }
    }
}
