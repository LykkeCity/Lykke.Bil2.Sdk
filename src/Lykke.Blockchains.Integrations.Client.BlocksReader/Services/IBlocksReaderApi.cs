using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Blockchains.Integrations.Contract.BlocksReader.Commands;

namespace Lykke.Blockchains.Integrations.Client.BlocksReader.Services
{
    /// <summary>
    /// Blockchain integration blocks reader messaging API.
    /// </summary>
    [PublicAPI]
    public interface IBlocksReaderApi
    {
        /// <summary>
        /// Sends <see cref="ReadBlockCommand"/>.
        /// </summary>
        Task SendAsync(ReadBlockCommand command);
    }
}
