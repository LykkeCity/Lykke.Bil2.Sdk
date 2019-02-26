using Lykke.Sdk;
using Microsoft.AspNetCore.TestHost;

namespace Lykke.Bil2.BaseTests
{
    public abstract class ClientTestBase
    {
        public ClientTestBase()
        {
        }

        public TestServer CreateTestServer<TStartup>() where TStartup : class
        {
            LykkeWebHostFactory factory = new LykkeWebHostFactory();
            var webHostBuilder = factory.CreateWebHostBuilder<TStartup>(options =>
            {
                options.Port = 5000;
                options.IsDebug = true;
            });

            TestServer testServer = new TestServer(webHostBuilder);

            return testServer;
        }
    }
}
