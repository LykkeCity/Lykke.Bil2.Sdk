using Lykke.Bil2.Sdk.TransactionsExecutor.Services;
using Moq;

namespace Lykke.Bil2.Client.TransactionExecutor.Tests.Tests
{
    public class MockAggregator
    {
        public Mock<IAddressValidator> AddressValidator { get; set; }

        public Mock<IHealthProvider> HealthProvider { get; set; }

        public Mock<IIntegrationInfoService> IntegrationInfoService { get; set; }

        public Mock<ITransferAmountTransactionsEstimator> TransactionEstimator { get; set; }

        public Mock<ITransactionBroadcaster> TransactionBroadcaster { get; set; }

        public Mock<ITransferAmountTransactionsBuilder> TransferAmountTransactionBuilder { get; set; }

        public Mock<IAddressFormatsProvider> AddressFormatsProvider { get; set; }

        public Mock<ITransactionsStateProvider> TransactionStateProvider { get; set; }

        public Mock<ITransferCoinsTransactionsBuilder> TransferCoinsTransactionsBuilder { get; set; } = null;

        public Mock<ITransferCoinsTransactionsEstimator> TransferCoinsTransactionsEstimator { get; set; } = null;
    }
}