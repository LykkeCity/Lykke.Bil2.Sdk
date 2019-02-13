using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Sdk;

namespace SignServiceExample
{
    [UsedImplicitly]
    internal sealed class Program
    {
        // ReSharper disable once UnusedParameter.Global
        public static async Task Main(string[] args)
        {
#if DEBUG
            await LykkeStarter.Start<Startup>(true);
#else
            await LykkeStarter.Start<Startup>(false);
#endif
        }
    }
}
