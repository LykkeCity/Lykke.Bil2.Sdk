using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Bil2.Contract.SignService.Requests;
using Lykke.Bil2.Contract.SignService.Responses;
using Lykke.Bil2.Sdk.Exceptions;
using Lykke.Bil2.Sdk.SignService.Models;

namespace Lykke.Bil2.Sdk.SignService.Services
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
        Task<AddressCreationResult> CreateAddressAsync();

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
