using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Lykke.Bil2.Contract.TransactionsExecutor.Responses
{
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

        public DependencyInfo(Version runningVersion, Version latestAvailableVersion)
        {
            RunningVersion = runningVersion;
            LatestAvailableVersion = latestAvailableVersion;
        }
    }
}
