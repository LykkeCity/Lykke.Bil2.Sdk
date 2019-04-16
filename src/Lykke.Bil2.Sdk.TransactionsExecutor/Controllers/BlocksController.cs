using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Bil2.Contract.Common.Exceptions;
using Lykke.Bil2.Contract.Common.Responses;
using Lykke.Bil2.Contract.TransactionsExecutor.Responses;
using Lykke.Bil2.Sdk.Repositories;
using Lykke.Bil2.Sdk.TransactionsExecutor.Repositories;
using Lykke.Bil2.SharedDomain;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Bil2.Sdk.TransactionsExecutor.Controllers
{
    [ApiController]
    [Route(("/api/blocks"))]
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    internal class BlocksController : ControllerBase
    {
        private readonly IRawObjectReadOnlyRepository _rawObjectsRepository;

        public BlocksController(IRawObjectReadOnlyRepository rawObjectsRepository)
        {
            _rawObjectsRepository = rawObjectsRepository;
        }

        [HttpGet("{blockId}/raw")]
        public async Task<ActionResult<RawObjectResponse>> GetRaw([FromRoute] BlockId blockId)
        {
            if (blockId == null)
                throw RequestValidationException.ShouldBeNotNull(nameof(blockId));

            var raw = await _rawObjectsRepository.GetOrDefaultAsync(RawObjectType.Block, blockId);
            if (raw == null)
            {
                return NotFound(BlockchainErrorResponse.Create($"Raw block [{blockId}] not found"));
            }

            return Ok(new RawObjectResponse(raw));
        }
    }
}
