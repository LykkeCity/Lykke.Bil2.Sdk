using System.Net.Http;
using System.Threading.Tasks;
using Lykke.Blockchains.Integrations.Sdk.TransactionsExecutor.Services;

namespace TransactionsExecutorExample.Services
{
    public class HealthProvider : IHealthProvider
    {
        private readonly INodeClient _nodeClient;

        public HealthProvider(INodeClient nodeClient)
        {
            _nodeClient = nodeClient;
        }

        public async Task<string> GetDiseaseAsync()
        {
            try
            {
                if (!await _nodeClient.GetIsSynchronizedAsync())
                {
                    return "Node is not synchronized";
                }
            }
            catch (HttpRequestException ex)
            {
                return $"Node is unavailable: {ex}";
            }

            return null;
        }
    }
}
