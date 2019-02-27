using Lykke.Bil2.BaseTests;
using Lykke.Bil2.BaseTests.HttpMessageHandlers;
using Lykke.Bil2.Client.SignService;
using Lykke.Common.Log;
using Lykke.Logs;
using Lykke.Logs.Loggers.LykkeConsole;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.Bil2.SignService.Client.Tests.Configuration
{
    public abstract class SignServiceClientBase : ClientTestBase
    {
        protected ISignServiceApi CreateClientApi<TStartup>(string localhost) where TStartup : class 
        {
            IServiceCollection collection = new ServiceCollection();
            var testServer = base.CreateTestServer<TStartup>();
            var client = testServer.CreateClient();
            collection.AddSingleton<ILogFactory>(LogFactory.Create().AddConsole());
            collection.AddSignServiceClient(localhost, new RedirectToTestHostMessageHandler(client));
            var provider = collection.BuildServiceProvider();
            var api =provider.GetRequiredService<ISignServiceApi>();

            return api;
        }
    }
}
