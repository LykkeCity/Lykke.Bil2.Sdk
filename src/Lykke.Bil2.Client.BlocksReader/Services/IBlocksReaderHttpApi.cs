using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Bil2.Contract.Common.Responses;
using Lykke.Bil2.WebClient.Exceptions;
using Refit;

namespace Lykke.Bil2.Client.BlocksReader.Services
{
    /// <summary>
    /// Blockchain integration blocks reader HTTP API
    /// </summary>
    [PublicAPI]
    public interface IBlocksReaderHttpApi
    {
        /// <summary>
        /// Should return some general service info. Used to check if the service is running.
        /// </summary>
        /// <exception cref="InternalServerErrorWebApiException">Transient server error</exception>
        /// <exception cref="WebApiException">Any other HTTP-related error</exception>
        /// <exception cref="Exception">Any other error</exception>
        [Get("/api/isalive")]
        Task<BlockchainIsAliveResponse> GetIsAliveAsync();
    }
}
