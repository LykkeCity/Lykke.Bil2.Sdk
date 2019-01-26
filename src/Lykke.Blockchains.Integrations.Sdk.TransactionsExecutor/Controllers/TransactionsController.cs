using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Blockchains.Integrations.Contract.Common;
using Lykke.Blockchains.Integrations.Contract.Common.Responses;
using Lykke.Blockchains.Integrations.Contract.TransactionsExecutor.Requests;
using Lykke.Blockchains.Integrations.Contract.TransactionsExecutor.Responses;
using Lykke.Blockchains.Integrations.Sdk.TransactionsExecutor.Repositories;
using Lykke.Blockchains.Integrations.Sdk.TransactionsExecutor.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Blockchains.Integrations.Sdk.TransactionsExecutor.Controllers
{
    [ApiController]
    [Route("/api/transactions")]
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    internal class TransactionsController : ControllerBase
    {
        private readonly ITransactionExecutor _transactionExecutor;
        private readonly ITransactionEstimator _transactionEstimator;
        private readonly IRawTransactionReadOnlyRepository _rawTransactionsRepository;

        public TransactionsController(
            ITransactionExecutor transactionExecutor,
            ITransactionEstimator transactionEstimator,
            IRawTransactionReadOnlyRepository rawTransactionsRepository)
        {
            _transactionExecutor = transactionExecutor ?? throw new ArgumentNullException(nameof(transactionExecutor));
            _transactionEstimator = transactionEstimator ?? throw new ArgumentNullException(nameof(transactionEstimator));
            _rawTransactionsRepository = rawTransactionsRepository ?? throw new ArgumentNullException(nameof(rawTransactionsRepository));
        }

        [HttpPost("sending/built")]
        public async Task<ActionResult<BuildSendingTransactionResponse>> BuildSending([FromBody] BuildSendingTransactionRequest request)
        {
            try
            {
                var response = await _transactionExecutor.BuildSendingAsync(request);
                if (response == null)
                {
                    throw new InvalidOperationException("Not null response object expected");
                }

                return Ok(response);
            }
            catch (SendingTransactionBuildingException ex)
            {
                var errorResponse = BlockchainErrorResponse.CreateFromCode(ex.Error, ex.ToString());

                return BadRequest(errorResponse);
            }
        }

        [HttpPost("sending/estimated")]
        public async Task<ActionResult<EstimateSendingTransactionResponse>> EstimateSending([FromBody] EstimateSendingTransactionRequest request)
        {
            var response = await _transactionEstimator.EstimateSendingAsync(request);
            if (response == null)
            {
                throw new InvalidOperationException("Not null response object expected");
            }

            return Ok(response);
        }

        [HttpPost("receiving/built")]
        public async Task<ActionResult<BuildReceivingTransactionResponse>> BuildReceiving([FromBody] BuildReceivingTransactionRequest request)
        {
            var response = await _transactionExecutor.BuildReceivingAsync(request);
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
                await _transactionExecutor.BroadcastAsync(request);
            }
            catch (TransactionBroadcastingException ex)
            {
                var errorResponse = BlockchainErrorResponse.CreateFromCode(ex.Error, ex.ToString());

                return BadRequest(errorResponse);
            }

            return Ok();
        }

        [HttpGet("{transactionHash}/raw")]
        public async Task<ActionResult<RawTransactionResponse>> GetRaw(string transactionHash)
        {
            if (string.IsNullOrWhiteSpace(transactionHash))
                throw RequestValidationException.ShouldBeNotEmptyString(transactionHash);

            var raw = await _rawTransactionsRepository.GetOrDefaultAsync(transactionHash);
            if (raw == null)
            {
                return NotFound(BlockchainErrorResponse.Create($"Raw transaction [{transactionHash}] not found"));
            }

            return Ok(new RawTransactionResponse(raw));
        }
    }
}
