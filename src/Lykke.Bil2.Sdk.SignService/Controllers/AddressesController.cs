using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Bil2.Contract.Common;
using Lykke.Bil2.Contract.SignService.Requests;
using Lykke.Bil2.Contract.SignService.Responses;
using Lykke.Bil2.Sdk.SignService.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Bil2.Sdk.SignService.Controllers
{
    [ApiController]
    [Route("/api/addresses")]
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    internal class AddressesController : ControllerBase
    {
        private readonly IAddressGenerator _addressGenerator;

        public AddressesController(IAddressGenerator addressGenerator)
        {
            _addressGenerator = addressGenerator ?? throw new ArgumentNullException(nameof(addressGenerator));
        }

        [HttpPost]
        public async Task<ActionResult<CreateAddressResponse>> CreateAddress(CreateAddressRequest request)
        {
            var creationResult = await _addressGenerator.CreateAddressAsync();
            if (creationResult == null)
            {
                throw new InvalidOperationException("Not null creation result object expected");
            }

            var encryptedPrivateKey = creationResult.PrivateKey.Encrypt(request.EncryptionPublicKey);
            
            var response = new CreateAddressResponse
            (
                encryptedPrivateKey,
                creationResult.Address,
                creationResult.AddressContext
            );

            return Ok(response);
        }

        [HttpPost("{address}/tags")]
        public async Task<ActionResult<CreateAddressTagResponse>> CreateAddressTag(string address, CreateAddressTagRequest request)
        {
            if (string.IsNullOrWhiteSpace(address))
                throw RequestValidationException.ShouldBeNotEmptyString(nameof(address));

            var response = await _addressGenerator.CreateAddressTagAsync(address, request);
            if (response == null)
            {
                throw new InvalidOperationException("Not null response object expected");
            }
            
            return Ok(response);
        }
    }
}
