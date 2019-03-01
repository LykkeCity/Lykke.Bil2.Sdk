using System.Threading.Tasks;
using Lykke.Bil2.Contract.TransactionsExecutor.Responses;
using Lykke.Bil2.Sdk.Exceptions;

namespace Lykke.Bil2.Sdk.TransactionsExecutor.Services
{
    internal class NotSupportedAddressFormatsProvider : IAddressFormatsProvider
    {
        public Task<AddressFormatsResponse> GetFormatsAsync(string address)
        {
            throw new OperationNotSupportedException("Integration does not support multiple formats for an address.");
        }
    }
}