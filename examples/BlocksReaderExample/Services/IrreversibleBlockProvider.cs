using System;
using System.Threading.Tasks;
using Lykke.Bil2.Contract.BlocksReader.Events;
using Lykke.Bil2.Sdk.BlocksReader.Services;

namespace BlocksReaderExample.Services
{
    public class IrreversibleBlockProvider : IIrreversibleBlockProvider
    {
        public Task<LastIrreversibleBlockUpdatedEvent> GetLastAsync()
        {
            return Task.FromResult(new LastIrreversibleBlockUpdatedEvent(1000, Guid.NewGuid().ToString("N")));
        }
    }
}
