using Lykke.Bil2.BaseTests;
using Lykke.Bil2.BaseTests.HttpMessageHandlers;
using Lykke.Bil2.Client.BlocksReader.Services;
using Lykke.Logs;
using Lykke.Logs.Loggers.LykkeConsole;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Lykke.Bil2.Client.BlocksReader.Tests.Configuration
{
    public abstract class BlocksReaderClientBase : ClientTestBase
    {
        protected (IBlocksReaderClient,
            IBlocksReaderApiFactory,
            IDisposable)
            CreateClientApi<TStartup>(Action<BlocksReaderClientOptions> clientOptions)

            where TStartup : class
        {
            IServiceCollection collection = new ServiceCollection();
            var testServer = CreateTestServer<TStartup>();
            var client = testServer.CreateClient();

            collection.AddSingleton(LogFactory.Create().AddConsole());
            collection.AddBlocksReaderClient(clientOptions);
            collection.AddBlocksReaderHttpClient("http://localhost", new RedirectToTestHostMessageHandler(client));

            var provider = collection.BuildServiceProvider();
            var client1 = provider.GetRequiredService<IBlocksReaderClient>();
            var apiFactory = provider.GetRequiredService<IBlocksReaderApiFactory>();
            var testServerDependencyWrapper = new TestServerDependencyWrapper(testServer);

            return (client1, apiFactory, testServerDependencyWrapper);
        }
    }
}
