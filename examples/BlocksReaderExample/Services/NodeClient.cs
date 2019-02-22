using System;

namespace BlocksReaderExample.Services
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
    }
}
