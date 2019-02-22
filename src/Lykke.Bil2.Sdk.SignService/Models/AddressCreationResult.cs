using System;
using JetBrains.Annotations;
using Lykke.Bil2.Contract.Common;

namespace Lykke.Bil2.Sdk.SignService.Models
{
    [PublicAPI]
    public class AddressCreationResult
    {
        public string Address { get; }
        public string PrivateKey { get; }
        public Base58String AddressContext { get; }

        public AddressCreationResult(string address, string privateKey, Base58String addressContext = null)
        {
            if (string.IsNullOrWhiteSpace(address))
                throw new ArgumentException("Should be not empty string", nameof(address));

            PrivateKey = privateKey ?? throw new ArgumentNullException(nameof(privateKey));
            Address = address;
            AddressContext = addressContext;
        }
    }
}
