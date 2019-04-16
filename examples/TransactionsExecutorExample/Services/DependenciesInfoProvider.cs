using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Bil2.Contract.TransactionsExecutor.Responses;
using Lykke.Bil2.Sdk.TransactionsExecutor.Services;
using Lykke.Bil2.SharedDomain;

namespace TransactionsExecutorExample.Services
{
    public class DependenciesInfoProvider : IDependenciesInfoProvider
    {
        public Task<IReadOnlyDictionary<DependencyName, DependencyInfo>> GetInfoAsync()
        {
            Console.WriteLine("Getting dependencies info");

            IReadOnlyDictionary<DependencyName, DependencyInfo> result = new Dictionary<DependencyName, DependencyInfo>
            {
                {
                    "node", 
                    new DependencyInfo(new Semver("1.2.3"), new Semver("1.4.2"))
                }
            };

            return Task.FromResult(result);
        }
    }
}
