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
        /// Result of the address validation.
        /// </summary>
        [JsonProperty("result")]
        public AddressValidationResult Result { get; }

        public AddressValidityResponse(AddressValidationResult result)
        {
            Result = result;
        }
    }
}
