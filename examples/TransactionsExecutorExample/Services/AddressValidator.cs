using System;
using System.Threading.Tasks;
using Lykke.Blockchains.Integrations.Contract.Common;
using Lykke.Blockchains.Integrations.Contract.TransactionsExecutor;
using Lykke.Blockchains.Integrations.Contract.TransactionsExecutor.Responses;
using Lykke.Blockchains.Integrations.Sdk.TransactionsExecutor.Services;

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
            var isAddressForatValid = addressParts.Length == 2 && Guid.TryParse(addressParts[1], out _);
            
            return Task.FromResult(!isAddressForatValid
                ? new AddressValidityResponse(AddressValidationResult.InvalidAddressFormat)
                : new AddressValidityResponse(AddressValidationResult.Valid));
        }
    }
}
