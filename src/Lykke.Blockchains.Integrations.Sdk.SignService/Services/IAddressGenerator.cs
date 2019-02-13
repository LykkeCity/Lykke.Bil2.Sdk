using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Blockchains.Integrations.Contract.SignService.Requests;
using Lykke.Blockchains.Integrations.Contract.SignService.Responses;
using Lykke.Blockchains.Integrations.Sdk.SignService.Models;

namespace Lykke.Blockchains.Integrations.Sdk.SignService.Services
{
    [PublicAPI]
    public interface IAddressGenerator
    {
        /// <summary>
        /// Should create a new address in the blockchain.
        /// </summary>
        /// <exception cref="OperationNotSupportedException">
        /// Should be thrown by implementation if offline address creation is not supported by the blockchain.
        /// </exception>
        Task<AddressCreationResult> CreateAddresAsync();

        /// <summary>
        /// Should create a new address tag for the specified address.
        /// </summary>
        /// <param name="address">
        /// Address for which a tag being created.
        /// </param>
        /// <param name="request">
        /// Request parameters.
        /// </param>
        /// <exception cref="OperationNotSupportedException">
        /// Should be thrown by implementation if address tag creation is not supported by the blockchain.
        /// </exception>
        Task<CreateAddressTagResponse> CreateAddressTagAsync(string address, CreateAddressTagRequest request);
    }
}
