using System;
using Lykke.Bil2.BaseTests;
using Lykke.Bil2.BaseTests.HttpMessageHandlers;
using Lykke.Bil2.Client.SignService.Services;
using Lykke.Logs;
using Lykke.Logs.Loggers.LykkeConsole;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.Bil2.Client.SignService.Tests.Configuration
{
    public abstract class SignServiceClientBase : ClientTestBase
    {
        protected ISignServiceApi CreateClientApi<TStartup>(string integrationName, string localhost, TimeSpan? timeout = null) where TStartup : class 
        {
            IServiceCollection collection = new ServiceCollection();
            var testServer = CreateTestServer<TStartup>();
            var client = testServer.CreateClient();
            collection.AddSingleton(LogFactory.Create().AddConsole());
            collection.AddSignServiceClient(options =>
            {
                options.Timeout = timeout;
                options.AddDelegatingHandler(new RedirectToTestHostMessageHandler(client));
                options.AddIntegration(integrationName, integrationOptions =>
                {
                    integrationOptions.Url = localhost;
                });
            });
            var provider = collection.BuildServiceProvider();
            var api = provider.GetRequiredService<ISignServiceApiProvider>().GetApi(integrationName);

            return api;
        }
    }
}
