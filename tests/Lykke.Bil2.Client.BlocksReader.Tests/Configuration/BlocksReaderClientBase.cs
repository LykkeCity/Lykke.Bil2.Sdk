using Lykke.Bil2.BaseTests;
using Lykke.Bil2.BaseTests.HttpMessageHandlers;
using Lykke.Bil2.Client.BlocksReader.Services;
using Lykke.Common.Log;
using Lykke.Logs;
using Lykke.Logs.Loggers.LykkeConsole;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Lykke.Bil2.Client.BlocksReader.Tests.Configuration
{
    public abstract class BlocksReaderClientBase : ClientTestBase
    {
        protected (IBlocksReaderHttpApi, 
            IBlocksReaderClient,
            IBlocksReaderApiFactory, 
            IDisposable) CreateClientApi<TStartup>(string localhost, 

            Action<BlocksReaderClientOptions> clientOptions) where TStartup : class 
        {
            IServiceCollection collection = new ServiceCollection();
            var testServer = base.CreateTestServer<TStartup>();
            var client = testServer.CreateClient();
            collection.AddSingleton<ILogFactory>(LogFactory.Create().AddConsole());
            BlocksReaderClientServiceCollectionExtensions.AddBlocksReaderClient(collection, clientOptions);
            BlocksReaderClientServiceCollectionExtensions.AddBlocksReaderHttpClient(collection,
                "http://localhost",
                new RedirectToTestHostMessageHandler(client));
            var provider = collection.BuildServiceProvider();
            var api =provider.GetRequiredService<IBlocksReaderHttpApi>();
            var client1 = provider.GetRequiredService<IBlocksReaderClient>();
            var apiFactory = provider.GetRequiredService<IBlocksReaderApiFactory>();
            var testServerDependencyWrapper = new TestServerDependencyWrapper(testServer);

            return (api, client1, apiFactory, testServerDependencyWrapper);
        }
    }
}
