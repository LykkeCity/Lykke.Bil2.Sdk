using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Bil2.Contract.TransactionsExecutor.Responses;
using Lykke.Bil2.Sdk.TransactionsExecutor.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Bil2.Sdk.TransactionsExecutor.Controllers
{
    [ApiController]
    [Route("/api/integration-info")]
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    internal class IntegrationInfoController : ControllerBase
    {
        private readonly IBlockchainInfoProvider _blockchainInfoProvider;
        private readonly IDependenciesInfoProvider _dependenciesInfoProvider;

        public IntegrationInfoController(
            IBlockchainInfoProvider blockchainInfoProvider,
            IDependenciesInfoProvider dependenciesInfoProvider)
        {
            _blockchainInfoProvider = blockchainInfoProvider;
            _dependenciesInfoProvider = dependenciesInfoProvider;
        }

        [HttpGet]
        public async Task<ActionResult<IntegrationInfoResponse>> GetInfo()
        {
            var blockchainInfoTask = _blockchainInfoProvider.GetInfoAsync();
            var dependenciesInfoTask = _dependenciesInfoProvider.GetInfoAsync();

            var blockchainInfo = await blockchainInfoTask;
            if (blockchainInfo == null)
            {
                throw new InvalidOperationException("Not null blockchain info expected");
            }

            var dependenciesInfo = await dependenciesInfoTask;
            if (dependenciesInfo == null)
            {
                throw new InvalidOperationException("Not null dependencies info expected");
            }

            var response = new IntegrationInfoResponse
            (
                blockchainInfo,
                dependenciesInfo
            );

            return Ok(response);
        }
    }
}
