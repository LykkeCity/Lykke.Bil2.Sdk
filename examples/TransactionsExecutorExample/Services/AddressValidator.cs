using System;
using System.Threading.Tasks;
using Lykke.Bil2.Contract.Common.Exceptions;
using Lykke.Bil2.Contract.TransactionsExecutor.Responses;
using Lykke.Bil2.Sdk.TransactionsExecutor.Services;
using Lykke.Bil2.SharedDomain;

namespace TransactionsExecutorExample.Services
{
    public class AddressValidator : IAddressValidator
    {
        public Task<AddressValidityResponse> ValidateAsync(string address, AddressTagType? tagType = null, string tag = null)
        {
            if (tag != null)
            {
                throw new RequestValidationException("Address tag is not supported", tag, nameof(tag));
            }
            if (tagType != null)
            {
                throw new RequestValidationException("Address tagType is not supported", tagType, nameof(tagType));
            }

            var addressParts = address.Split(":");
            var isAddressFormatValid = addressParts.Length == 2 && Guid.TryParse(addressParts[1], out _);
            
            return Task.FromResult(!isAddressFormatValid
                ? new AddressValidityResponse(AddressValidationResult.InvalidAddressFormat)
                : new AddressValidityResponse(AddressValidationResult.Valid));
        }
    }
}
