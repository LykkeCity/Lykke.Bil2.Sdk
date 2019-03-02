using System;
using Lykke.Bil2.BaseTests;
using Lykke.Bil2.BaseTests.HttpMessageHandlers;
using Lykke.Bil2.Client.BlocksReader.Services;
using Lykke.Common.Log;
using Lykke.Logs;
using Lykke.Logs.Loggers.LykkeConsole;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.Bil2.Client.BlocksReader.Tests.Configuration
{
    public abstract class BlocksReaderClientBase : ClientTestBase
    {
        protected (IBlocksReaderApi, IBlocksReaderClient) CreateClientApi<TStartup>(string localhost, 
            Action<BlocksReaderClientOptions> clientOptions) where TStartup : class 
        {
            IServiceCollection collection = new ServiceCollection();
            var testServer = base.CreateTestServer<TStartup>();
            var client = testServer.CreateClient();
            collection.AddSingleton<ILogFactory>(LogFactory.Create().AddConsole());
            BlocksReaderClientServiceCollectionExtensions.AddBlocksReaderClient(collection, clientOptions);
            BlocksReaderClientServiceCollectionExtensions.AddBlocksReaderHttpClient(collection,
                "http://localHost",
                new RedirectToTestHostMessageHandler(client));
            var provider = collection.BuildServiceProvider();
            var api =provider.GetRequiredService<IBlocksReaderApi>();
            var client1 = provider.GetRequiredService<IBlocksReaderClient>();

            return (api, client1);
        }
    }
}
