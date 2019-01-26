using System;
using JetBrains.Annotations;
using Lykke.Blockchains.Integrations.Sdk.SignService;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using SignServiceExample.Services;
using SignServiceExample.Settings;

namespace SignServiceExample
{
    [UsedImplicitly]
    public class Startup
    {
        private const string IntegrationName = "Example";

        [UsedImplicitly]
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            return services.BuildBlockchainSignServiceProvider<AppSettings>(options =>
            {
                options.IntegrationName = IntegrationName;
                options.TransactionSignerFactory = c => new TransactionSigner(c.Settings.CurrentValue.Network);
                options.AddressGeneratorFactory = c => new AddressGenerator(c.Settings.CurrentValue.Network);
            });
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
