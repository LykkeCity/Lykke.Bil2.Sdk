using JetBrains.Annotations;
using System.Threading.Tasks;
using System.Collections.Generic;
using Lykke.Blockchains.Integrations.Contract.Common;
using Lykke.Blockchains.Integrations.Contract.SignService.Responses;

namespace Lykke.Blockchains.Integrations.Sdk.SignService.Services
{
    [PublicAPI]
    public interface ITransactionSigner
    {
        /// <summary>
        /// Should sign the given transaction with the specified private keys.
        /// </summary>
        /// <param name="privateKeys">
        /// Private keys of the addresses which should sign the transaction.
        /// Multiple keys can be used for the transactions with multiple inputs.
        /// </param>
        /// <param name="requestTransactionContext">
        /// Implementation specific transaction context.
        /// </param>
        Task<SignTransactionResponse> SignAsync(IReadOnlyCollection<string> privateKeys, Base58String requestTransactionContext);
    }
}
