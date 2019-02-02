using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Blockchains.Integrations.Contract.Common.Responses;
using Lykke.Blockchains.Integrations.Contract.SignService.Requests;
using Lykke.Blockchains.Integrations.Contract.SignService.Responses;
using Refit;

namespace Lykke.Blockchains.Integrations.Client.SignService
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
        [Get("/api/isalive")]
        Task<BlockchainIsAliveResponse> GetIsAliveAsync();

        /// <summary>
        /// Should create a new address in the blockchain.
        /// </summary>
        [Post("/api/addresses")]
        Task<CreateAddressResponse> CreateAddressAsync([Body] CreateAddressRequest body);

        /// <summary>
        /// Should create a new address tag for the specified address.
        /// </summary>
        [Post("/api/addresses/{address}/tags")]
        Task<CreateAddressTagResponse> CreateAddressTagAsync(string address, [Body] CreateAddressTagRequest body);

        /// <summary>
        /// Should sign the given transaction with the specified private keys.
        /// </summary>
        [Post("/api/transactions/signed")]
        Task<SignTransactionResponse> SignTransactionAsync([Body] SignTransactionRequest body);
    }
}
