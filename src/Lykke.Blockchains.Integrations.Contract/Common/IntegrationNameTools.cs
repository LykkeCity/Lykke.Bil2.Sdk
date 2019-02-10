using System.Linq;
using JetBrains.Annotations;

namespace Lykke.Blockchains.Integrations.Contract.Common
{
    /// <summary>
    /// Tools to process blockchain integration name
    /// </summary>
    [PublicAPI]
    public static class IntegrationNameTools
    {
        /// <summary>
        /// Converts integration name to the CamelCaseNotation
        /// </summary>
        /// <param name="integrationName">
        /// Name of the integration. Words should be separated by the space, each word should be started from the capital case.
        /// </param>
        public static string ToCamelCase(string integrationName)
        {
            var words = integrationName.Split(' ');

            return string.Join(string.Empty, words);
        }

        /// <summary>
        /// Converts integration name to the kebab-notation
        /// </summary>
        /// <param name="integrationName">
        /// Name of the integration. Words should be separated by the space, each word should be started from the capital case.
        /// </param>
        public static string ToKebab(string integrationName)
        {
            var words = integrationName.Split(' ');

            return string.Join('-', words.Select(w => w.ToLowerInvariant()));
        }
    }
}
