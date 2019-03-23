using JetBrains.Annotations;
using Lykke.Bil2.Contract.Common.Extensions;

namespace Lykke.Bil2.Sdk.Repositories
{
    /// <summary>
    /// Tools for the raw objects repository
    /// </summary>
    [PublicAPI]
    public static class RawObjectRepositoryTools
    {
        public static string GetContainerName(string integrationName, RawObjectType objectType)
        {
            return $"{integrationName}-raw-{objectType.ToString().CamelToKebab()}";
        }

        public static string GetBlobName(string objectId)
        {
            return objectId;
        }
    }
}
