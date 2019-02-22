using System.Collections.Generic;

namespace Lykke.Bil2.Sdk.Services
{
    /// <summary>
    /// Renders settings to the flat dictionary of key/values. Takes into account secure values such as connections strings
    /// and properties marked with <see cref="SecureSettingsAttribute"/>
    /// </summary>
    public interface ISettingsRenderer
    {
        /// <summary>
        /// Renders current value of the settings to the flat dictionary of key/values.
        /// Takes into account secure values such as connections strings and properties marked with <see cref="SecureSettingsAttribute"/>
        /// </summary>
        IDictionary<string, string> RenderSettings();
    }
}
