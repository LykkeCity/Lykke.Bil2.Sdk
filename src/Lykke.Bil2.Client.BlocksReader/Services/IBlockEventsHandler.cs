﻿using JetBrains.Annotations;
using Lykke.Bil2.Contract.BlocksReader.Events;
using Lykke.Bil2.RabbitMq.Subscription;

namespace Lykke.Bil2.Client.BlocksReader.Services
{
    /// <summary>
    /// Handles events from the blockchain integration block reader application.
    /// </summary>
    [PublicAPI]
    public interface IBlockEventsHandler :
        IMessageHandler<BlockHeaderReadEvent, string>,
        IMessageHandler<BlockNotFoundEvent, string>,
        IMessageHandler<TransferAmountTransactionExecutedEvent, string>,
        IMessageHandler<TransferCoinsTransactionExecutedEvent, string>,
        IMessageHandler<TransactionFailedEvent, string>,
        IMessageHandler<LastIrreversibleBlockUpdatedEvent, string>
    {
    }
}
