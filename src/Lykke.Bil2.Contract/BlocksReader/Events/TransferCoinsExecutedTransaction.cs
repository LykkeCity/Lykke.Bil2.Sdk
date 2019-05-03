using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using JetBrains.Annotations;
using Lykke.Bil2.Contract.Common;
using Lykke.Bil2.SharedDomain;

namespace Lykke.Bil2.Contract.BlocksReader.Events
{
    /// <summary>
    /// "Transfer coins" transactions model.
    /// Should be published for each executed transaction in the block being read if
    /// integration uses “transfer coins” transactions model. Integration should either
    /// support “transfer coins” or “transfer amount” transactions model.
    /// </summary>
    [PublicAPI, DataContract]
    public class TransferCoinsExecutedTransaction
    {
        /// <summary>
        /// Number of the transaction in the block.
        /// </summary>
        [DataMember(Order = 0)]
        public int TransactionNumber { get; }

        /// <summary>
        /// ID of the transaction.
        /// </summary>
        [DataMember(Order = 1)]
        public TransactionId TransactionId { get; }

        /// <summary>
        /// Coins which were received within the transaction.
        /// </summary>
        [DataMember(Order = 2)]
        public IReadOnlyCollection<ReceivedCoin> ReceivedCoins { get; }

        /// <summary>
        /// Coins which were spent within the transaction.
        /// </summary>
        [DataMember(Order = 3)]
        public IReadOnlyCollection<CoinId> SpentCoins { get; }

        /// <summary>
        /// Optional.
        /// Fees in the particular asset, that was spent for the transaction.
        /// Can be omitted, if fee can be determined from the received and spent coins.
        /// </summary>
        [CanBeNull, DataMember(Order = 4)]
        public IReadOnlyCollection<Fee> Fees { get; }

        /// <summary>
        /// Optional.
        /// Flag which indicates, if transaction is irreversible.
        /// </summary>
        [CanBeNull, DataMember(Order = 5)]
        public bool? IsIrreversible { get; }

        /// <summary>
        /// "Transfer coins" transactions model.
        /// Should be published for each executed transaction in the block being read if
        /// integration uses “transfer coins” transactions model. Integration should either
        /// support “transfer coins” or “transfer amount” transactions model.
        /// </summary>
        /// <param name="transactionNumber">Number of the transaction in the block.</param>
        /// <param name="transactionId">ID of the transaction.</param>
        /// <param name="receivedCoins">Coins which were received within the transaction.</param>
        /// <param name="spentCoins">Coins which were spent within the transaction.</param>
        /// <param name="fees">
        /// Optional.
        /// Fees in the particular asset, that was spent for the transaction.
        /// Can be omitted, if fee can be determined from the received and spent coins.
        /// </param>
        /// <param name="isIrreversible">
        /// Optional.
        /// Flag which indicates, if transaction is irreversible.
        /// </param>
        public TransferCoinsExecutedTransaction(
            int transactionNumber,
            TransactionId transactionId,
            IReadOnlyCollection<ReceivedCoin> receivedCoins,
            IReadOnlyCollection<CoinId> spentCoins,
            IReadOnlyCollection<Fee> fees = null,
            bool? isIrreversible = null)
        {
            if (transactionNumber < 0)
                throw new ArgumentOutOfRangeException(nameof(transactionNumber), transactionNumber, "Should be zero or positive number");

            ReceivedCoinsValidator.Validate(receivedCoins);
            SpentCoinsValidator.Validate(spentCoins);
            FeesValidator.ValidateFeesInResponse(fees);

            TransactionNumber = transactionNumber;
            TransactionId = transactionId ?? throw new ArgumentNullException(nameof(transactionId));
            ReceivedCoins = receivedCoins;
            SpentCoins = spentCoins;
            Fees = fees;
            IsIrreversible = isIrreversible;
        }
    }
}
