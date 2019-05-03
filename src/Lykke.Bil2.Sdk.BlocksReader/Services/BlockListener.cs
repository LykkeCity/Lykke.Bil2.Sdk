using System;
using System.Threading.Tasks;
using Lykke.Bil2.Contract.BlocksReader.Events;
using Lykke.Bil2.Contract.Common;
using Lykke.Bil2.RabbitMq.Publication;
using Lykke.Bil2.Sdk.BlocksReader.Repositories;
using Lykke.Bil2.Sdk.Repositories;
using Lykke.Bil2.SharedDomain;

namespace Lykke.Bil2.Sdk.BlocksReader.Services
{
    internal class BlockListener : 
        IBlockListener,
        IDisposable
    {
        private readonly IMessagePublisher _messagePublisher;
        private readonly IRawObjectWriteOnlyRepository _rawObjectsRepository;
        private readonly int _transactionsBatchSize;
        private readonly int _maxTransactionsSavingParallelism;
        private readonly BlockchainTransferModel _transferModel;

        private BlockHeaderReadEvent _blockHeader;
        private bool _isBlockNotFound;
        private BlockTransactionsListener _transactionsListener;
        private Task _saveRawBlockTask;
        
        public BlockListener(
            IMessagePublisher messagePublisher,
            IRawObjectWriteOnlyRepository rawObjectsRepository,
            int transactionsBatchSize,
            int maxTransactionsSavingParallelism,
            BlockchainTransferModel transferModel)
        {
            _messagePublisher = messagePublisher;
            _rawObjectsRepository = rawObjectsRepository;
            _transactionsBatchSize = transactionsBatchSize;
            _maxTransactionsSavingParallelism = maxTransactionsSavingParallelism;
            _transferModel = transferModel;
        }

        public IBlockTransactionsListener StartBlockTransactionsHandling(BlockHeaderReadEvent evt)
        {
            if (_blockHeader != null)
            {
                throw new InvalidOperationException($"Block header already was handled: {_blockHeader}");
            }

            if (_isBlockNotFound)
            {
                throw new InvalidOperationException("Block already handled as not found");
            }
            
            _blockHeader = evt ?? throw new ArgumentNullException(nameof(evt));

            _transactionsListener = new BlockTransactionsListener
            (
                _blockHeader,
                _messagePublisher,
                _rawObjectsRepository,
                _transactionsBatchSize,
                _maxTransactionsSavingParallelism,
                _transferModel
            );

            return _transactionsListener;
        }

        public void HandleRawBlock(Base64String rawBlock, BlockId blockId)
        {
            if (_blockHeader != null)
            {
                throw new InvalidOperationException("Invoke HandleRawBlock before StartBlockTransactionsHandling");
            }

            _saveRawBlockTask = _rawObjectsRepository.SaveAsync(RawObjectType.Block, blockId, rawBlock);
        }

        public void HandleBlockNotFound(BlockNotFoundEvent evt)
        {           
            if (_blockHeader != null)
            {
                throw new InvalidOperationException($"Block header already was handled: {_blockHeader}");
            }

            if (_isBlockNotFound)
            {
                throw new InvalidOperationException("Block already handled as not found");
            }
            
            _isBlockNotFound = true;

            _messagePublisher.Publish(evt);
        }

        public async Task FlushAsync()
        {
            if (_transactionsListener != null)
            {
                await _transactionsListener.FlushAsync();
            }

            if (_blockHeader != null)
            {
                _messagePublisher.Publish(_blockHeader);
            }

            if (_saveRawBlockTask != null)
            {
                await _saveRawBlockTask;
            }
        }

        public void Dispose()
        {
            _transactionsListener?.Dispose();
            _saveRawBlockTask?.Dispose();
        }
    }
}
