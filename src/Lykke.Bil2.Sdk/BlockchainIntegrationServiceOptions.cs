using System;
using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;
using Lykke.Bil2.Sdk.Controllers;
using Lykke.Sdk;
using Lykke.SettingsReader;

namespace Lykke.Bil2.Sdk
{
    /// <summary>
    /// Options of the blockchain integration service container.
    /// </summary>
    /// <typeparam name="TAppSettings"></typeparam>
    [PublicAPI]
    public class BlockchainIntegrationServiceOptions<TAppSettings>
    {
        /// <summary>
        /// Required.
        /// Name of the blockchain service.
        /// </summary>
        public string ServiceName { get; set; }

        /// <summary>
        /// Optional.
        /// Name of the Azure table for logs.
        /// Call <see cref="DisableLogging"/> if wan't to specify this option.
        /// </summary>
        [CanBeNull]
        public string LogsAzureTableName { get; set; }

        /// <summary>
        /// Optional.
        /// Azure table connection string resolver delegate for Azure table logs.
        /// Call <see cref="DisableLogging"/> if wan't to specify this option.
        /// </summary>
        [CanBeNull]
        public Func<TAppSettings, string> LogsAzureTableConnectionStringResolver { get; set; }

        /// <summary>
        /// Optional.
        /// Provides option to process settings values.
        /// </summary>
        public Action<IReloadingManager<TAppSettings>> UseSettings { get; set; }

        /// <summary>
        /// Optional.
        /// Provides access to the extended options of Lykke.Sdk.
        /// </summary>
        public Action<LykkeServiceOptions<TAppSettings>> Extended { get; set; }
        
        internal List<TypeInfo> ControllerTypes { get; }

        internal bool HaveToDisableLogging { get; private set; }
        
        public BlockchainIntegrationServiceOptions()
        {
            ControllerTypes = new List<TypeInfo>();
        }

        /// <summary>
        /// Adds HTTP API controller to the service.
        /// </summary>
        /// <typeparam name="TController"></typeparam>
        public void AddController<TController>()
        {
            ControllerTypes.Add(typeof(TController).GetTypeInfo());
        }

        /// <summary>
        /// Adds default isAlive HTTP API controller to the service.
        /// </summary>
        public void AddDefaultIsAliveController()
        {
            AddController<IsAliveController>();
        }

        /// <summary>
        /// Disables logging.
        /// </summary>
        public void DisableLogging()
        {
            HaveToDisableLogging = true;
        }
    }
}
