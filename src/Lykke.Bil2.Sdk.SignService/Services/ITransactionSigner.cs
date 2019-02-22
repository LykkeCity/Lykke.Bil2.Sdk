using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Bil2.Contract.Common;
using Lykke.Bil2.Contract.SignService.Responses;

namespace Lykke.Bil2.Sdk.SignService.Services
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
