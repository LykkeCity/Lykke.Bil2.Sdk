using JetBrains.Annotations;
using Lykke.Bil2.Sdk.BlocksReader;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Lykke.Bil2.Client.BlocksReader.Tests.Configuration
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
            StartupDependencyFactorySingleton.Instance.ServerServiceProvider = 
                services.BuildBlockchainBlocksReaderServiceProvider(optionsConfiguration);

            return StartupDependencyFactorySingleton.Instance.ServerServiceProvider;
        }

        [UsedImplicitly]
        public void Configure(IApplicationBuilder app)
        {
            app.UseBlockchainBlocksReader(options =>
            {
                options.IntegrationName = IntegrationName;
            });
        }
    }
}
