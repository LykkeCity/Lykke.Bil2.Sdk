using System;
using JetBrains.Annotations;
using Lykke.Blockchains.Integrations.Contract.Common;
using Lykke.Blockchains.Integrations.Contract.Common.Responses;
using Lykke.Common;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Blockchains.Integrations.Sdk.Controllers
{
    [ApiController]
    [Route("/api/isalive")]
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    internal class IsAliveController : ControllerBase
    {
        [HttpGet]
        public ActionResult<BlockchainIsAliveResponse> GetIsAlive()
        {
            var response = new BlockchainIsAliveResponse(
                AppEnvironment.Name,
                new Version(AppEnvironment.Version),
                AppEnvironment.EnvInfo,
                Constants.ContractVersion);

            return Ok(response);
        }
    }
}
