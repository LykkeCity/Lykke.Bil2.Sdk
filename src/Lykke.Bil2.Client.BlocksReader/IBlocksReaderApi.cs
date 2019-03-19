using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Bil2.Contract.BlocksReader.Commands;

namespace Lykke.Bil2.Client.BlocksReader
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
