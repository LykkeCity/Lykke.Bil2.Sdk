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
using Lykke.Bil2.Client.TransactionsExecutor.Exceptions;
using Lykke.Bil2.Contract.Common.Exceptions;
using Lykke.Bil2.Contract.TransactionsExecutor;
using Lykke.Bil2.Contract.TransactionsExecutor.Requests;
using Lykke.Bil2.Sdk.TransactionsExecutor.Exceptions;
using Lykke.Bil2.Sdk.TransactionsExecutor.Repositories;
using Lykke.Bil2.WebClient.Exceptions;

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
                 CreateMocks(
                    out var addressValidator, 
                    out var healthProvider, 
                    out var integrationInfoService, 
                    out var transactionEstimator, 
                    out var transactionBroadcaster,
                    out var transferAmountTransactionBuilder,
                    out var addressFormatsProvider);

                options.IntegrationName = $"{nameof(TransactionExecutorClientTests)}+{nameof(Get_is_alive)}";
                healthProvider.Setup(x => x.GetDiseaseAsync()).ReturnsAsync(disease);

                options.AddressValidatorFactory = c => addressValidator.Object;
                options.HealthProviderFactory = c => healthProvider.Object;
                options.IntegrationInfoServiceFactory = c => integrationInfoService.Object;
                options.TransferAmountTransactionsEstimatorFactory = c => transactionEstimator.Object;
                options.TransactionBroadcasterFactory = c => transactionBroadcaster.Object;
                options.TransferAmountTransactionsBuilderFactory = c => transferAmountTransactionBuilder.Object;
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
                CreateMocks(
                    out var addressValidator,
                    out var healthProvider,
                    out var integrationInfoService,
                    out var transactionEstimator,
                    out var transactionBroadcaster,
                    out var transferAmountTransactionBuilder,
                    out var addressFormatsProvider);

                options.IntegrationName = $"{nameof(TransactionExecutorClientTests)}+{nameof(Get_integration_info)}";
                integrationInfoService.Setup(x => x.GetInfoAsync())
                    .ReturnsAsync(new IntegrationInfo(blockchainInfo, dependencies));
                healthProvider.Setup(x => x.GetDiseaseAsync()).ReturnsAsync(disease);

                ConfigureFactories(options,
                    addressValidator, 
                    healthProvider,
                    integrationInfoService,
                    transactionEstimator,
                    transactionBroadcaster,
                    transferAmountTransactionBuilder,
                    addressFormatsProvider);
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
                CreateMocks(
                    out var addressValidator,
                    out var healthProvider,
                    out var integrationInfoService,
                    out var transactionEstimator,
                    out var transactionBroadcaster,
                    out var transferAmountTransactionBuilder,
                    out var addressFormatsProvider);

                options.IntegrationName = $"{nameof(TransactionExecutorClientTests)}+{nameof(Get_address_validity)}";
                healthProvider.Setup(x => x.GetDiseaseAsync()).ReturnsAsync(disease);
                addressValidator.Setup(x => x.ValidateAsync(address, tagType, tag))
                    .ReturnsAsync(new AddressValidityResponse(validationResult));

                ConfigureFactories(options,
                    addressValidator,
                    healthProvider,
                    integrationInfoService,
                    transactionEstimator,
                    transactionBroadcaster,
                    transferAmountTransactionBuilder,
                    addressFormatsProvider);
            });

            //ACT
            var result = await client.GetAddressValidityAsync(address, tagType, tag);

            //ASSERT
           
            Assert.True(result != null);
            Assert.True(result.Result == validationResult);
        }

        [Test]
        public async Task Build_transfer_amount_transaction()
        {
            //ARRANGE
            string disease = "Disease";
            string transactionResponse = "transactionResponse";

            var client = PrepareClient<AppSettings>((options) =>
            {
                CreateMocks(
                    out var addressValidator,
                    out var healthProvider,
                    out var integrationInfoService,
                    out var transactionEstimator,
                    out var transactionBroadcaster,
                    out var transferAmountTransactionBuilder,
                    out var addressFormatsProvider);

                options.IntegrationName = $"{nameof(TransactionExecutorClientTests)}+{nameof(Build_transfer_amount_transaction)}";
                healthProvider.Setup(x => x.GetDiseaseAsync()).ReturnsAsync(disease);
                transferAmountTransactionBuilder.Setup(x => x.BuildTransferAmountAsync(It.IsAny<BuildTransferAmountTransactionRequest>()))
                    .ReturnsAsync(new BuildTransactionResponse(Base58String.Encode(transactionResponse)));

                ConfigureFactories(options,
                    addressValidator,
                    healthProvider,
                    integrationInfoService,
                    transactionEstimator,
                    transactionBroadcaster,
                    transferAmountTransactionBuilder,
                    addressFormatsProvider);
            });

            //ACT
            var transfers = new[]
            {
                new Transfer(
                    new AssetId("asset"),
                    CoinsAmount.FromDecimal(1000000000, 4),
                    new Address("x1"),
                    new Address("x2")),
            };
            var request = new BuildTransferAmountTransactionRequest(transfers, new Dictionary<AssetId, CoinsAmount>());
            var result = await client.BuildTransferAmountTransactionAsync(request);

            //ASSERT

            Assert.True(result != null);
            Assert.True(result.TransactionContext.DecodeToString() == transactionResponse);
        }

        [Test]
        public void Bad_request_while_building_transfer_amount_transaction()
        {
            //ARRANGE
            string disease = "Disease";

            var client = PrepareClient<AppSettings>((options) =>
            {
                CreateMocks(
                    out var addressValidator,
                    out var healthProvider,
                    out var integrationInfoService,
                    out var transactionEstimator,
                    out var transactionBroadcaster,
                    out var transferAmountTransactionBuilder,
                    out var addressFormatsProvider);

                options.IntegrationName = $"{nameof(TransactionExecutorClientTests)}+{nameof(Bad_request_while_building_transfer_amount_transaction)}";
                healthProvider.Setup(x => x.GetDiseaseAsync()).ReturnsAsync(disease);
                transferAmountTransactionBuilder.Setup(x => x.BuildTransferAmountAsync(It.IsAny<BuildTransferAmountTransactionRequest>()))
                    .ThrowsAsync(new RequestValidationException("NOT VALID"));

                ConfigureFactories(options,
                    addressValidator,
                    healthProvider,
                    integrationInfoService,
                    transactionEstimator,
                    transactionBroadcaster,
                    transferAmountTransactionBuilder,
                    addressFormatsProvider);
            });

            //ACT && ASSERT

            Assert.ThrowsAsync<TransactionBuildingWebApiException>(async () =>
            {
                var transfers = new[]
                {
                    new Transfer(
                        new AssetId("asset"),
                        CoinsAmount.FromDecimal(1000000000, 4),
                        new Address("x1"),
                        new Address("x2")),
                };
                var request = new BuildTransferAmountTransactionRequest(transfers, new Dictionary<AssetId, CoinsAmount>());
                await client.BuildTransferAmountTransactionAsync(request);
            });
        }

        [Test]
        public void Node_issues_while_building_transfer_amount_transaction()
        {
            //ARRANGE
            string disease = "Disease";

            var client = PrepareClient<AppSettings>((options) =>
            {
                CreateMocks(
                    out var addressValidator,
                    out var healthProvider,
                    out var integrationInfoService,
                    out var transactionEstimator,
                    out var transactionBroadcaster,
                    out var transferAmountTransactionBuilder,
                    out var addressFormatsProvider);

                options.IntegrationName = $"{nameof(TransactionExecutorClientTests)}+{nameof(Node_issues_while_building_transfer_amount_transaction)}";
                healthProvider.Setup(x => x.GetDiseaseAsync()).ReturnsAsync(disease);
                transferAmountTransactionBuilder.Setup(x => x.BuildTransferAmountAsync(It.IsAny<BuildTransferAmountTransactionRequest>()))
                    .ThrowsAsync(
                        new TransactionBuildingException(
                            TransactionBuildingError.RetryLater,
                            "Node is too busy"));

                ConfigureFactories(options,
                    addressValidator,
                    healthProvider,
                    integrationInfoService,
                    transactionEstimator,
                    transactionBroadcaster,
                    transferAmountTransactionBuilder,
                    addressFormatsProvider);
            });

            //ACT && ASSERT

            Assert.ThrowsAsync<TransactionBuildingWebApiException>(async () =>
            {
                var transfers = new[]
                {
                    new Transfer(
                        new AssetId("asset"),
                        CoinsAmount.FromDecimal(1000000000, 4),
                        new Address("x1"),
                        new Address("x2")),
                };
                var request = new BuildTransferAmountTransactionRequest(transfers, new Dictionary<AssetId, CoinsAmount>());
                await client.BuildTransferAmountTransactionAsync(request);
            });
        }

        [Test]
        public async Task Estimate_transfer_amount_transaction()
        {
            //ARRANGE
            string disease = "Disease";
            var dict = new Dictionary<AssetId, CoinsAmount>()
            {
                { new AssetId("asset"), CoinsAmount.FromDecimal(1000, 4) }
            };


            var client = PrepareClient<AppSettings>((options) =>
            {
                CreateMocks(
                    out var addressValidator,
                    out var healthProvider,
                    out var integrationInfoService,
                    out var transactionEstimator,
                    out var transactionBroadcaster,
                    out var transferAmountTransactionBuilder,
                    out var addressFormatsProvider);

                options.IntegrationName = $"{nameof(TransactionExecutorClientTests)}+{nameof(Estimate_transfer_amount_transaction)}";
                healthProvider.Setup(x => x.GetDiseaseAsync()).ReturnsAsync(disease);
                transactionEstimator.Setup(x => x.EstimateTransferAmountAsync(It.IsAny<EstimateTransferAmountTransactionRequest>()))
                    .ReturnsAsync(new EstimateTransactionResponse(dict));

                ConfigureFactories(options,
                    addressValidator,
                    healthProvider,
                    integrationInfoService,
                    transactionEstimator,
                    transactionBroadcaster,
                    transferAmountTransactionBuilder,
                    addressFormatsProvider);
            });

            //ACT
            var transfers = new[]
            {
                new Transfer(
                    new AssetId("asset"),
                    CoinsAmount.FromDecimal(1000000000, 4),
                    new Address("x1"),
                    new Address("x2")),
            };
            var request = new EstimateTransferAmountTransactionRequest(transfers);
            var result = await client.EstimateTransferAmountTransactionAsync(request);

            //ASSERT
            var estimation = result.EstimatedFee.First();
            Assert.True(result != null);
            Assert.True(estimation.Key == "asset");
            Assert.True(estimation.Value.ToDecimal() == CoinsAmount.FromDecimal(1000, 4).ToDecimal());
        }

        [Test]
        public void Bad_request_while_estimating_transfer_amount_transaction()
        {
            //ARRANGE
            string disease = "Disease";

            var client = PrepareClient<AppSettings>((options) =>
            {
                CreateMocks(
                    out var addressValidator,
                    out var healthProvider,
                    out var integrationInfoService,
                    out var transactionEstimator,
                    out var transactionBroadcaster,
                    out var transferAmountTransactionBuilder,
                    out var addressFormatsProvider);

                options.IntegrationName = $"{nameof(TransactionExecutorClientTests)}+{nameof(Bad_request_while_estimating_transfer_amount_transaction)}";
                healthProvider.Setup(x => x.GetDiseaseAsync()).ReturnsAsync(disease);
                transactionEstimator.Setup(x => x.EstimateTransferAmountAsync(It.IsAny<EstimateTransferAmountTransactionRequest>()))
                    .ThrowsAsync(new RequestValidationException("Not VALID"));

                ConfigureFactories(options,
                    addressValidator,
                    healthProvider,
                    integrationInfoService,
                    transactionEstimator,
                    transactionBroadcaster,
                    transferAmountTransactionBuilder,
                    addressFormatsProvider);
            });

            //ACT && ASSERT
            Assert.ThrowsAsync<BadRequestWebApiException>(async () =>
            {
                var transfers = new[]
                {
                    new Transfer(
                        new AssetId("asset"),
                        CoinsAmount.FromDecimal(1000000000, 4),
                        new Address("x1"),
                        new Address("x2")),
                };
                var request = new EstimateTransferAmountTransactionRequest(transfers);
                await client.EstimateTransferAmountTransactionAsync(request);
            });
        }

        [Test]
        public async Task Broadcast_transaction()
        {
            //ARRANGE
            string disease = "Disease";
            string signedTransaction = "signedTransaction";

            var client = PrepareClient<AppSettings>((options) =>
            {
                CreateMocks(
                    out var addressValidator,
                    out var healthProvider,
                    out var integrationInfoService,
                    out var transactionEstimator,
                    out var transactionBroadcaster,
                    out var transferAmountTransactionBuilder,
                    out var addressFormatsProvider);

                options.IntegrationName = $"{nameof(TransactionExecutorClientTests)}+{nameof(Broadcast_transaction)}";
                healthProvider.Setup(x => x.GetDiseaseAsync()).ReturnsAsync(disease);
                transactionBroadcaster.Setup(x => x.BroadcastAsync(It.IsAny<BroadcastTransactionRequest>()))
                    .Returns(Task.CompletedTask);

                ConfigureFactories(options,
                    addressValidator,
                    healthProvider,
                    integrationInfoService,
                    transactionEstimator,
                    transactionBroadcaster,
                    transferAmountTransactionBuilder,
                    addressFormatsProvider);
            });

            //ACT && ASSERT
            var request = new BroadcastTransactionRequest(Base58String.Encode(signedTransaction));
            await client.BroadcastTransactionAsync(request);

            //Assume everything is ok if no exceptions here
        }

        [Test]
        public void Bad_request_broadcast_transaction()
        {
            //ARRANGE
            string disease = "Disease";
            string signedTransaction = "signedTransaction";

            var client = PrepareClient<AppSettings>((options) =>
            {
                CreateMocks(
                    out var addressValidator,
                    out var healthProvider,
                    out var integrationInfoService,
                    out var transactionEstimator,
                    out var transactionBroadcaster,
                    out var transferAmountTransactionBuilder,
                    out var addressFormatsProvider);

                options.IntegrationName = $"{nameof(TransactionExecutorClientTests)}+{nameof(Broadcast_transaction)}";
                healthProvider.Setup(x => x.GetDiseaseAsync()).ReturnsAsync(disease);
                transactionBroadcaster.Setup(x => x.BroadcastAsync(It.IsAny<BroadcastTransactionRequest>()))
                    .ThrowsAsync(new TransactionBroadcastingException(TransactionBroadcastingError.RetryLater, "Error"));

                ConfigureFactories(options,
                    addressValidator,
                    healthProvider,
                    integrationInfoService,
                    transactionEstimator,
                    transactionBroadcaster,
                    transferAmountTransactionBuilder,
                    addressFormatsProvider);
            });

            //ACT && ASSERT
            Assert.ThrowsAsync<TransactionBroadcastingWebApiException>(async () =>
            {
                var request = new BroadcastTransactionRequest(Base58String.Encode(signedTransaction));
                await client.BroadcastTransactionAsync(request);
            });
        }

        [Test]
        public void Internal_server_error_broadcast_transaction()
        {
            //ARRANGE
            string disease = "Disease";
            string signedTransaction = "signedTransaction";

            var client = PrepareClient<AppSettings>(options =>
            {
                CreateMocks(
                    out var addressValidator,
                    out var healthProvider,
                    out var integrationInfoService,
                    out var transactionEstimator,
                    out var transactionBroadcaster,
                    out var transferAmountTransactionBuilder,
                    out var addressFormatsProvider);

                options.IntegrationName = $"{nameof(TransactionExecutorClientTests)}+{nameof(Broadcast_transaction)}";
                healthProvider.Setup(x => x.GetDiseaseAsync()).ReturnsAsync(disease);
                transactionBroadcaster.Setup(x => x.BroadcastAsync(It.IsAny<BroadcastTransactionRequest>()))
                    .ThrowsAsync(new Exception("Error"));

                ConfigureFactories(options,
                    addressValidator,
                    healthProvider,
                    integrationInfoService,
                    transactionEstimator,
                    transactionBroadcaster,
                    transferAmountTransactionBuilder,
                    addressFormatsProvider);
            });

            //ACT && ASSERT
            Assert.ThrowsAsync<InternalServerErrorWebApiException>(async () =>
            {
                var request = new BroadcastTransactionRequest(Base58String.Encode(signedTransaction));
                await client.BroadcastTransactionAsync(request);
            });
        }

        [Test]
        public async Task Transaction_raw()
        {
            //ARRANGE
            string disease = "Disease";
            string transactionId = "transactionId";
            string transactionResult = "result";

            var client = PrepareClient<AppSettings>((options) =>
            {
                CreateMocks(
                    out var addressValidator,
                    out var healthProvider,
                    out var integrationInfoService,
                    out var transactionEstimator,
                    out var transactionBroadcaster,
                    out var transferAmountTransactionBuilder,
                    out var addressFormatsProvider);

                var rawTransactionReadOnlyRepository = new Mock<IRawTransactionReadOnlyRepository>();
                options.IntegrationName = $"{nameof(TransactionExecutorClientTests)}+{nameof(Broadcast_transaction)}";
                healthProvider.Setup(x => x.GetDiseaseAsync()).ReturnsAsync(disease);
                rawTransactionReadOnlyRepository.Setup(x => x.GetOrDefaultAsync(transactionId))
                    .ReturnsAsync(Base58String.Encode(transactionResult));

                options.RawTransactionReadOnlyRepositoryFactory = (name, context) => rawTransactionReadOnlyRepository.Object;

                ConfigureFactories(options,
                    addressValidator,
                    healthProvider,
                    integrationInfoService,
                    transactionEstimator,
                    transactionBroadcaster,
                    transferAmountTransactionBuilder,
                    addressFormatsProvider);
            });

            //ACT
            var result = await client.GetRawTransactionAsync(transactionId);

            //ASSERT
            Assert.True(result != null);
            Assert.True(result.Raw.DecodeToString() == transactionResult);
        }

        [Test]
        public async Task Address_format()
        {
            //ARRANGE
            string disease = "Disease";
            string address = "0x1";
            IReadOnlyCollection<AddressFormat> formats = new AddressFormat[]
            {
                new AddressFormat(new Address(address),"format_1"),
                new AddressFormat(new Address("0"+address),"format_2"),
            };

            var client = PrepareClient<AppSettings>((options) =>
            {
                CreateMocks(
                    out var addressValidator,
                    out var healthProvider,
                    out var integrationInfoService,
                    out var transactionEstimator,
                    out var transactionBroadcaster,
                    out var transferAmountTransactionBuilder,
                    out var addressFormatsProvider);

                options.IntegrationName = $"{nameof(TransactionExecutorClientTests)}+{nameof(Broadcast_transaction)}";
                healthProvider.Setup(x => x.GetDiseaseAsync()).ReturnsAsync(disease);
                addressFormatsProvider.Setup(x => x.GetFormatsAsync(address)).ReturnsAsync(
                    new AddressFormatsResponse(formats));

                ConfigureFactories(options,
                    addressValidator,
                    healthProvider,
                    integrationInfoService,
                    transactionEstimator,
                    transactionBroadcaster,
                    transferAmountTransactionBuilder,
                    addressFormatsProvider);
            });

            //ACT
            var result = await client.GetAddressFormatsAsync(address);

            //ASSERT
            Assert.True(result != null);
            Assert.True(result.Formats.Count == 2);

            using (var enumerator = result.Formats.GetEnumerator())
            {
                foreach (var format in formats)
                {
                    enumerator.MoveNext();
                    Assert.True(format.Address.Value == enumerator.Current.Address.Value);
                    Assert.True(format.FormatName == enumerator.Current.FormatName);
                }
            }
        }

        private static void CreateMocks(out Mock<IAddressValidator> addressValidator,
            out Mock<IHealthProvider> healthProvider,
            out Mock<IIntegrationInfoService> integrationInfoService,
            out Mock<ITransferAmountTransactionsEstimator> transactionEstimator,
            out Mock<ITransactionBroadcaster> transactionBroadcaster,
            out Mock<ITransferAmountTransactionsBuilder> transferAmountTransactionBuilder,
            out Mock<IAddressFormatsProvider> addressFormatsProvider)
        {
            addressValidator = new Mock<IAddressValidator>();
            healthProvider = new Mock<IHealthProvider>();
            integrationInfoService = new Mock<IIntegrationInfoService>();
            transactionEstimator = new Mock<ITransferAmountTransactionsEstimator>();
            transactionBroadcaster = new Mock<ITransactionBroadcaster>();
            transferAmountTransactionBuilder = new Mock<ITransferAmountTransactionsBuilder>();
            addressFormatsProvider = new Mock<IAddressFormatsProvider>();
        }

        private static void ConfigureFactories(TransactionsExecutorServiceOptions<AppSettings> options,
            Mock<IAddressValidator> addressValidator,
            Mock<IHealthProvider> healthProvider,
            Mock<IIntegrationInfoService> integrationInfoService, 
            Mock<ITransferAmountTransactionsEstimator> transactionEstimator,
            Mock<ITransactionBroadcaster> transactionBroadcaster,
            Mock<ITransferAmountTransactionsBuilder> transferAmountTransactionBuilder,
            Mock<IAddressFormatsProvider> addressFormatsProvider)
        {
            options.AddressFormatsProviderFactory = c => addressFormatsProvider.Object;
            options.AddressValidatorFactory = c => addressValidator.Object;
            options.HealthProviderFactory = c => healthProvider.Object;
            options.IntegrationInfoServiceFactory = c => integrationInfoService.Object;
            options.TransferAmountTransactionsEstimatorFactory = c => transactionEstimator.Object;
            options.TransactionBroadcasterFactory = c => transactionBroadcaster.Object;
            options.TransferAmountTransactionsBuilderFactory = c => transferAmountTransactionBuilder.Object;
            options.DisableLogging = true;
        }

        private void PrepareSettings()
        {
            Environment.SetEnvironmentVariable("DisableAutoRegistrationInMonitoring", "true");
            Environment.SetEnvironmentVariable("SettingsUrl", _pathToSettings);

            var prepareSettings = new AppSettings()
            {
                Db = new DbSettings
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
            };

            string serializedSettings = Newtonsoft.Json.JsonConvert.SerializeObject(prepareSettings);

            try
            {
                File.Delete(_pathToSettings);
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch
            {
            }

            File.AppendAllText(_pathToSettings, serializedSettings);
        }

        private ITransactionsExecutorApi PrepareClient<TAppSettings>(Action<TransactionsExecutorServiceOptions<TAppSettings>> config)
            where TAppSettings : BaseTransactionsExecutorSettings<DbSettings>
        {
            StartupDependencyFactorySingleton.Instance = new StartupDependencyFactory<TAppSettings>(config);
            var client = CreateClientApi<StartupTemplate>("http://localhost:5000");

            return client;
        }
    }
}
