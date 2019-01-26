using System.Collections.Generic;
using Lykke.Blockchains.Integrations.Sdk.Services;

namespace Lykke.Blockchains.Integrations.Sdk
{
    /// <summary>
    /// Renders settings to the flat dictionary of key/values. Takes into accounts secure values such as connections trings
    /// and properties marked with <see cref="SecureSettingsAttribute"/>
    /// </summary>
    public interface ISettingsRenderer
    {
        /// <summary>
        /// Renders current value of the settings to the flat dictionary of key/values.
        /// Takes into accounts secure values such as connections trings and properties marked with <see cref="SecureSettingsAttribute"/>
        /// </summary>
        IDictionary<string, string> RenderSettings();
    }
}
