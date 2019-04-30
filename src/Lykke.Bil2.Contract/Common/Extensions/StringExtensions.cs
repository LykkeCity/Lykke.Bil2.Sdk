using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Lykke.Bil2.SharedDomain;

namespace Lykke.Bil2.Contract.Common.Extensions
{
    [PublicAPI]
    public static class StringExtensions
    {
        /// <summary>
        /// Encrypts the string with RSA/AES
        /// </summary>
        public static EncryptedString Encrypt(this string value, Base64String publicKey)
        {
            return EncryptedString.Encrypt(publicKey, value);
        }

        /// <summary>
        /// Converts the CamelCase notation to the kebab-notation
        /// </summary>
        public static string CamelToKebab(this string integrationName)
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
