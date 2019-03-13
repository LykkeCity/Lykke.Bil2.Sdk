using Lykke.Bil2.BaseTests;
using Lykke.Bil2.BaseTests.HttpMessageHandlers;
using Lykke.Bil2.Client.TransactionsExecutor;
using Lykke.Logs;
using Lykke.Logs.Loggers.LykkeConsole;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.Bil2.Client.TransactionExecutor.Tests.Configuration
{
    public abstract class TransactionsExecutorClientBase : ClientTestBase
    {
        protected ITransactionsExecutorApi CreateClientApi<TStartup>(string localhost) where TStartup : class 
        {
            IServiceCollection collection = new ServiceCollection();
            var testServer = CreateTestServer<TStartup>();
            var client = testServer.CreateClient();
            collection.AddSingleton(LogFactory.Create().AddConsole());
            collection.AddTransactionsExecutorClient(localhost, new RedirectToTestHostMessageHandler(client));
            var provider = collection.BuildServiceProvider();
            var api =provider.GetRequiredService<ITransactionsExecutorApi>();

            return api;
        }
    }
}
