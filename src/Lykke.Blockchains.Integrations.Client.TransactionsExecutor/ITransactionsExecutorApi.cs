using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Blockchains.Integrations.Contract.Common;
using Lykke.Blockchains.Integrations.Contract.TransactionsExecutor.Requests;
using Lykke.Blockchains.Integrations.Contract.TransactionsExecutor.Responses;
using Lykke.Blockchains.Integrations.WebClient;
using Lykke.Blockchains.Integrations.WebClient.Exceptions;
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
        /// <exception cref="InternalServerErrorWebApiException">Transient server error</exception>
        /// <exception cref="ApiException">Any other HTTP-related error</exception>
        /// <exception cref="Exception">Any other error</exception>
        [Get("/api/isalive")]
        Task<TransactionsExecutorIsAliveResponse> GetIsAliveAsync();

        /// <summary>
        /// Should return information about running integration. All required information could be gathered synchronously in the call.
        /// </summary>
        /// <exception cref="InternalServerErrorWebApiException">Transient server error</exception>
        /// <exception cref="ApiException">Any other HTTP-related error</exception>
        /// <exception cref="Exception">Any other error</exception>
        [Get("/api/integration-info")]
        Task<IntegrationInfoResponse> GetIntegrationInfoAsync();

        /// <summary>
        /// Should check and return address validity.
        /// </summary>
        /// <exception cref="BadRequestWebApiException">Invalid request parameters</exception>
        /// <exception cref="InternalServerErrorWebApiException">Transient server error</exception>
        /// <exception cref="ApiException">Any other HTTP-related error</exception>
        /// <exception cref="Exception">Any other error</exception>
        [Get("/api/addresses/{address}/validity")]
        Task<AddressValidityResponse> GetAddressValidityAsync(string address, [Query] AddressTagType? tagType = null, [Query] string tag = null);

        /// <summary>
        /// Should build a not signed transaction. For the blockchains where “sending” and “receiving”
        /// transactions are distinguished, this endpoint builds the “sending” transactions.
        /// </summary>
        /// <exception cref="BadRequestWebApiException">
        /// Transaction can’t be built with the given parameters and it will be never possible to
        /// build the transaction with exactly the same parameters.
        /// </exception>
        /// <exception cref="SendingTransactionBuildingWebApiException">
        /// Transaction can't be built. See <see cref="SendingTransactionBuildingWebApiException.ErrorCode"/>
        /// to determine the reason.
        /// </exception>
        /// <exception cref="InternalServerErrorWebApiException">Transient server error</exception>
        /// <exception cref="ApiException">Any other HTTP-related error</exception>
        /// <exception cref="Exception">Any other error</exception>
        [Post("/api/transactions/sending/built")]
        [ExceptionMapper(typeof(SendingTransactionBuildingExceptionMapper))]
        Task<BuildSendingTransactionResponse> BuildSendingTransactionAsync([Body] BuildSendingTransactionRequest body);

        /// <summary>
        /// Should estimate the transaction fee. For the blockchains where “sending” and “receiving”
        /// transactions are distinguished, this endpoint estimates fee for the “sending” transactions.
        /// </summary>
        /// <exception cref="BadRequestWebApiException">
        /// Transaction can’t be estimated with the given parameters and it will be never possible to
        /// estimate the transaction with exactly the same parameters.
        /// </exception>
        /// <exception cref="InternalServerErrorWebApiException">Transient server error</exception>
        /// <exception cref="ApiException">Any other HTTP-related error</exception>
        /// <exception cref="Exception">Any other error</exception>
        [Post("/api/transactions/sending/estimated")]
        Task<EstimateSendingTransactionResponse> EstimateSendingTransactionAsync([Body] EstimateSendingTransactionRequest body);

        /// <summary>
        /// Optional.
        /// Should build the not signed “receiving” transaction. This endpoint should be implemented by the
        /// blockchains, which distinguishes “sending” and “receiving” transactions.
        /// </summary>
        /// <exception cref="BadRequestWebApiException">
        /// Transaction can’t be built with the given parameters. The given “sending” transaction can’t
        /// be received and it will be never possible to receive the given “sending” transaction.
        /// </exception>
        /// <exception cref="InternalServerErrorWebApiException">Transient server error</exception>
        /// <exception cref="NotImplementedWebApiException">Method is not implemented by the blockchain integration</exception>
        /// <exception cref="ApiException">Any other HTTP-related error</exception>
        /// <exception cref="Exception">Any other error</exception>
        [Post("/api/transactions/receiving/built")]
        Task<BuildReceivingTransactionResponse> BuildReceivingTransactionAsync([Body] BuildReceivingTransactionRequest body);

        /// <summary>
        /// Should broadcast the signed transaction to the blockchain.
        /// </summary>
        /// <exception cref="BadRequestWebApiException">
        /// Transaction can’t be broadcasted with the given parameters. It should be
        /// guaranteed that the transaction is not included and will not be included to
        /// the any blockchain block.
        /// </exception>
        /// <exception cref="TransactionBroadcastingWebApiException">
        /// Transaction broadcasting has been failed. See <see cref="TransactionBroadcastingWebApiException.ErrorCode"/>
        /// to determine the reason.
        /// </exception>
        /// <exception cref="InternalServerErrorWebApiException">
        /// Transient server error. It’s not guaranteed if transaction was broadcasted to the blockchain or not.
        /// </exception>
        /// <exception cref="ApiException">Any other HTTP-related error</exception>
        /// <exception cref="Exception">Any other error</exception>
        [Post("/api/transactions/broadcasted")]
        [ExceptionMapper(typeof(TransactionBroadcastingExceptionMapper))]
        Task BroadcastTransactionAsync([Body] BroadcastTransactionRequest body);

        /// <summary>
        /// Should return raw transaction by its hash.
        /// </summary>
        /// <exception cref="BadRequestWebApiException">Invalid request parameters</exception>
        /// <exception cref="InternalServerErrorWebApiException">Transient server error</exception>
        /// <exception cref="ApiException">Any other HTTP-related error</exception>
        /// <exception cref="Exception">Any other error</exception>
        [Get("/api/transactions/{transactionHash}/raw")]
        Task<RawTransactionResponse> GetRawTransactionAsync(string transactionHash);
    }
}
