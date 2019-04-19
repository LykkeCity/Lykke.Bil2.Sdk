using System.Threading;
using System.Threading.Tasks;

namespace Lykke.Bil2.RabbitMq.Subscription.Core
{
    public interface IBackgroundService
    {
        void Start();

        Task StopAsync();
        
        Task StopAsync(
            CancellationToken cancellationToken);
    }
}
