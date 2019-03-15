using System;
using System.Net.Http;
using Lykke.Common.Log;

namespace Lykke.Bil2.WebClient
{
    public class HttpClientGeneratorOptions
    {
        public string Url { get; set; }

        public ILogFactory LogFactory { get; set; }

        public DelegatingHandler[] Handlers { get; set; }

        public TimeSpan? Timeout { get; set; }

        internal HttpClientGeneratorOptions()
        {

        }
    }
}
