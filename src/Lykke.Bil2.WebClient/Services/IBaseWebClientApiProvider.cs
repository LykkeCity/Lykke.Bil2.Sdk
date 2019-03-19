using JetBrains.Annotations;

namespace Lykke.Bil2.WebClient.Services
{
    /// <summary>
    /// Base provider of the integration web client API.
    /// </summary>
    /// <typeparam name="TApi">Web client API interface</typeparam>
    [PublicAPI]
    public interface IBaseWebClientApiProvider<out TApi>
    {
        /// <summary>
        /// Returns web client API instance for the specified <param name="integrationName"></param>.
        /// </summary>
        TApi GetApi(string integrationName);
    }
}
