using System;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Blockchains.Integrations.Contract.SignService.Requests;
using Lykke.Blockchains.Integrations.Contract.SignService.Responses;
using Lykke.Blockchains.Integrations.Sdk.SignService.Models;
using Lykke.Blockchains.Integrations.Sdk.SignService.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Blockchains.Integrations.Sdk.SignService.Controllers
{
    [ApiController]
    [Route("/api/transactions")]
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    internal class TransactionsController : ControllerBase
    {
        private readonly ITransactionSigner _transactionSigner;
        private readonly EncryptionConfiguration _encryptionConfiguration;

        public TransactionsController(
            ITransactionSigner transactionSigner,
            EncryptionConfiguration encryptionConfiguration)
        {
            _transactionSigner = transactionSigner;
            _encryptionConfiguration = encryptionConfiguration;
        }

        [HttpPost("signed")]
        public async Task<ActionResult<SignTransactionResponse>> SignTransaction(SignTransactionRequest request)
        {
            var privateKeys = request
                .PrivateKeys
                .Select(x => x.DecryptToString(_encryptionConfiguration.PrivateKey))
                .ToArray();

            var response = await _transactionSigner.SignAsync(privateKeys, request.TransactionContext);
            if (response == null)
            {
                throw new InvalidOperationException("Not null response object expected");
            }

            return Ok(response);
        }
    }
}
