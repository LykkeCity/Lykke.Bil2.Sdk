using System.Collections.Generic;
using Lykke.Bil2.WebClient.Services;

namespace Lykke.Bil2.Client.BlocksReader.Services
{
    internal class BlocksReaderWebApiProvider : 
        BaseWebClientApiProvider<IBlocksReaderWebApi>,
        IBlocksReaderWebApiProvider
    {
        public BlocksReaderWebApiProvider(IReadOnlyDictionary<string, IBlocksReaderWebApi> integrationApis) : 
            base(integrationApis)
        {
        }
    }
}
