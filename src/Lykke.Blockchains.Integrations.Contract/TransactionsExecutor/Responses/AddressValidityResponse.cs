using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Lykke.Blockchains.Integrations.Contract.TransactionsExecutor.Responses
{
    /// <summary>
    /// Endpoint: [GET] /api/addresses/{address}/validity?[tag=string]
    /// </summary>
    [PublicAPI]
    public class AddressValidityResponse
    {
        /// <summary>
        /// Flag, which indicates if the address is valid.
        /// </summary>
        [JsonProperty("isValid")]
        public bool IsValid { get; }

        public AddressValidityResponse(bool isValid)
        {
            IsValid = isValid;
        }
    }
}
