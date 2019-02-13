using System;

namespace Lykke.Blockchains.Integrations.Sdk.Services
{
    /// <summary>
    /// Marks property of the app settings as secure. Secure properties are not rendered.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class SecureSettingsAttribute : Attribute
    {
    }
}
