using JetBrains.Annotations;
using Refit;

namespace Lykke.Blockchains.Integrations.WebClient
{
    /// <summary>
    /// Maps <see cref="ApiException"/> to application-specific exception
    /// </summary>
    [PublicAPI]
    public interface IExceptionMapper
    {
        /// <summary>
        /// Maps <see cref="ApiException"/> to application-specific exception
        /// </summary>
        void ThrowMappedExceptionOrPassThrough(ApiException ex);
    }
}
