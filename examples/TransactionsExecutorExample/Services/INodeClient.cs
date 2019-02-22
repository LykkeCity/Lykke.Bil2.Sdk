using System.Threading.Tasks;

namespace TransactionsExecutorExample.Services
{
    public interface INodeClient
    {
        Task<bool> GetIsSynchronizedAsync();
    }
}
