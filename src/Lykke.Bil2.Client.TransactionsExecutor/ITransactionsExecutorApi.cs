using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Bil2.Client.TransactionsExecutor.Exceptions;
using Lykke.Bil2.Client.TransactionsExecutor.ExceptionsMappers;
using Lykke.Bil2.Contract.Common;
using Lykke.Bil2.Contract.TransactionsExecutor.Requests;
using Lykke.Bil2.Contract.TransactionsExecutor.Responses;
using Lykke.Bil2.WebClient;
using Lykke.Bil2.WebClient.Exceptions;
using Refit;

namespace Lykke.Bil2.Client.TransactionsExecutor
{
    /// <summary>
    /// Blockchain integration transactions executor HTTP API
    /// </summary>
    [PublicAPI]
    public interface ITransactionsExecutorApi
    {
        #region General

        /// <summary>
        /// Should return some general service info. Used to check if the service is running.
        /// </summary>
        /// <exception cref="InternalServerErrorWebApiException">Transient server error</exception>
        /// <exception cref="WebApiException">Any other HTTP-related error</exception>
        /// <exception cref="Exception">Any other error</exception>
        [Get("/api/isalive")]
        Task<TransactionsExecutorIsAliveResponse> GetIsAliveAsync();

        /// <summary>
        /// Should return information about running integration. All required information could be gathered synchronously in the call.
        /// </summary>
        /// <exception cref="InternalServerErrorWebApiException">Transient server error</exception>
        /// <exception cref="WebApiException">Any other HTTP-related error</exception>
        /// <exception cref="Exception">Any other error</exception>
        [Get("/api/integration-info")]
        Task<IntegrationInfoResponse> GetIntegrationInfoAsync();

        #endregion


        #region Addresses

        /// <summary>
        /// Should check and return address validity.
        /// </summary>
        /// <exception cref="BadRequestWebApiException">Invalid request parameters</exception>
        /// <exception cref="InternalServerErrorWebApiException">Transient server error</exception>
        /// <exception cref="WebApiException">Any other HTTP-related error</exception>
        /// <exception cref="Exception">Any other error</exception>
        [Get("/api/addresses/{address}/validity")]
        Task<AddressValidityResponse> GetAddressValidityAsync(string address, [Query] AddressTagType? tagType = null, [Query] string tag = null);
        
        /// <summary>
        /// Optional.
        /// Should return all formats of the given <paramref name="address"/>.
        /// </summary>
        /// <exception cref="BadRequestWebApiException">Invalid request parameters</exception>
        /// <exception cref="InternalServerErrorWebApiException">Transient server error</exception>
        /// <exception cref="NotImplementedWebApiException">Method is not implemented by the blockchain integration</exception>
        /// <exception cref="WebApiException">Any other HTTP-related error</exception>
        /// <exception cref="Exception">Any other error</exception>
        [Get("/api/addresses/{address}/formats")]
        Task<AddressFormatsResponse> GetAddressFormatsAsync(string address);

        #endregion


        #region Transactions building

        /// <summary>
        /// "Transfer amount" transactions model.
        /// Should build a not signed transaction which sends funds if integration uses “transfer amount” transactions model.
        /// Integration should either support “transfer coins”  or “transfer amount” transactions model.
        /// </summary>
        /// <exception cref="BadRequestWebApiException">
        /// Transaction can’t be built with the given parameters and it will never be possible to
        /// build the transaction with exactly the same parameters.
        /// </exception>
        /// <exception cref="TransactionBuildingWebApiException">
        /// Transaction can't be built. See <see cref="TransactionBuildingWebApiException.ErrorCode"/>
        /// to determine the reason.
        /// </exception>
        /// <exception cref="InternalServerErrorWebApiException">Transient server error</exception>
        /// <exception cref="NotImplementedWebApiException">Method is not implemented by the blockchain integration</exception>
        /// <exception cref="WebApiException">Any other HTTP-related error</exception>
        /// <exception cref="Exception">Any other error</exception>
        [Post("/api/transactions/built/transfers/amount")]
        [ExceptionMapper(typeof(TransactionBuildingExceptionMapper))]
        Task<BuildTransactionResponse> BuildTransferAmountTransactionAsync([Body] BuildTransferAmountTransactionRequest body);

        /// <summary>
        /// "Transfer coins" transactions model.
        /// Should build a not signed transaction which sends funds if integration uses “transfer coins” transactions model.
        /// Integration should either support “transfer coins”  or “transfer amount” transactions model.
        /// </summary>
        /// <exception cref="BadRequestWebApiException">
        /// Transaction can’t be built with the given parameters and it will never be possible to
        /// build the transaction with exactly the same parameters.
        /// </exception>
        /// <exception cref="TransactionBuildingWebApiException">
        /// Transaction can't be built. See <see cref="TransactionBuildingWebApiException.ErrorCode"/>
        /// to determine the reason.
        /// </exception>
        /// <exception cref="InternalServerErrorWebApiException">Transient server error</exception>
        /// <exception cref="NotImplementedWebApiException">Method is not implemented by the blockchain integration</exception>
        /// <exception cref="WebApiException">Any other HTTP-related error</exception>
        /// <exception cref="Exception">Any other error</exception>
        [Post("/api/transactions/built/transfers/coins")]
        [ExceptionMapper(typeof(TransactionBuildingExceptionMapper))]
        Task<BuildTransactionResponse> BuildTransferCoinsTransactionAsync([Body] BuildTransferCoinsTransactionRequest body);

        #endregion


        #region Transactions fee estimation

        /// <summary>
        /// "Transfer amount" transactions model.
        /// Should estimate the transaction fee if integration uses “transfer amount” transactions model.
        /// Integration should either support “transfer coins”  or “transfer amount” transactions model.
        /// </summary>
        /// <exception cref="BadRequestWebApiException">
        /// Transaction can’t be estimated with the given parameters and it will never be possible to
        /// estimate the transaction with exactly the same parameters.
        /// </exception>
        /// <exception cref="InternalServerErrorWebApiException">Transient server error</exception>
        /// <exception cref="WebApiException">Any other HTTP-related error</exception>
        /// <exception cref="Exception">Any other error</exception>
        [Post("/api/transactions/estimated/transfers/amount")]
        Task<EstimateTransactionResponse> EstimateTransferAmountTransactionAsync([Body] EstimateTransferAmountTransactionRequest body);

        /// <summary>
        /// "Transfer coins" transactions model.
        /// Should estimate the transaction fee if integration uses “transfer coins” transactions model.
        /// Integration should either support “transfer coins”  or “transfer amount” transactions model.
        /// </summary>
        /// <exception cref="BadRequestWebApiException">
        /// Transaction can’t be estimated with the given parameters and it will never be possible to
        /// estimate the transaction with exactly the same parameters.
        /// </exception>
        /// <exception cref="InternalServerErrorWebApiException">Transient server error</exception>
        /// <exception cref="WebApiException">Any other HTTP-related error</exception>
        /// <exception cref="Exception">Any other error</exception>
        [Post("/api/transactions/estimated/transfers/coins")]
        Task<EstimateTransactionResponse> EstimateTransferCoinsTransactionAsync([Body] EstimateTransferCoinsTransactionRequest body);

        #endregion


        #region Transactions broadcasting

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
        /// <exception cref="WebApiException">Any other HTTP-related error</exception>
        /// <exception cref="Exception">Any other error</exception>
        [Post("/api/transactions/broadcasted")]
        [ExceptionMapper(typeof(TransactionBroadcastingExceptionMapper))]
        Task BroadcastTransactionAsync([Body] BroadcastTransactionRequest body);

        #endregion


        #region Raw Transactions

        /// <summary>
        /// Should return raw transaction by its id.
        /// </summary>
        /// <exception cref="BadRequestWebApiException">Invalid request parameters</exception>
        /// <exception cref="NotFoundWebApiException">Transaction is not found</exception>
        /// <exception cref="InternalServerErrorWebApiException">Transient server error</exception>
        /// <exception cref="WebApiException">Any other HTTP-related error</exception>
        /// <exception cref="Exception">Any other error</exception>
        [Get("/api/transactions/{transactionId}/raw")]
        Task<RawTransactionResponse> GetRawTransactionAsync(string transactionId);

        /// <summary>
        /// Should return transaction state by its id.
        /// </summary>
        /// <exception cref="BadRequestWebApiException">Invalid request parameters</exception>
        /// <exception cref="InternalServerErrorWebApiException">Transient server error</exception>
        /// <exception cref="WebApiException">Any other HTTP-related error</exception>
        /// <exception cref="Exception">Any other error</exception>
        [Get("/api/transactions/{transactionId}/state")]
        Task<TransactionStateResponse> GetTransactionStateAsync(string transactionId);

        #endregion
    }
}
