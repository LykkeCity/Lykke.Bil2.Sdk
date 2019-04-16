using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Bil2.Contract.Common.Extensions;
using Lykke.Bil2.Contract.SignService.Responses;
using Lykke.Bil2.Sdk.SignService.Services;
using Lykke.Bil2.SharedDomain;
using Newtonsoft.Json;
using SignServiceExample.Settings;

namespace SignServiceExample.Services
{
    internal class TransactionSigner : ITransactionSigner
    {
        private readonly NetworkType _network;

        public TransactionSigner(NetworkType network)
        {
            _network = network;
        }

        public Task<SignTransactionResponse> SignAsync(IReadOnlyCollection<string> privateKeys, Base58String requestTransactionContext)
        {
            var id = requestTransactionContext.Value.GetHashCode().ToString("X8");
            var signed = new
            {
                Context = requestTransactionContext.Value,
                PrivateKey = privateKeys.FirstOrDefault(),
                Network = _network
            };

            Console.WriteLine($"Signed with private keys: [{string.Join(", ", privateKeys)}]");

            var serializedSigned = JsonConvert.SerializeObject(signed).ToBase58();

            return Task.FromResult(new SignTransactionResponse(serializedSigned, id));
        }
    }
}
