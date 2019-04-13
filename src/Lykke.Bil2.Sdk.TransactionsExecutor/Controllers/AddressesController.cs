using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Bil2.Contract.Common.Exceptions;
using Lykke.Bil2.Contract.TransactionsExecutor.Responses;
using Lykke.Bil2.Sdk.TransactionsExecutor.Services;
using Lykke.Bil2.SharedDomain;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Bil2.Sdk.TransactionsExecutor.Controllers
{
    [ApiController]
    [Route("/api/addresses")]
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    internal class AddressesController : ControllerBase
    {
        private readonly IAddressValidator _addressValidator;
        private readonly IAddressFormatsProvider _addressFormatsProvider;

        public AddressesController(
            IAddressValidator addressValidator,
            IAddressFormatsProvider addressFormatsProvider)
        {
            _addressValidator = addressValidator ?? throw new ArgumentNullException(nameof(addressValidator));
            _addressFormatsProvider = addressFormatsProvider ?? throw new ArgumentNullException(nameof(addressFormatsProvider));
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

        [HttpGet("{address}/formats")]
        public async Task<ActionResult<AddressFormatsResponse>> GetFormats(string address)
        {
            var response = await _addressFormatsProvider.GetFormatsAsync(address);
            if (response == null)
            {
                throw new InvalidOperationException("Not null response object expected");
            }

            return Ok(response);
        }
    }
}
