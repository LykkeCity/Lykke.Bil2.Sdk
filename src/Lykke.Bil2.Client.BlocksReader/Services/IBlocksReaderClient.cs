using System;
using JetBrains.Annotations;

namespace Lykke.Bil2.Client.BlocksReader.Services
{
    /// <summary>
    /// Blockchain integration blocks reader client
    /// </summary>
    /// <remarks>
    /// Use <see cref="BlocksReaderClientServiceCollectionExtensions.AddBlocksReaderClient"/> to register <see cref="IBlocksReaderClient"/>
    /// implementation in the service provider.
    /// </remarks>
    [PublicAPI]
    public interface IBlocksReaderClient : IDisposable
    {
        /// <summary>
        /// Starts the commands sending engine. Should be called in order to send commands via <see cref="IBlocksReaderApi"/> and start events
        /// listening using <see cref="StartListening"/>.
        /// </summary>
        void StartSending();

        /// <summary>
        /// Starts the events listening engine. Should be called in order to receive events via <see cref="IBlockEventsHandler"/>. Should be called
        /// after <see cref="StartSending"/>.
        /// </summary>
        void StartListening();
    }
}
