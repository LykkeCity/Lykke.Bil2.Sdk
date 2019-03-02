using System;
using System.Threading.Tasks;
using Lykke.Bil2.Contract.Common.Exceptions;
using Lykke.Bil2.Contract.TransactionsExecutor.Responses;
using Lykke.Bil2.Sdk.Exceptions;

namespace Lykke.Bil2.Sdk.TransactionsExecutor.Services
{
    /// <summary>
    /// Address formats provider.
    /// </summary>
    /// <remarks>
    /// May be implemented only if blockchain has several formats of the same address.
    /// Use <see cref="TransactionsExecutorServiceOptions{TAppSettings}.AddressFormatsProviderFactory"/> to assign your implementation.
    /// </remarks>
    public interface IAddressFormatsProvider
    {
        /// <summary>
        /// Should return all formats of the <paramref name="address"/>.
        /// </summary>
        /// <exception cref="RequestValidationException">
        /// Should be thrown if input parameters are invalid.
        /// </exception>
        /// <exception cref="OperationNotSupportedException">
        /// Should be thrown by implementation if multiple address formats are not supported by the blockchain.
        /// </exception>
        /// <exception cref="Exception">
        /// Includes any other exception types not listed above.
        /// Should be thrown if there are any other errors.
        /// Likely a temporary issue with infrastructure or configuration, request should be repeated later.
        /// </exception>
        Task<AddressFormatsResponse> GetFormatsAsync(string address);
    }
}
