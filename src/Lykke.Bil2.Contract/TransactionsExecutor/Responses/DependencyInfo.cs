using System;
using Lykke.Bil2.Contract.Common;
using Newtonsoft.Json;

namespace Lykke.Bil2.Contract.TransactionsExecutor.Responses
{
    /// <summary>
    /// Integration dependency info.
    /// </summary>
    public class DependencyInfo
    {
        /// <summary>
        /// Running version of the dependency.
        /// </summary>
        [JsonProperty("runningVersion")]
        public Semver RunningVersion { get; }

        /// <summary>
        /// Latest available version of the dependency. 
        /// </summary>
        [JsonProperty("latestAvailableVersion")]
        public Semver LatestAvailableVersion { get; }

        /// <summary>
        /// Integration dependency info.
        /// </summary>
        /// <param name="runningVersion">Running version of the dependency.</param>
        /// <param name="latestAvailableVersion">Latest available version of the dependency.</param>
        public DependencyInfo(Semver runningVersion, Semver latestAvailableVersion)
        {
            RunningVersion = runningVersion ?? throw new ArgumentNullException(nameof(runningVersion));
            LatestAvailableVersion = latestAvailableVersion ?? throw new ArgumentNullException(nameof(latestAvailableVersion));
        }
    }
}
