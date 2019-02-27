using Lykke.Bil2.Client.TransactionExecutor.Tests.Configuration;
using Lykke.Bil2.Client.TransactionsExecutor;
using Lykke.Bil2.Contract.Common;
using Lykke.Bil2.Contract.TransactionsExecutor.Responses;
using Lykke.Bil2.Sdk.TransactionsExecutor;
using Lykke.Bil2.Sdk.TransactionsExecutor.Models;
using Lykke.Bil2.Sdk.TransactionsExecutor.Services;
using Lykke.Bil2.Sdk.TransactionsExecutor.Settings;
using Lykke.Sdk.Settings;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Bil2.Contract.TransactionsExecutor;
using Lykke.Bil2.Contract.TransactionsExecutor.Requests;

namespace Lykke.Bil2.Client.TransactionExecutor.Tests.Tests
{
    [TestFixture]
    public class TransactionExecutorClientTests : TransactionsExecutorClientBase
    {
        private static readonly Base58String MyPublicKey = new Base58String("Vj75CuZgqYqhewfDfF9KQEdejjqbnoDiRjuXUnwCo2jkLo8AJpqBF6jFovufKrvwqUaubTRrAwr3wBBHtFVWhxhrxwFMoeB3mrBXnreVkfRdL1L9NUpyn4qDTB1Hwm3kBjmnhdVm2ZxmZ696FKj6yeBMnPB7Lkoa4XxKg6a9TwPCfgUbNV9b8dCPXm1YdYMdFK9Hf8sestxpp6FphTxbrjnicpeiU");
        private static readonly Base58String MyPrivateKey = new Base58String("hLLPYvqqGdBtPBMGWp98NbAHtDHuoj4vnrQqNPuMBMZpRFCX2kVtbC9HgrMQthM6NzEDjQw6c88PfiRCqzegb2KpTnBN1hqLyQKfT8FuGVDGRBJEYFZbgUbbb5e5PZZVnRwabyy4Rup8c5Yb5fWJ1Lw73yUtcxvmx52ymwm8m9j3NtVRTkcb5NsXGbMdanUpa9MdAMHReFDDh1qWx8N6zTb4st4WbaTuTamhcTEueuPKosaum5TLv2udcUDj2JtaZYUhDJ6EvotsHScwLQnwcW4ZkEydSCJgqefYp4n7qgNpzNKU2ffsDmPuQDfVBLNiFkqv99TMYsvrYbemcLLRoEUiqvGTaGZBpHErd5quYq1G9187nbySGy3aJCMVqVWzApuYKewUQDRND1fx6kybQpuDy6CjBp4i5sdAub4eFNHjRwVVUUsYQjjhy8PQpES5AWszv2DgMyrqEaAGYxr3akK6HmbYyBquWriAdYjp9v8NW29yhpSBXwyGpCT5j54KA2F5yaxv6MfUiucGHxYdgNFdkhSZYRB3FgZJQzTAy6utSZTapjcBmu25nzQcdaevnSW4rMd9kuhMGJL6PtyG6niHn48bsmwxSukGZsMvfRb8UDf3ZnGnNFHXChADzywM7ghZBtohBRgbnAyduUQ7GbLv6vhuyXV7qxAkvAwuz4mipe8MbTem6cjhKJrV1Sa4Ya6pUnWR7BhqNjWEKuVbCt8D9Et2no6u8hXxyxV6kD5tKEJ932PVHYBmnLQV9jCfS2RRv2BhBjhA6bUp8zCSuW2CyVYPCgmqQCa4EahA6iiHhymdaPQ8YGChbH8san");

        private static readonly Base58String MyPublicKey2 = new Base58String("HHhSTuePBZCYtqDNEPCSy3UJnH77zm4u9sSDX3EZXuCFuHAjFYxbAHJvK1VEyRRnBqSog71ek64MtzC9WvQoojzS1qRSFwEBFcvjx5JDecTQo8MDXUW4LufoxG9teN572QtmQPeahYThUDvTxhVHRxTNN5Em7gvd5SYMLgThREzTXV1Sn7ZUiJ8Fs3j7DbpwDhUaJEpcqdzB4Hd8YmoncUqAFYM6Hnpf7ZEfUaHfBf6L4bvjjQa4fx7KCNH1W6aP622UWemzHu1E1r6poDzqiyVZcKN1nbfHHHz75F7");
        private static readonly Base58String MyPrivateKey2 = new Base58String("DNfv4gHkDhJHoTo2F653XjAScBXXw5qyvyqKK41AkcfVLpcgURa8KLoF9FSxUfvt7jxUUuv2yFhV1ede8LANCzdFMSPLV6pNBm5EpU4tY6sTKzhJeLQCw7KBhA1us58yZRXFAeakozo8fjA32v94wefe6eSzmiZv6tukTzie1vdKRNakuxbGodzmMmQocpC3wyp26aheCLHkbny5vCGwpUHAmWPMc4LpMkyCoE5cVdvgSdc8a9bpJ4RgHMEtKfBkqY1pjCzrrdhrtcKve9CMuuS2tftRZGMPpZZwqj25th9b9P1K3jRbzt8Z3fvjDYNYGuq2165yRpgNzihs9XAYwL9DdXWtFS1K8pAKJoVBqMZfdXqvcVtAAMyTH7JxMtDmUFZgJZfg3SGGQrvMYZCRTJq7CjxsynsPagVzJoEhEqGzdbXwbX91b9uNaAP1zzeoXpnXZWHpYXmKqb3yUAKe7ymf4ibnM3JA7UYVNW9bkUbNmcMfMbi9rtCvTb5c9D2tss56JYsbuYnXoPBnwiwSZzhJuccBDa5KPVqzApRf4ZsmgjqTxEYWWGZV8jWzp1qsdR7kuxCQKjQ8ubS24uWMi28Ppxho6NSuppAQzPiFk4owfAzqnzbtDA5W9oCQoBXyxa7Cv1w9m9GPn4dqMVGchmbjLnTHiekkmaiXSG7HtjZUPEBJxsguiRPFEVCMywkcau7hnoFv9acn2fL3pxdYwvE6Q5ZxPnQfZKFaZurF2427L3QoBuoJsVa1oesysYb8tWT9r21gRpddT2yMnPFQeDhje9yQZxQqpGYLr6jAmATtovZ4p9ncRS5fFH3iYr49fT93pSYHdD4hXe9bkY34QZnTMMYrurHSr9L91iAaQm7WBFEUHymBgddMfUcBakfJiifgHf8GBf2cC9PH146HJ5AwkooTUNZzrZUP2Pu8S3WgJk7byRajDty8H1qANSaXRqbXCfNmnLsvXKPA47GepfywQBhcFtQns8mzrBuvvSTry166n2porcEodMkhAb5pvN11BET2ebMSLUKWKZes1Qj4ezoZmudA9pLCoNSkrUWk4GTfn9A37TLSjxQuiw6uBUMB2uepVJ81Dy4VCNsqW");

        private static readonly string _pathToSettings = "appsettings.tests.json";

        [OneTimeSetUp]
        public void GlobalSetup()
        {
            PrepareSettings();
        }

        [OneTimeTearDown]
        public void GlobalTeardown()
        {
        }

        [Test]
        public async Task Get_is_alive()
        {
            //ARRANGE
            string disease = "Disease";

            var client = PrepareClient<AppSettings>((options) =>
            {
                Mock<IAddressValidator> addressValidator = new Mock<IAddressValidator>();
                Mock<IHealthProvider> healthProvider = new Mock<IHealthProvider>();
                Mock<IIntegrationInfoService> integrationInfoService = new Mock<IIntegrationInfoService>();
                Mock<ITransactionEstimator> transactionEstimator = new Mock<ITransactionEstimator>();
                Mock<ITransactionExecutor> transactionExecutor = new Mock<ITransactionExecutor>();

                options.IntegrationName = $"{nameof(TransactionExecutorClientTests)}+{nameof(Get_is_alive)}";
                healthProvider.Setup(x => x.GetDiseaseAsync()).ReturnsAsync(disease);

                options.AddressValidatorFactory = c => addressValidator.Object;
                options.HealthProviderFactory = c => healthProvider.Object;
                options.IntegrationInfoServiceFactory = c => integrationInfoService.Object;
                options.TransactionEstimatorFactory = c => transactionEstimator.Object;
                options.TransactionExecutorFactory = c => transactionExecutor.Object;
                options.DisableLogging = true;
            });

            //ACT
            var result = await client.GetIsAliveAsync();

            //ASSERT
            Assert.True(result != null);
            Assert.True(result.Disease == disease);
        }

        [Test]
        public async Task Get_integration_info()
        {
            //ARRANGE
            var blockchainInfo = new BlockchainInfo(123, DateTime.UtcNow);
            var dependencies = new Dictionary<string, DependencyInfo>()
            {
                {"dependency", new DependencyInfo(new Version(1, 0, 0),
                    new Version(1, 0, 0))},
            };

            string disease = "Disease";

            var client = PrepareClient<AppSettings>((options) =>
            {
                Mock<IAddressValidator> addressValidator = new Mock<IAddressValidator>();
                Mock<IHealthProvider> healthProvider = new Mock<IHealthProvider>();
                Mock<IIntegrationInfoService> integrationInfoService = new Mock<IIntegrationInfoService>();
                Mock<ITransactionEstimator> transactionEstimator = new Mock<ITransactionEstimator>();
                Mock<ITransactionExecutor> transactionExecutor = new Mock<ITransactionExecutor>();

                options.IntegrationName = $"{nameof(TransactionExecutorClientTests)}+{nameof(Get_integration_info)}";
                integrationInfoService.Setup(x => x.GetInfoAsync())
                    .ReturnsAsync(new IntegrationInfo(blockchainInfo, dependencies));
                healthProvider.Setup(x => x.GetDiseaseAsync()).ReturnsAsync(disease);

                options.AddressValidatorFactory = c => addressValidator.Object;
                options.HealthProviderFactory = c => healthProvider.Object;
                options.IntegrationInfoServiceFactory = c => integrationInfoService.Object;
                options.TransactionEstimatorFactory = c => transactionEstimator.Object;
                options.TransactionExecutorFactory = c => transactionExecutor.Object;
                options.DisableLogging = true;
            });

            //ACT
            var result = await client.GetIntegrationInfoAsync();

            //ASSERT
            var dependency = dependencies.First();
            Assert.True(result != null);
            Assert.True(result.Blockchain.LatestBlockMoment == blockchainInfo.LatestBlockMoment);
            Assert.True(result.Blockchain.LatestBlockNumber == blockchainInfo.LatestBlockNumber);
            Assert.True(result.Dependencies[dependency.Key].LatestAvailableVersion == dependency.Value.LatestAvailableVersion);
            Assert.True(result.Dependencies[dependency.Key].RunningVersion == dependency.Value.RunningVersion);
        }

        [Test]
        [TestCase("0x1", AddressTagType.Number, "1", AddressValidationResult.Valid)]
        [TestCase("0x1", AddressTagType.Number, "x", AddressValidationResult.InvalidTagFormat)]
        [TestCase("0x1", AddressTagType.Text, "x", AddressValidationResult.Valid)]
        [TestCase("0x1", AddressTagType.Text, "1", AddressValidationResult.Valid)]
        [TestCase("zx", AddressTagType.Text, "x", AddressValidationResult.InvalidAddressFormat)]
        [TestCase("zx", null, null, AddressValidationResult.RequiredTagMissed)]
        public async Task Get_address_validity(string address, AddressTagType? tagType, string tag, AddressValidationResult validationResult)
        {
            //ARRANGE
            string disease = "Disease";

            var client = PrepareClient<AppSettings>((options) =>
            {
                Mock<IAddressValidator> addressValidator = new Mock<IAddressValidator>();
                Mock<IHealthProvider> healthProvider = new Mock<IHealthProvider>();
                Mock<IIntegrationInfoService> integrationInfoService = new Mock<IIntegrationInfoService>();
                Mock<ITransactionEstimator> transactionEstimator = new Mock<ITransactionEstimator>();
                Mock<ITransactionExecutor> transactionExecutor = new Mock<ITransactionExecutor>();

                options.IntegrationName = $"{nameof(TransactionExecutorClientTests)}+{nameof(Get_address_validity)}";
                healthProvider.Setup(x => x.GetDiseaseAsync()).ReturnsAsync(disease);
                addressValidator.Setup(x => x.ValidateAsync(address, tagType, tag))
                    .ReturnsAsync(new AddressValidityResponse(validationResult));

                options.AddressValidatorFactory = c => addressValidator.Object;
                options.HealthProviderFactory = c => healthProvider.Object;
                options.IntegrationInfoServiceFactory = c => integrationInfoService.Object;
                options.TransactionEstimatorFactory = c => transactionEstimator.Object;
                options.TransactionExecutorFactory = c => transactionExecutor.Object;
                options.DisableLogging = true;
            });

            //ACT
            var result = await client.GetAddressValidityAsync(address, tagType, tag);

            //ASSERT
           
            Assert.True(result != null);
            Assert.True(result.Result == validationResult);
        }

        [Test]
        public async Task Build_sending_transaction()
        {
            //ARRANGE
            string disease = "Disease";
            string transactionResponse = "transactionResponse";

            var client = PrepareClient<AppSettings>((options) =>
            {
                Mock<IAddressValidator> addressValidator = new Mock<IAddressValidator>();
                Mock<IHealthProvider> healthProvider = new Mock<IHealthProvider>();
                Mock<IIntegrationInfoService> integrationInfoService = new Mock<IIntegrationInfoService>();
                Mock<ITransactionEstimator> transactionEstimator = new Mock<ITransactionEstimator>();
                Mock<ITransactionExecutor> transactionExecutor = new Mock<ITransactionExecutor>();

                options.IntegrationName = $"{nameof(TransactionExecutorClientTests)}+{nameof(Get_address_validity)}";
                healthProvider.Setup(x => x.GetDiseaseAsync()).ReturnsAsync(disease);
                transactionExecutor.Setup(x => x.BuildSendingAsync(It.IsAny<BuildSendingTransactionRequest>()))
                    .ReturnsAsync(new BuildSendingTransactionResponse(Base58String.Encode(transactionResponse)));

                options.AddressValidatorFactory = c => addressValidator.Object;
                options.HealthProviderFactory = c => healthProvider.Object;
                options.IntegrationInfoServiceFactory = c => integrationInfoService.Object;
                options.TransactionEstimatorFactory = c => transactionEstimator.Object;
                options.TransactionExecutorFactory = c => transactionExecutor.Object;
                options.DisableLogging = true;
            });

            //ACT
            var transfers = new Transfer[]
            {
                new Transfer(
                    new AssetId("asset"),
                    CoinsAmount.FromDecimal(1000000000, 4),
                    new Address("x1"),
                    new Address("x2")),
            };
            var request = new BuildSendingTransactionRequest(transfers, new FeeOptions(FeeType.DeductFromAmount));
            var result = await client.BuildSendingTransactionAsync(request);

            //ASSERT

            Assert.True(result != null);
            Assert.True(result.TransactionContext.DecodeToString() == transactionResponse);
        }

        private void PrepareSettings()
        {
            Environment.SetEnvironmentVariable("DisableAutoRegistrationInMonitoring", "true");
            Environment.SetEnvironmentVariable("SettingsUrl", _pathToSettings);

            var prepareSettings = new AppSettings()
            {
                Db = new DbSettings()
                {
                    AzureDataConnString = "empty",
                    LogsConnString = "empty"
                },
                HealthMonitoringPeriod = TimeSpan.FromSeconds(10),
                NodeUrl = "http://localhost:7777/api",
                NodeUser = "user",
                NodePassword = "password",
                MonitoringServiceClient = new MonitoringServiceClientSettings()
                { MonitoringServiceUrl = "http://localhost:5431" },
                //SlackNotifications = new SlackNotificationsSettings()
                //{
                //    AzureQueue = "empty"
                //}
            };

            string serializedSettings = Newtonsoft.Json.JsonConvert.SerializeObject(prepareSettings);

            try
            {
                File.Delete(_pathToSettings);
            }
            catch
            {
            }

            File.AppendAllText(_pathToSettings, serializedSettings);
        }

        private ITransactionsExecutorApi PrepareClient<TAppSettings>(Action<TransactionsExecutorServiceOptions<TAppSettings>> config)
            where TAppSettings : BaseTransactionsExecutorSettings<DbSettings>
        {
            StartupDependencyFactorySingleton.Instance = new StartupDependencyFactory<TAppSettings>(config);
            var client = base.CreateClientApi<StartupTemplate>("http://localhost:5000");

            return client;
        }
    }
}
