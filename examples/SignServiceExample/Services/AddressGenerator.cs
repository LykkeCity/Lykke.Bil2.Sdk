using System;
using System.Threading.Tasks;
using Lykke.Bil2.Contract.Common;
using Lykke.Bil2.Contract.SignService.Requests;
using Lykke.Bil2.Contract.SignService.Responses;
using Lykke.Bil2.Sdk.Exceptions;
using Lykke.Bil2.Sdk.SignService.Models;
using Lykke.Bil2.Sdk.SignService.Services;
using SignServiceExample.Settings;

namespace SignServiceExample.Services
{
    internal class AddressGenerator : IAddressGenerator
    {
        private readonly NetworkType _network;

        public AddressGenerator(NetworkType network)
        {
            _network = network;
        }

        public Task<AddressCreationResult> CreateAddressAsync()
        {
            var address = $"{_network}:{Guid.NewGuid():N}";
            var privateKey = Guid.NewGuid().ToString("N").Substring(0, 8);
            var context = Guid.NewGuid().ToString("N").Substring(0, 4);

            Console.WriteLine($"Generated address: {address}, privateKey: {privateKey}");

            return Task.FromResult(new AddressCreationResult(address, privateKey, context.ToBase58()));
        }

        public Task<CreateAddressTagResponse> CreateAddressTagAsync(string address, CreateAddressTagRequest request)
        {
            throw new OperationNotSupportedException("Address tag is not supported");
        }
    }
}
