using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace TransactionsExecutorExample.Services
{
    public class NodeClient : INodeClient
    {
        private readonly string _nodeUrl;
        private readonly Random _random;

        public NodeClient(string nodeUrl)
        {
            _nodeUrl = nodeUrl;
            _random = new Random();
        }

        public Task<bool> GetIsSynchronizedAsync()
        {
            var value = _random.Next(0, 100);

            if (value < 30)
            {
                throw new HttpRequestException($"Request to {_nodeUrl} is failed");
            }

            if (value < 40)
            {
                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }
    }
}
