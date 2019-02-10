using System.Threading.Tasks;

namespace Lykke.Blockchains.Integrations.Sdk.BlocksReader.Services
{
    /// <summary>
    /// Reads the block from the blockchain node and notifies observer about block content via the <see cref="IBlockListener"/>
    /// </summary>
    public interface IBlockReader
    {
        /// <summary>
        /// Reads the block from the blockchain node and notifies observer about block content via the <paramref name="listener" />
        /// </summary>
        /// <param name="blockNumber">Block number to read</param>
        /// <param name="listener">Lister which should be used to notify observer about block content</param>
        Task ReadBlockAsync(long blockNumber, IBlockListener listener);
    }
}
