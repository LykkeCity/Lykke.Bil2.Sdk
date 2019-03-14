using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

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
        [JsonConverter(typeof(VersionConverter))]
        public Version RunningVersion { get; }

        /// <summary>
        /// Latest available version of the dependency. 
        /// </summary>
        [JsonProperty("latestAvailableVersion")]
        [JsonConverter(typeof(VersionConverter))]
        public Version LatestAvailableVersion { get; }

        /// <summary>
        /// Integration dependency info.
        /// </summary>
        /// <param name="runningVersion">Running version of the dependency.</param>
        /// <param name="latestAvailableVersion">Latest available version of the dependency.</param>
        public DependencyInfo(Version runningVersion, Version latestAvailableVersion)
        {
            RunningVersion = runningVersion;
            LatestAvailableVersion = latestAvailableVersion;
        }
    }
}
