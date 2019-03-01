using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Bil2.Contract.Common;
using Lykke.Bil2.Contract.Common.Exceptions;
using Lykke.Bil2.Contract.TransactionsExecutor;
using Lykke.Bil2.Contract.TransactionsExecutor.Responses;

namespace Lykke.Bil2.Sdk.TransactionsExecutor.Services
{
    /// <summary>
    /// Address validator
    /// </summary>
    public interface IAddressValidator
    {
        /// <summary>
        /// Should validate an address.
        /// </summary>
        /// <param name="address">
        /// Address.
        /// </param>
        /// <param name="tagType">
        /// Optional.
        /// Type of the address tag
        /// </param>
        /// <param name="tag">
        /// Optional.
        /// Address tag.
        /// </param>
        /// <exception cref="RequestValidationException">
        /// Should be thrown if input parameters are invalid and result can't be mapped to any of <see cref="AddressValidationResult"/> value.
        /// </exception>
        /// <exception cref="Exception">
        /// Includes any other exception types not listed above.
        /// Should be thrown if there are any other errors.
        /// Likely a temporary issue with infrastructure or configuration, request should be repeated later.
        /// </exception>
        Task<AddressValidityResponse> ValidateAsync(string address, [CanBeNull] AddressTagType? tagType = null, [CanBeNull] string tag = null);
    }
}
