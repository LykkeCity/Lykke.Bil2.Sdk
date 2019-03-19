using Lykke.Bil2.BaseTests;
using Lykke.Bil2.BaseTests.HttpMessageHandlers;
using Lykke.Bil2.Client.BlocksReader.Services;
using Lykke.Logs;
using Lykke.Logs.Loggers.LykkeConsole;
using Microsoft.Extensions.DependencyInjection;
using System;
using Lykke.Bil2.Client.BlocksReader.Options;

namespace Lykke.Bil2.Client.BlocksReader.Tests.Configuration
{
    public abstract class BlocksReaderClientBase : ClientTestBase
    {
        protected (IBlocksReaderClient,
            IBlocksReaderApiFactory,
            IDisposable)
            CreateClientApi<TStartup>(string integrationName, Action<BlocksReaderClientOptions> clientOptions)

            where TStartup : class
        {
            IServiceCollection collection = new ServiceCollection();
            var testServer = CreateTestServer<TStartup>();
            var client = testServer.CreateClient();

            collection.AddSingleton(LogFactory.Create().AddConsole());
            collection.AddBlocksReaderClient(clientOptions);
            collection.AddBlocksReaderWebClient(options =>
            {
                options.AddDelegatingHandler(new RedirectToTestHostMessageHandler(client));
                options.AddIntegration(integrationName, integrationOptions =>
                {
                    integrationOptions.Url = "http://localhost";
                });
            });

            var provider = collection.BuildServiceProvider();
            var blocksReaderClient = provider.GetRequiredService<IBlocksReaderClient>();
            var apiFactory = provider.GetRequiredService<IBlocksReaderApiFactory>();
            var testServerDependencyWrapper = new TestServerDependencyWrapper(testServer);

            return (blocksReaderClient, apiFactory, testServerDependencyWrapper);
        }
    }
}
