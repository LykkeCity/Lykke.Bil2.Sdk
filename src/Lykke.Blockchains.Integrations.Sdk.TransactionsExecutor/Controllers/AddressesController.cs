using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Blockchains.Integrations.Contract.Common;
using Lykke.Blockchains.Integrations.Contract.TransactionsExecutor.Responses;
using Lykke.Blockchains.Integrations.Sdk.TransactionsExecutor.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Blockchains.Integrations.Sdk.TransactionsExecutor.Controllers
{
    [ApiController]
    [Route("/api/addresses")]
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    internal class AddressesController : ControllerBase
    {
        private readonly IAddressValidator _addressValidator;

        public AddressesController(IAddressValidator addressValidator)
        {
            _addressValidator = addressValidator ?? throw new ArgumentNullException(nameof(addressValidator));
        }

        [HttpGet("{address}/validity")]
        public async Task<ActionResult<AddressValidityResponse>> Validate(
            string address, 
            [FromQuery] AddressTagType? tagType = null, 
            [FromQuery] string tag = null)
        {
            if (tagType.HasValue && tag == null)
                throw new RequestValidationException("If the tag type is specified, the tag should be specified too", new [] {nameof(tagType), nameof(tag)});

            var response = await _addressValidator.ValidateAsync(address, tagType, tag);
            if (response == null)
            {
                throw new InvalidOperationException("Not null response object expected");
            }

            return Ok(response);
        }
    }
}
