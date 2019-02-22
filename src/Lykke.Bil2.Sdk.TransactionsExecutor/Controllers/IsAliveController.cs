using System;
using JetBrains.Annotations;
using Lykke.Bil2.Contract.Common;
using Lykke.Bil2.Contract.Common.Responses;
using Lykke.Bil2.Contract.TransactionsExecutor.Responses;
using Lykke.Bil2.Sdk.TransactionsExecutor.Services;
using Lykke.Common;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Bil2.Sdk.TransactionsExecutor.Controllers
{
    [ApiController]
    [Route("/api/isalive")]
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    internal class IsAliveController : ControllerBase
    {
        private readonly IHealthMonitor _healthMonitor;

        public IsAliveController(IHealthMonitor healthMonitor)
        {
            _healthMonitor = healthMonitor ?? throw new ArgumentNullException(nameof(healthMonitor));
        }

        [HttpGet]
        public ActionResult<BlockchainIsAliveResponse> GetIsAlive()
        {
            var disease = _healthMonitor.Disease;
            var response = new TransactionsExecutorIsAliveResponse(
                AppEnvironment.Name,
                new Version(AppEnvironment.Version),
                AppEnvironment.EnvInfo,
                Constants.ContractVersion,
                disease);

            return Ok(response);
        }
    }
}
