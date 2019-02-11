﻿using JetBrains.Annotations;

namespace Lykke.Blockchains.Integrations.Client.BlocksReader.Services
{
    /// <summary>
    /// Factory of the blockchain integration blocks reader API instance for the particular blockchain integration.
    /// </summary>
    [PublicAPI]
    public interface IBlocksReaderApiFactory
    {
        /// <summary>
        /// Creates the blockchain integration blocks reader API instance for the particular blockchain integration.
        /// </summary>
        IBlocksReaderApi Create(string integrationName);
    }
}
