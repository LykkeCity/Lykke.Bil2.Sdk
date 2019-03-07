using System;

namespace Lykke.Bil2.Client.BlocksReader.Tests.Configuration
{
    internal class TestServerDependencyWrapper : IDisposable
    {
        private readonly IDisposable _testServer;

        public TestServerDependencyWrapper(IDisposable testServer)
        {
            _testServer = testServer;
        }

        public void Dispose()
        {
            _testServer.Dispose();
            StartupDependencyFactorySingleton.Instance.ServerServiceProvider = null;
            GC.Collect(2, GCCollectionMode.Forced, true);
        }
    }
}