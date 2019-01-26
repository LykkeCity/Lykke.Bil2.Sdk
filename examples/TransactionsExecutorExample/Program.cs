using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Sdk;

namespace TransactionsExecutorExample
{
    [UsedImplicitly]
    internal sealed class Program
    {
        // ReSharper disable once UnusedParameter.Global
        public static async Task Main(string[] args)
        {
#if DEBUG
            await LykkeStarter.Start<Startup>(true, 5001);
#else
            await LykkeStarter.Start<Startup>(false);
#endif
        }
    }
}
