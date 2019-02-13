using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Sdk;

namespace BlocksReaderExample
{
    [UsedImplicitly]
    internal sealed class Program
    {
        public static string IntegrationName { get; private set; }

        // ReSharper disable once UnusedParameter.Global
        public static async Task Main(string[] args)
        {
#if DEBUG
            var port = 5002;

            IntegrationName = args.Length < 1 ? "Example" : args[0];

            if (args.Length > 1)
            {
                port = int.Parse(args[1]);
            }

            await LykkeStarter.Start<Startup>(true, port);
#else
            await LykkeStarter.Start<Startup>(false);
#endif
        }
    }
}
