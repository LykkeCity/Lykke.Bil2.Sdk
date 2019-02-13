using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Lykke.Blockchains.Integrations.Contract.Common
{
    /// <summary>
    /// Tools to process blockchain integration name
    /// </summary>
    [PublicAPI]
    public static class StringTools
    {
        /// <summary>
        /// Converts the CamelCase notation to the kebab-notation
        /// </summary>
        public static string CamelToKebab(string integrationName)
        {
            var words = GetCamelCaseWords(integrationName);

            return string.Join('-', words.Select(w => w.ToLowerInvariant()));
        }

        private static IEnumerable<string> GetCamelCaseWords(string integrationName)
        {
            if (!string.IsNullOrWhiteSpace(integrationName))
            {
                var wordStart = 0;
                
                for (var i = 1; i < integrationName.Length; i++)
                {
                    if (char.IsUpper(integrationName[i]))
                    {
                        yield return new string(integrationName.Substring(wordStart, i - wordStart));
                        wordStart = i;
                    }
                }

                yield return new string(integrationName.Substring(wordStart));
            }
        }
    }
}
