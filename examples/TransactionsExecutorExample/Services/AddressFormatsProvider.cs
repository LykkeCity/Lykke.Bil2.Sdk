using System;
using System.Threading.Tasks;
using Lykke.Bil2.Contract.Common.Exceptions;
using Lykke.Bil2.Contract.TransactionsExecutor.Responses;
using Lykke.Bil2.Sdk.TransactionsExecutor.Services;

namespace TransactionsExecutorExample.Services
{
    public class AddressFormatsProvider : IAddressFormatsProvider
    {
        public Task<AddressFormatsResponse> GetFormatsAsync(string address)
        {
            if (!Guid.TryParse(address, out var addressGuid))
            {
                throw new RequestValidationException("Address format is invalid", address, nameof(address));
            }

            return Task.FromResult(new AddressFormatsResponse(new[]
            {
                new AddressFormat(addressGuid.ToString("N")),
                new AddressFormat(addressGuid.ToString("D"), "Legacy format"),
                new AddressFormat(addressGuid.ToString("B"), "Super format"),
                new AddressFormat(addressGuid.ToString("P"), "Ultra format"),
            }));
        }
    }
}
