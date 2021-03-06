﻿using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Bil2.Contract.Common.Exceptions;
using Lykke.Bil2.Contract.Common.Responses;
using Lykke.Bil2.Contract.TransactionsExecutor.Requests;
using Lykke.Bil2.Contract.TransactionsExecutor.Responses;
using Lykke.Bil2.Sdk.Repositories;
using Lykke.Bil2.Sdk.TransactionsExecutor.Exceptions;
using Lykke.Bil2.Sdk.TransactionsExecutor.Repositories;
using Lykke.Bil2.Sdk.TransactionsExecutor.Services;
using Lykke.Bil2.SharedDomain;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Bil2.Sdk.TransactionsExecutor.Controllers
{
    [ApiController]
    [Route("/api/transactions")]
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    internal class TransactionsController : ControllerBase
    {
        private readonly ITransferAmountTransactionsBuilder _transferAmountTransactionsBuilder;
        private readonly ITransferCoinsTransactionsBuilder _transferCoinsTransactionsBuilder;
        private readonly ITransactionBroadcaster _transactionBroadcaster;
        private readonly ITransferAmountTransactionsEstimator _transferAmountTransactionsEstimator;
        private readonly ITransferCoinsTransactionsEstimator _transferCoinsTransactionsEstimator;
        private readonly IRawObjectReadOnlyRepository _rawObjectsRepository;
        private readonly ITransactionsStateProvider _transactionsStateProvider;

        public TransactionsController(
            ITransferAmountTransactionsBuilder transferAmountTransactionsBuilder,
            ITransferCoinsTransactionsBuilder transferCoinsTransactionsBuilder,
            ITransactionBroadcaster transactionBroadcaster,
            ITransferAmountTransactionsEstimator transferAmountTransactionsEstimator,
            ITransferCoinsTransactionsEstimator transferCoinsTransactionsEstimator,
            IRawObjectReadOnlyRepository rawObjectsRepository,
            ITransactionsStateProvider transactionsStateProvider)
        {
            _transferAmountTransactionsBuilder = transferAmountTransactionsBuilder ?? throw new ArgumentNullException(nameof(transferAmountTransactionsBuilder));
            _transferCoinsTransactionsBuilder = transferCoinsTransactionsBuilder ?? throw new ArgumentNullException(nameof(transferCoinsTransactionsBuilder));
            _transactionBroadcaster = transactionBroadcaster ?? throw new ArgumentNullException(nameof(transactionBroadcaster));
            _transferAmountTransactionsEstimator = transferAmountTransactionsEstimator ?? throw new ArgumentNullException(nameof(transferAmountTransactionsEstimator));
            _transferCoinsTransactionsEstimator = transferCoinsTransactionsEstimator ?? throw new ArgumentNullException(nameof(transferCoinsTransactionsEstimator));
            _rawObjectsRepository = rawObjectsRepository ?? throw new ArgumentNullException(nameof(rawObjectsRepository));
            _transactionsStateProvider = transactionsStateProvider ?? throw new ArgumentNullException(nameof(transactionsStateProvider));
        }

        [HttpPost("built/transfers/amount")]
        public async Task<ActionResult<BuildTransactionResponse>> BuildTransferAmount([FromBody] BuildTransferAmountTransactionRequest request)
        {
            try
            {
                var response = await _transferAmountTransactionsBuilder.BuildTransferAmountAsync(request);
                if (response == null)
                {
                    throw new InvalidOperationException("Not null response object expected");
                }

                return Ok(response);
            }
            catch (TransactionBuildingException ex)
            {
                var errorResponse = BlockchainErrorResponse.CreateFromCode(ex.Error, ex.Message);

                return BadRequest(errorResponse);
            }
        }

        [HttpPost("built/transfers/coins")]
        public async Task<ActionResult<BuildTransactionResponse>> BuildTransferCoins([FromBody] BuildTransferCoinsTransactionRequest request)
        {
            try
            {
                var response = await _transferCoinsTransactionsBuilder.BuildTransferCoinsAsync(request);
                if (response == null)
                {
                    throw new InvalidOperationException("Not null response object expected");
                }

                return Ok(response);
            }
            catch (TransactionBuildingException ex)
            {
                var errorResponse = BlockchainErrorResponse.CreateFromCode(ex.Error, ex.Message);

                return BadRequest(errorResponse);
            }
        }

        [HttpPost("estimated/transfers/amount")]
        public async Task<ActionResult<EstimateTransactionResponse>> EstimateTransferAmount([FromBody] EstimateTransferAmountTransactionRequest request)
        {
            var response = await _transferAmountTransactionsEstimator.EstimateTransferAmountAsync(request);
            if (response == null)
            {
                throw new InvalidOperationException("Not null response object expected");
            }

            return Ok(response);
        }

        [HttpPost("estimated/transfers/coins")]
        public async Task<ActionResult<EstimateTransactionResponse>> EstimateTransferCoins([FromBody] EstimateTransferCoinsTransactionRequest request)
        {
            var response = await _transferCoinsTransactionsEstimator.EstimateTransferCoinsAsync(request);
            if (response == null)
            {
                throw new InvalidOperationException("Not null response object expected");
            }

            return Ok(response);
        }

        [HttpPost("broadcasted")]
        public async Task<IActionResult> Broadcast([FromBody] BroadcastTransactionRequest request)
        {
            try
            {
                await _transactionBroadcaster.BroadcastAsync(request);
            }
            catch (TransactionBroadcastingException ex)
            {
                var errorResponse = BlockchainErrorResponse.CreateFromCode(ex.Error, ex.Message);

                return BadRequest(errorResponse);
            }

            return Ok();
        }

        [HttpGet("{transactionId}/raw")]
        public async Task<ActionResult<RawObjectResponse>> GetRaw([FromRoute] TransactionId transactionId)
        {
            if (transactionId == null)
                throw RequestValidationException.ShouldBeNotNull(nameof(transactionId));

            var raw = await _rawObjectsRepository.GetOrDefaultAsync(RawObjectType.Transaction, transactionId);
            if (raw == null)
            {
                return NotFound(BlockchainErrorResponse.Create($"Raw transaction [{transactionId}] not found"));
            }

            return Ok(new RawObjectResponse(raw));
        }

        [HttpGet("{transactionId}/state")]
        public async Task<ActionResult<TransactionStateResponse>> GetState([FromRoute] TransactionId transactionId)
        {
            if (transactionId == null)
                throw RequestValidationException.ShouldBeNotNull(nameof(transactionId));

            var state = await _transactionsStateProvider.GetStateAsync(transactionId);

            return Ok(new TransactionStateResponse(state));
        }
    }
}
