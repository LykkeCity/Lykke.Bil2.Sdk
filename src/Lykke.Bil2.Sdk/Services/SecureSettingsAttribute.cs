using System;

namespace Lykke.Bil2.Sdk.Services
{
    /// <summary>
    /// Marks property of the app settings as secure. Secure properties are not rendered.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class SecureSettingsAttribute : Attribute
    {
    }
}
