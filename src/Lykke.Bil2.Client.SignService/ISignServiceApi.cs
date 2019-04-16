using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Bil2.Contract.Common.Responses;
using Lykke.Bil2.Contract.SignService.Requests;
using Lykke.Bil2.Contract.SignService.Responses;
using Lykke.Bil2.SharedDomain;
using Lykke.Bil2.WebClient.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Refit;

namespace Lykke.Bil2.Client.SignService
{
    /// <summary>
    /// Blockchain integration sign service HTTP API
    /// </summary>
    [PublicAPI]
    public interface ISignServiceApi
    {
        /// <summary>
        /// Should return some general service info. Used to check if the service is running.
        /// </summary>
        /// <exception cref="InternalServerErrorWebApiException">Transient server error</exception>
        /// <exception cref="WebApiException">Any other HTTP-related error</exception>
        /// <exception cref="Exception">Any other error</exception>
        [Get("/api/isalive")]
        Task<BlockchainIsAliveResponse> GetIsAliveAsync();

        /// <summary>
        /// Should create a new address in the blockchain.
        /// </summary>
        /// <exception cref="BadRequestWebApiException">Invalid request parameters</exception>
        /// <exception cref="InternalServerErrorWebApiException">Transient server error</exception>
        /// <exception cref="NotImplementedWebApiException">
        /// Offline address creation is not supported by the blockchain integration.
        /// </exception>
        /// <exception cref="WebApiException">Any other HTTP-related error</exception>
        /// <exception cref="Exception">Any other error</exception>
        [Post("/api/addresses")]
        Task<CreateAddressResponse> CreateAddressAsync([Body] CreateAddressRequest body);

        /// <summary>
        /// Should create a new address tag for the specified address.
        /// </summary>
        /// <exception cref="BadRequestWebApiException">Invalid request parameters</exception>
        /// <exception cref="InternalServerErrorWebApiException">Transient server error</exception>
        /// <exception cref="NotImplementedWebApiException">
        /// Address tag creation is not supported by the blockchain integration.
        /// </exception>
        /// <exception cref="WebApiException">Any other HTTP-related error</exception>
        /// <exception cref="Exception">Any other error</exception>
        [Post("/api/addresses/{address}/tags")]
        Task<CreateAddressTagResponse> CreateAddressTagAsync(Address address, [Body] CreateAddressTagRequest body);

        /// <summary>
        /// Should sign the given transaction with the specified private keys.
        /// </summary>
        /// <exception cref="BadRequestWebApiException">Invalid request parameters</exception>
        /// <exception cref="InternalServerErrorWebApiException">Transient server error</exception>
        /// <exception cref="WebApiException">Any other HTTP-related error</exception>
        /// <exception cref="Exception">Any other error</exception>
        [Post("/api/transactions/signed")]
        Task<SignTransactionResponse> SignTransactionAsync([Body] SignTransactionRequest body);
    }
}
