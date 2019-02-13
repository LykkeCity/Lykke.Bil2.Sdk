using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Blockchains.Integrations.Contract.Common;
using Lykke.Blockchains.Integrations.Contract.TransactionsExecutor.Requests;
using Lykke.Blockchains.Integrations.Contract.TransactionsExecutor.Responses;
using Refit;

namespace Lykke.Blockchains.Integrations.Client.TransactionsExecutor
{
    /// <summary>
    /// Blockchain integration transactions executor HTTP API
    /// </summary>
    [PublicAPI]
    public interface ITransactionsExecutorApi
    {
        /// <summary>
        /// Should return some general service info. Used to check if the service is running.
        /// </summary>
        /// <returns></returns>
        [Get("/api/isalive")]
        Task<TransactionsExecutorIsAliveResponse> GetIsAliveAsync();

        /// <summary>
        /// Should return information about running integration. All required information could be gathered synchronously in the call.
        /// </summary>
        /// <returns></returns>
        [Get("/api/integration-info")]
        Task<IntegrationInfoResponse> GetIntegrationInfoAsync();

        /// <summary>
        /// Should check and return address validity.
        /// </summary>
        [Get("/api/addresses/{address}/validity")]
        Task<AddressValidityResponse> GetAddressValidityAsync(string address, [Query] AddressTagType? tagType = null, [Query] string tag = null);

        /// <summary>
        /// Should build a not signed transaction. For the blockchains where “sending” and “receiving”
        /// transactions are distinguished, this endpoint builds the “sending” transactions.
        /// </summary>
        [Post("/api/transactions/sending/built")]
        Task<BuildSendingTransactionResponse> BuildSendingTransactionAsync([Body] BuildSendingTransactionRequest body);

        /// <summary>
        /// Should estimate the transaction fee. For the blockchains where “sending” and “receiving”
        /// transactions are distinguished, this endpoint estimates fee for the “sending” transactions.
        /// </summary>
        [Post("/api/transactions/sending/estimated")]
        Task<EstimateSendingTransactionResponse> EstimateSendingTransactionAsync([Body] EstimateSendingTransactionRequest body);

        /// <summary>
        /// Optional.
        /// Should build the not signed “receiving” transaction. This endpoint should be implemented by the
        /// blockchains, which distinguishes “sending” and “receiving” transactions.
        /// </summary>
        [Post("/api/transactions/receiving/built")]
        Task<BuildReceivingTransactionResponse> BuildReceivingTransactionAsync([Body] BuildReceivingTransactionRequest body);

        /// <summary>
        /// Should broadcast the signed transaction to the blockchain.
        /// </summary>
        [Post("/api/transactions/broadcasted")]
        Task BroadcastTransactionAsync([Body] BroadcastTransactionRequest body);

        /// <summary>
        /// Should return raw transaction by its hash.
        /// </summary>
        [Get("/api/transactions/{transactionHash}/raw")]
        Task<RawTransactionResponse> GetRawTransactionAsync(string transactionHash);
    }
}
