using System;
using BlocksReaderExample.Services;
using BlocksReaderExample.Settings;
using JetBrains.Annotations;
using Lykke.Bil2.Sdk.BlocksReader;
using Lykke.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace BlocksReaderExample
{
    [UsedImplicitly]
    public class Startup
    {
        [UsedImplicitly]
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            return services.BuildBlockchainBlocksReaderServiceProvider<AppSettings>(options =>
            {
                options.IntegrationName = Program.IntegrationName;

                options.BlockReaderFactory = c => new BlockReader();
                options.AddIrreversibleBlockPulling(c => new IrreversibleBlockProvider());
                options.RabbitVhost = AppEnvironment.EnvInfo;

                options.UseSettings = (s, settings) =>
                {
                    services.AddSingleton<INodeClient>(new NodeClient(settings.CurrentValue.NodeUrl));
                };
            });
        }

        [UsedImplicitly]
        public void Configure(IApplicationBuilder app)
        {
            app.UseBlockchainBlocksReader(options =>
            {
                options.IntegrationName = Program.IntegrationName;
            });
        }
    }
}
