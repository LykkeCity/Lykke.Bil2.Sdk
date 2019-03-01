using System.Threading.Tasks;
using Lykke.Bil2.Contract.TransactionsExecutor.Responses;

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
        Task<AddressFormatsResponse> GetFormatsAsync(string address);
    }
}
