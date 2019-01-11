﻿using System;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Lykke.Blockchains.Integrations.Contract.Common.Responses
{
    /// <summary>
    /// Endpoint: [GET] /api/isalive
    /// </summary>
    [PublicAPI]
    public class IsAliveResponse
    {
        /// <summary>
        /// Name of the service 
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; }

        /// <summary>
        /// Version of the service - version of the executing assembly. 
        /// </summary>
        [JsonProperty("version")]
        [JsonConverter(typeof(VersionConverter))]
        public Version Version { get; }
 
        /// <summary>
        /// ENV_INFO environment variable value
        /// </summary>
        [JsonProperty("env")]
        public string EnvInfo { get; }
 
        /// <summary>
        /// Flag, which indicates if the service is built in the debug configuration or not
        /// </summary>
        [JsonProperty("isDebug")]
        public bool IsDebug { get; }
       
        /// <summary>
        /// Should return implemented contract version. For example: “2.0.0”
        /// </summary>
        [JsonProperty("contractVersion")]
        [JsonConverter(typeof(VersionConverter))]
        public Version ContractVersion { get; }

        public IsAliveResponse(
            string name,
            Version version,
            string envInfo,
            bool isDebug,
            Version contractVersion)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Should be not empty string", nameof(name));

            if (string.IsNullOrWhiteSpace(envInfo))
                throw new ArgumentException("Should be not empty string", nameof(envInfo));

            Name = name;
            Version = version;
            EnvInfo = envInfo;
            IsDebug = isDebug;
            ContractVersion = contractVersion;
        }
    }
}
