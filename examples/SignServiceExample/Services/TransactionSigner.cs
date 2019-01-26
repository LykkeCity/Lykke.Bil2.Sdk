using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Blockchains.Integrations.Contract.Common;
using Lykke.Blockchains.Integrations.Contract.SignService.Responses;
using Lykke.Blockchains.Integrations.Sdk.SignService.Services;
using Newtonsoft.Json;

namespace SignServiceExample.Services
{
    internal class TransactionSigner : ITransactionSigner
    {
        public Task<SignTransactionResponse> SignAsync(IReadOnlyCollection<string> privateKeys, Base58String requestTransactionContext)
        {
            var hash = requestTransactionContext.Value.GetHashCode().ToString("X8");
            var signed = new
            {
                Context = requestTransactionContext.Value,
                PrivateKey = privateKeys.FirstOrDefault()
            };

            Console.WriteLine($"Signed with private keys: [{string.Join(", ", privateKeys)}]");

            var serializedSigned = JsonConvert.SerializeObject(signed);

            return Task.FromResult(new SignTransactionResponse(serializedSigned, hash));
        }
    }
}
