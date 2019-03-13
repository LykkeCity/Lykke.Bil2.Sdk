using System;

namespace BlocksReaderExample.Services
{
    public class NodeClient : INodeClient
    {
        // ReSharper disable NotAccessedField.Local
        private readonly string _nodeUrl;
        private readonly Random _random;
        // ReSharper restore NotAccessedField.Local
        
        public NodeClient(string nodeUrl)
        {
            _nodeUrl = nodeUrl;
            _random = new Random();
        }
    }
}
