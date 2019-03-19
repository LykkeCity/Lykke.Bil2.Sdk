using System.Collections.Generic;
using Lykke.Bil2.WebClient.Services;

namespace Lykke.Bil2.Client.SignService.Services
{
    internal class SignServiceApiProvider : 
        BaseWebClientApiProvider<ISignServiceApi>,
        ISignServiceApiProvider
    {
        public SignServiceApiProvider(IReadOnlyDictionary<string, ISignServiceApi> integrationApis) : 
            base(integrationApis)
        {
        }
    }
}
