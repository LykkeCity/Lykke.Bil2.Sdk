using System;
using System.Threading.Tasks;
using Lykke.Blockchains.Integrations.Contract.SignService.Requests;
using Lykke.Blockchains.Integrations.Contract.SignService.Responses;
using Lykke.Blockchains.Integrations.Sdk;
using Lykke.Blockchains.Integrations.Sdk.SignService.Models;
using Lykke.Blockchains.Integrations.Sdk.SignService.Services;

namespace SignServiceExample.Services
{
    internal class AddressGenerator : IAddressGenerator
    {
        public Task<AddressCreationResult> CreateAddresAsync()
        {
            var address = Guid.NewGuid().ToString("N");
            var privateKey = Guid.NewGuid().ToString("N").Substring(0, 8);
            var context = Guid.NewGuid().ToString("N").Substring(0, 4);

            Console.WriteLine($"Generated address: {address}, privateKey: {privateKey}");

            return Task.FromResult(new AddressCreationResult(address, privateKey, context));
        }

        public Task<CreateAddressTagResponse> CreateAddressTagAsync(string address, CreateAddressTagRequest request)
        {
            throw new OperationNotSupportedException("Address tag is not supported");
        }
    }
}
