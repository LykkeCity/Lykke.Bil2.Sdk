using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Lykke.Bil2.Contract.BlocksReader.Events;
using Lykke.Bil2.Contract.Common;
using Lykke.Bil2.RabbitMq.Publication;
using Lykke.Bil2.Sdk.BlocksReader.Repositories;
using Lykke.Bil2.Sdk.Repositories;
using Lykke.Bil2.SharedDomain;

namespace Lykke.Bil2.Sdk.BlocksReader.Services
{
    internal class BlockTransactionsListener : 
        IBlockTransactionsListener,
        IDisposable
    {
        private int WaitingToSendTransactionsCount => _transferCoinsExecutedTransactions.Count +
                                                      _transferAmountExecutedTransactions.Count +
                                                      _failedTransactions.Count;

        private readonly BlockHeaderReadEvent _blockHeader;
        private readonly IMessagePublisher _messagePublisher;
        private readonly IRawObjectWriteOnlyRepository _rawObjectsRepository;
        private readonly int _transactionsBatchSize;
        private readonly int _tailTransactionsBatchExtensionSize;
        private readonly BlockchainTransferModel _transferModel;
        
        private readonly List<TransferAmountExecutedTransaction> _transferAmountExecutedTransactions;
        private readonly List<TransferCoinsExecutedTransaction> _transferCoinsExecutedTransactions;
        private readonly List<FailedTransaction> _failedTransactions;
        private readonly SemaphoreSlim _rawTransactionsPersistenceParallelismGuard;
        private readonly List<Task> _rawTransactionsPersistenceTasks;
        private int _remainedTransactionsCount;
        
        public BlockTransactionsListener(
            BlockHeaderReadEvent blockHeader,
            IMessagePublisher messagePublisher,
            IRawObjectWriteOnlyRepository rawObjectsRepository,
            int transactionsBatchSize,
            int maxTransactionsSavingParallelism,
            BlockchainTransferModel transferModel)
        {
            if (transactionsBatchSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(transactionsBatchSize), transactionsBatchSize, "Should be positive number");
            }
            if (maxTransactionsSavingParallelism <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(transactionsBatchSize), transactionsBatchSize, "Should be positive number");
            }

            _blockHeader = blockHeader;
            _messagePublisher = messagePublisher;
            _rawObjectsRepository = rawObjectsRepository;
            _transactionsBatchSize = transactionsBatchSize;
            _tailTransactionsBatchExtensionSize = transactionsBatchSize * 20 / 100; // 20%
            _transferModel = transferModel;

            _transferAmountExecutedTransactions = new List<TransferAmountExecutedTransaction>(_transactionsBatchSize);
            _transferCoinsExecutedTransactions = new List<TransferCoinsExecutedTransaction>(_transactionsBatchSize);
            _failedTransactions = new List<FailedTransaction>(_transactionsBatchSize);
            _rawTransactionsPersistenceTasks = new List<Task>(blockHeader.BlockTransactionsCount);
            _remainedTransactionsCount = blockHeader.BlockTransactionsCount;
            _rawTransactionsPersistenceParallelismGuard = new SemaphoreSlim(maxTransactionsSavingParallelism);
        }

        public void HandleExecutedTransaction(TransferAmountExecutedTransaction transaction)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException(nameof(transaction));
            }

            _transferAmountExecutedTransactions.Add(transaction);

            SendTransactionsBatchIfNeeded();
        }

        public void HandleExecutedTransaction(TransferCoinsExecutedTransaction transaction)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException(nameof(transaction));
            }

            _transferCoinsExecutedTransactions.Add(transaction);

            SendTransactionsBatchIfNeeded();
        }

        public void HandleFailedTransaction(FailedTransaction transaction)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException(nameof(transaction));
            }

            _failedTransactions.Add(transaction);

            SendTransactionsBatchIfNeeded();
        }

        public async Task HandleRawTransactionAsync(Base64String rawTransaction, TransactionId transactionId)
        {
            async Task SaveRawTransaction()
            {
                try
                {
                    await _rawObjectsRepository.SaveAsync(RawObjectType.Transaction, transactionId, rawTransaction);
                }
                finally
                {
                    _rawTransactionsPersistenceParallelismGuard.Release();
                }
            }

            await _rawTransactionsPersistenceParallelismGuard.WaitAsync();

            _rawTransactionsPersistenceTasks.Add(SaveRawTransaction());
        }

        public async Task FlushAsync()
        {
            if (WaitingToSendTransactionsCount > 0)
            {
                SendTransactionsBatch();
            }

            await Task.WhenAll(_rawTransactionsPersistenceTasks);
        }
        
        public void Dispose()
        {
            _rawTransactionsPersistenceParallelismGuard.Dispose();
        }

        private void SendTransactionsBatchIfNeeded()
        {
            var waitingToSendTransactionsCount = WaitingToSendTransactionsCount;

            if (waitingToSendTransactionsCount < _transactionsBatchSize)
            {
                return;
            }

            var tailTransactionsCount = _remainedTransactionsCount - waitingToSendTransactionsCount;

            if (tailTransactionsCount > 0 && tailTransactionsCount <= _tailTransactionsBatchExtensionSize)
            {
                // Don't send the batch now, since we need to include rest of transactions into this batch.
                return;
            }

            SendTransactionsBatch();
        }

        private void SendTransactionsBatch()
        {
            if (_transferModel == BlockchainTransferModel.Amount)
            {
                _messagePublisher.Publish(new TransferAmountTransactionsBatchEvent
                (
                    blockId: _blockHeader.BlockId,
                    transferAmountExecutedTransactions: _transferAmountExecutedTransactions,
                    failedTransactions: _failedTransactions
                ));

                _remainedTransactionsCount -= _transferAmountExecutedTransactions.Count + _failedTransactions.Count;

                _transferAmountExecutedTransactions.Clear();
                _failedTransactions.Clear();
            }
            else if (_transferModel == BlockchainTransferModel.Coins)
            {
                _messagePublisher.Publish(new TransferCoinsTransactionsBatchEvent
                (
                    blockId: _blockHeader.BlockId,
                    transferCoinsExecutedTransactions: _transferCoinsExecutedTransactions,
                    failedTransactions: _failedTransactions
                ));

                _remainedTransactionsCount -= _transferCoinsExecutedTransactions.Count + _failedTransactions.Count;

                _transferCoinsExecutedTransactions.Clear();
                _failedTransactions.Clear();
            }
            else
            {
                throw new InvalidOperationException($"Unknown transfer model {_transferModel}");
            }
        }
    }
}
