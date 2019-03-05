﻿using Lykke.Bil2.Client.TransactionExecutor.Tests.Configuration;
using Lykke.Bil2.Client.TransactionsExecutor;
using Lykke.Bil2.Client.TransactionsExecutor.Exceptions;
using Lykke.Bil2.Contract.Common;
using Lykke.Bil2.Contract.Common.Exceptions;
using Lykke.Bil2.Contract.TransactionsExecutor;
using Lykke.Bil2.Contract.TransactionsExecutor.Requests;
using Lykke.Bil2.Contract.TransactionsExecutor.Responses;
using Lykke.Bil2.Sdk.TransactionsExecutor;
using Lykke.Bil2.Sdk.TransactionsExecutor.Exceptions;
using Lykke.Bil2.Sdk.TransactionsExecutor.Models;
using Lykke.Bil2.Sdk.TransactionsExecutor.Repositories;
using Lykke.Bil2.Sdk.TransactionsExecutor.Services;
using Lykke.Bil2.Sdk.TransactionsExecutor.Settings;
using Lykke.Bil2.WebClient.Exceptions;
using Lykke.Sdk.Settings;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;

namespace Lykke.Bil2.Client.TransactionExecutor.Tests.Tests
{
    [TestFixture]
    public class TransactionExecutorClientTests : TransactionsExecutorClientBase
    {
        private const string Disease = "Disease";
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
            var client = PrepareClient<AppSettings>((options) =>
            {
                var aggregator = CreateMocks();

                options.IntegrationName = $"{nameof(TransactionExecutorClientTests)}+{nameof(Get_is_alive)}";
                aggregator.HealthProvider.Setup(x => x.GetDiseaseAsync()).ReturnsAsync(Disease);

                options.AddressValidatorFactory = c => aggregator.AddressValidator.Object;
                options.HealthProviderFactory = c => aggregator.HealthProvider.Object;
                options.IntegrationInfoServiceFactory = c => aggregator.IntegrationInfoService.Object;
                options.TransferAmountTransactionsEstimatorFactory = c => aggregator.TransactionEstimator.Object;
                options.TransactionBroadcasterFactory = c => aggregator.TransactionBroadcaster.Object;
                options.TransferAmountTransactionsBuilderFactory = c => aggregator.TransferAmountTransactionBuilder.Object;
                options.TransactionsStateProviderFactory = c => aggregator.TransactionStateProvider.Object;
                options.DisableLogging = true;
            });

            //ACT
            var result = await client.GetIsAliveAsync();

            //ASSERT
            Assert.True(result != null);
            Assert.True(result.Disease == Disease);
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

            var client = PrepareClient<AppSettings>((options) =>
            {
                var aggregator = CreateMocks();

                options.IntegrationName = $"{nameof(TransactionExecutorClientTests)}+{nameof(Get_integration_info)}";
                aggregator.IntegrationInfoService.Setup(x => x.GetInfoAsync())
                    .ReturnsAsync(new IntegrationInfo(blockchainInfo, dependencies));
                aggregator.HealthProvider.Setup(x => x.GetDiseaseAsync()).ReturnsAsync(Disease);

                ConfigureFactories(options,
                    aggregator.AddressValidator,
                    aggregator.HealthProvider,
                    aggregator.IntegrationInfoService,
                    aggregator.TransactionEstimator,
                    aggregator.TransactionBroadcaster,
                    aggregator.TransferAmountTransactionBuilder,
                    aggregator.AddressFormatsProvider,
                    aggregator.TransactionStateProvider);
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
            var client = PrepareClient<AppSettings>((options) =>
            {
                var aggregator = CreateMocks();

                options.IntegrationName = $"{nameof(TransactionExecutorClientTests)}+{nameof(Get_address_validity)}";
                aggregator.HealthProvider.Setup(x => x.GetDiseaseAsync()).ReturnsAsync(Disease);
                aggregator.AddressValidator.Setup(x => x.ValidateAsync(address, tagType, tag))
                    .ReturnsAsync(new AddressValidityResponse(validationResult));

                ConfigureFactories(options,
                    aggregator.AddressValidator,
                    aggregator.HealthProvider,
                    aggregator.IntegrationInfoService,
                    aggregator.TransactionEstimator,
                    aggregator.TransactionBroadcaster,
                    aggregator.TransferAmountTransactionBuilder,
                    aggregator.AddressFormatsProvider,
                    aggregator.TransactionStateProvider);
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
            string transactionResponse = "transactionResponse";

            var client = PrepareClient<AppSettings>((options) =>
            {
                var aggregator = CreateMocks();

                options.IntegrationName = $"{nameof(TransactionExecutorClientTests)}+{nameof(Build_transfer_amount_transaction)}";
                aggregator.HealthProvider.Setup(x => x.GetDiseaseAsync()).ReturnsAsync(Disease);
                aggregator.TransferAmountTransactionBuilder.Setup(x => x.BuildTransferAmountAsync(It.IsAny<BuildTransferAmountTransactionRequest>()))
                    .ReturnsAsync(new BuildTransactionResponse(Base58String.Encode(transactionResponse)));

                ConfigureFactories(options,
                    aggregator.AddressValidator,
                    aggregator.HealthProvider,
                    aggregator.IntegrationInfoService,
                    aggregator.TransactionEstimator,
                    aggregator.TransactionBroadcaster,
                    aggregator.TransferAmountTransactionBuilder,
                    aggregator.AddressFormatsProvider,
                    aggregator.TransactionStateProvider);
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
            var client = PrepareClient<AppSettings>((options) =>
            {
                var aggregator = CreateMocks();

                options.IntegrationName = $"{nameof(TransactionExecutorClientTests)}+{nameof(Bad_request_while_building_transfer_amount_transaction)}";
                aggregator.HealthProvider.Setup(x => x.GetDiseaseAsync()).ReturnsAsync(Disease);
                aggregator.TransferAmountTransactionBuilder.Setup(x => x.BuildTransferAmountAsync(It.IsAny<BuildTransferAmountTransactionRequest>()))
                    .ThrowsAsync(new RequestValidationException("NOT VALID"));

                ConfigureFactories(options,
                    aggregator.AddressValidator,
                    aggregator.HealthProvider,
                    aggregator.IntegrationInfoService,
                    aggregator.TransactionEstimator,
                    aggregator.TransactionBroadcaster,
                    aggregator.TransferAmountTransactionBuilder,
                    aggregator.AddressFormatsProvider,
                    aggregator.TransactionStateProvider);
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
            var client = PrepareClient<AppSettings>((options) =>
            {
                var aggregator = CreateMocks();

                options.IntegrationName = $"{nameof(TransactionExecutorClientTests)}+{nameof(Node_issues_while_building_transfer_amount_transaction)}";
                aggregator.HealthProvider.Setup(x => x.GetDiseaseAsync()).ReturnsAsync(Disease);
                aggregator.TransferAmountTransactionBuilder.Setup(x => x.BuildTransferAmountAsync(It.IsAny<BuildTransferAmountTransactionRequest>()))
                    .ThrowsAsync(
                        new TransactionBuildingException(
                            TransactionBuildingError.RetryLater,
                            "Node is too busy"));

                ConfigureFactories(options,
                    aggregator.AddressValidator,
                    aggregator.HealthProvider,
                    aggregator.IntegrationInfoService,
                    aggregator.TransactionEstimator,
                    aggregator.TransactionBroadcaster,
                    aggregator.TransferAmountTransactionBuilder,
                    aggregator.AddressFormatsProvider,
                    aggregator.TransactionStateProvider);
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
            var dict = new Dictionary<AssetId, CoinsAmount>()
            {
                { new AssetId("asset"), CoinsAmount.FromDecimal(1000, 4) }
            };


            var client = PrepareClient<AppSettings>((options) =>
            {
                var aggregator = CreateMocks();

                options.IntegrationName = $"{nameof(TransactionExecutorClientTests)}+{nameof(Estimate_transfer_amount_transaction)}";
                aggregator.HealthProvider.Setup(x => x.GetDiseaseAsync()).ReturnsAsync(Disease);
                aggregator.TransactionEstimator.Setup(x => x.EstimateTransferAmountAsync(It.IsAny<EstimateTransferAmountTransactionRequest>()))
                    .ReturnsAsync(new EstimateTransactionResponse(dict));

                ConfigureFactories(options,
                    aggregator.AddressValidator,
                    aggregator.HealthProvider,
                    aggregator.IntegrationInfoService,
                    aggregator.TransactionEstimator,
                    aggregator.TransactionBroadcaster,
                    aggregator.TransferAmountTransactionBuilder,
                    aggregator.AddressFormatsProvider,
                    aggregator.TransactionStateProvider);
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
            var client = PrepareClient<AppSettings>((options) =>
            {
                var aggregator = CreateMocks();

                options.IntegrationName = $"{nameof(TransactionExecutorClientTests)}+{nameof(Bad_request_while_estimating_transfer_amount_transaction)}";
                aggregator.HealthProvider.Setup(x => x.GetDiseaseAsync()).ReturnsAsync(Disease);
                aggregator.TransactionEstimator.Setup(x => x.EstimateTransferAmountAsync(It.IsAny<EstimateTransferAmountTransactionRequest>()))
                    .ThrowsAsync(new RequestValidationException("Not VALID"));

                ConfigureFactories(options,
                    aggregator.AddressValidator,
                    aggregator.HealthProvider,
                    aggregator.IntegrationInfoService,
                    aggregator.TransactionEstimator,
                    aggregator.TransactionBroadcaster,
                    aggregator.TransferAmountTransactionBuilder,
                    aggregator.AddressFormatsProvider,
                    aggregator.TransactionStateProvider);
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
            string signedTransaction = "signedTransaction";

            var client = PrepareClient<AppSettings>((options) =>
            {
                var aggregator = CreateMocks();

                options.IntegrationName = $"{nameof(TransactionExecutorClientTests)}+{nameof(Broadcast_transaction)}";
                aggregator.HealthProvider.Setup(x => x.GetDiseaseAsync()).ReturnsAsync(Disease);
                aggregator.TransactionBroadcaster.Setup(x => x.BroadcastAsync(It.IsAny<BroadcastTransactionRequest>()))
                    .Returns(Task.CompletedTask);

                ConfigureFactories(options,
                    aggregator.AddressValidator,
                    aggregator.HealthProvider,
                    aggregator.IntegrationInfoService,
                    aggregator.TransactionEstimator,
                    aggregator.TransactionBroadcaster,
                    aggregator.TransferAmountTransactionBuilder,
                    aggregator.AddressFormatsProvider,
                    aggregator.TransactionStateProvider);
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
            string signedTransaction = "signedTransaction";

            var client = PrepareClient<AppSettings>((options) =>
            {
                var aggregator = CreateMocks();

                options.IntegrationName = $"{nameof(TransactionExecutorClientTests)}+{nameof(Broadcast_transaction)}";
                aggregator.HealthProvider.Setup(x => x.GetDiseaseAsync()).ReturnsAsync(Disease);
                aggregator.TransactionBroadcaster.Setup(x => x.BroadcastAsync(It.IsAny<BroadcastTransactionRequest>()))
                    .ThrowsAsync(new TransactionBroadcastingException(TransactionBroadcastingError.RetryLater, "Error"));

                ConfigureFactories(options,
                    aggregator.AddressValidator,
                    aggregator.HealthProvider,
                    aggregator.IntegrationInfoService,
                    aggregator.TransactionEstimator,
                    aggregator.TransactionBroadcaster,
                    aggregator.TransferAmountTransactionBuilder,
                    aggregator.AddressFormatsProvider,
                    aggregator.TransactionStateProvider);
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
            string signedTransaction = "signedTransaction";

            var client = PrepareClient<AppSettings>(options =>
            {
                var aggregator = CreateMocks();

                options.IntegrationName = $"{nameof(TransactionExecutorClientTests)}+{nameof(Broadcast_transaction)}";
                aggregator.HealthProvider.Setup(x => x.GetDiseaseAsync()).ReturnsAsync(Disease);
                aggregator.TransactionBroadcaster.Setup(x => x.BroadcastAsync(It.IsAny<BroadcastTransactionRequest>()))
                    .ThrowsAsync(new Exception("Error"));

                ConfigureFactories(options,
                    aggregator.AddressValidator,
                    aggregator.HealthProvider,
                    aggregator.IntegrationInfoService,
                    aggregator.TransactionEstimator,
                    aggregator.TransactionBroadcaster,
                    aggregator.TransferAmountTransactionBuilder,
                    aggregator.AddressFormatsProvider,
                    aggregator.TransactionStateProvider);
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
            string transactionId = "transactionId";
            string transactionResult = "result";

            var client = PrepareClient<AppSettings>((options) =>
            {
                var aggregator = CreateMocks();

                var rawTransactionReadOnlyRepository = new Mock<IRawTransactionReadOnlyRepository>();
                options.IntegrationName = $"{nameof(TransactionExecutorClientTests)}+{nameof(Broadcast_transaction)}";
                aggregator.HealthProvider.Setup(x => x.GetDiseaseAsync()).ReturnsAsync(Disease);
                rawTransactionReadOnlyRepository.Setup(x => x.GetOrDefaultAsync(transactionId))
                    .ReturnsAsync(Base58String.Encode(transactionResult));

                options.RawTransactionReadOnlyRepositoryFactory = (name, context) => rawTransactionReadOnlyRepository.Object;

                ConfigureFactories(options,
                    aggregator.AddressValidator,
                    aggregator.HealthProvider,
                    aggregator.IntegrationInfoService,
                    aggregator.TransactionEstimator,
                    aggregator.TransactionBroadcaster,
                    aggregator.TransferAmountTransactionBuilder,
                    aggregator.AddressFormatsProvider,
                    aggregator.TransactionStateProvider);
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
            string address = "0x1";
            IReadOnlyCollection<AddressFormat> formats = new AddressFormat[]
            {
                new AddressFormat(new Address(address),"format_1"),
                new AddressFormat(new Address("0"+address),"format_2"),
            };

            var client = PrepareClient<AppSettings>((options) =>
            {
                var aggregator = CreateMocks();

                options.IntegrationName = $"{nameof(TransactionExecutorClientTests)}+{nameof(Broadcast_transaction)}";
                aggregator.HealthProvider.Setup(x => x.GetDiseaseAsync()).ReturnsAsync(Disease);
                aggregator.AddressFormatsProvider.Setup(x => x.GetFormatsAsync(address)).ReturnsAsync(
                    new AddressFormatsResponse(formats));

                ConfigureFactories(options,
                    aggregator.AddressValidator,
                    aggregator.HealthProvider,
                    aggregator.IntegrationInfoService,
                    aggregator.TransactionEstimator,
                    aggregator.TransactionBroadcaster,
                    aggregator.TransferAmountTransactionBuilder,
                    aggregator.AddressFormatsProvider,
                    aggregator.TransactionStateProvider);
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

        [Test]
        public async Task Build_transfer_coins_transaction()
        {
            //ARRANGE
            string transactionResponse = "transactionResponse";

            var client = PrepareClient<AppSettings>((options) =>
            {
                var aggregator = CreateMocks();

                options.IntegrationName = $"{nameof(TransactionExecutorClientTests)}+{nameof(Build_transfer_coins_transaction)}";
                aggregator.HealthProvider.Setup(x => x.GetDiseaseAsync()).ReturnsAsync(Disease);
                aggregator.TransferCoinsTransactionsBuilder
                    .Setup(x => x.BuildTransferCoinsAsync(It.IsAny<BuildTransferCoinsTransactionRequest>()))
                    .ReturnsAsync(new BuildTransactionResponse(Base58String.Encode(transactionResponse)));

                ConfigureFactories(options,
                    aggregator.AddressValidator,
                    aggregator.HealthProvider,
                    aggregator.IntegrationInfoService,
                    aggregator.TransactionEstimator,
                    aggregator.TransactionBroadcaster,
                    aggregator.TransferAmountTransactionBuilder,
                    aggregator.AddressFormatsProvider,
                    aggregator.TransactionStateProvider,
                    aggregator.TransferCoinsTransactionsBuilder,
                    aggregator.TransferCoinsTransactionsEstimator);
            });

            //ACT
            var coinsToSpend = new[]
            {
                new CoinToSpend(new CoinReference("tx1", 0),
                    new AssetId("assetId"),
                    CoinsAmount.FromDecimal(1000, 4),
                    new Address("0x1"),
                    Base58String.Encode("context"),
                    1),
            };
            var coinsToReceive = new[]
            {
                new CoinToReceive(0,
                    new AssetId("assetId"),
                    CoinsAmount.FromDecimal(1000, 4),
                    new Address("0x2"),
                    new AddressTag("tag"),
                    AddressTagType.Text
                    ),
            };
            var expirationOptions = new ExpirationOptions(DateTime.Now + TimeSpan.FromDays(1));

            var request = new BuildTransferCoinsTransactionRequest(coinsToSpend, coinsToReceive, expirationOptions);
            var result = await client.BuildTransferCoinsTransactionAsync(request);

            //ASSERT

            Assert.True(result != null);
            Assert.True(result.TransactionContext.DecodeToString() == transactionResponse);
        }

        [Test]
        public async Task Not_implemented_transfer_coins_transaction()
        {
            //ARRANGE
            string transactionResponse = "transactionResponse";

            var client = PrepareClient<AppSettings>((options) =>
            {
                var aggregator = CreateMocks();

                options.IntegrationName = $"{nameof(TransactionExecutorClientTests)}+{nameof(Not_implemented_transfer_coins_transaction)}";
                aggregator.HealthProvider.Setup(x => x.GetDiseaseAsync()).ReturnsAsync(Disease);

                ConfigureFactories(options,
                    aggregator.AddressValidator,
                    aggregator.HealthProvider,
                    aggregator.IntegrationInfoService,
                    aggregator.TransactionEstimator,
                    aggregator.TransactionBroadcaster,
                    aggregator.TransferAmountTransactionBuilder,
                    aggregator.AddressFormatsProvider,
                    aggregator.TransactionStateProvider);
            });

            //ACT && ASSERT
            var coinsToSpend = new[]
            {
                new CoinToSpend(new CoinReference("tx1", 0),
                    new AssetId("assetId"),
                    CoinsAmount.FromDecimal(1000, 4),
                    new Address("0x1"),
                    Base58String.Encode("context"),
                    1),
            };
            var coinsToReceive = new[]
            {
                new CoinToReceive(0,
                    new AssetId("assetId"),
                    CoinsAmount.FromDecimal(1000, 4),
                    new Address("0x2"),
                    new AddressTag("tag"),
                    AddressTagType.Text
                    ),
            };
            var expirationOptions = new ExpirationOptions(DateTime.Now + TimeSpan.FromDays(1));

            Assert.ThrowsAsync<NotImplementedWebApiException>(async () =>
            {
                var request = new BuildTransferCoinsTransactionRequest(coinsToSpend, coinsToReceive, expirationOptions);
                var result = await client.BuildTransferCoinsTransactionAsync(request);
            });
        }

        [Test]
        public async Task Bad_request_transfer_coins_transaction()
        {
            //ARRANGE
            string transactionResponse = "transactionResponse";

            var client = PrepareClient<AppSettings>((options) =>
            {
                var aggregator = CreateMocks();

                options.IntegrationName = $"{nameof(TransactionExecutorClientTests)}+{nameof(Bad_request_transfer_coins_transaction)}";
                aggregator.HealthProvider.Setup(x => x.GetDiseaseAsync()).ReturnsAsync(Disease);
                aggregator.TransferCoinsTransactionsBuilder
                    .Setup(x => x.BuildTransferCoinsAsync(It.IsAny<BuildTransferCoinsTransactionRequest>()))
                    .ThrowsAsync(new TransactionBuildingException(TransactionBuildingError.RetryLater, "some error"));

                ConfigureFactories(options,
                    aggregator.AddressValidator,
                    aggregator.HealthProvider,
                    aggregator.IntegrationInfoService,
                    aggregator.TransactionEstimator,
                    aggregator.TransactionBroadcaster,
                    aggregator.TransferAmountTransactionBuilder,
                    aggregator.AddressFormatsProvider,
                    aggregator.TransactionStateProvider,
                    aggregator.TransferCoinsTransactionsBuilder,
                    aggregator.TransferCoinsTransactionsEstimator);
            });

            //ACT && ASSERT
            var coinsToSpend = new[]
            {
                new CoinToSpend(new CoinReference("tx1", 0),
                    new AssetId("assetId"),
                    CoinsAmount.FromDecimal(1000, 4),
                    new Address("0x1"),
                    Base58String.Encode("context"),
                    1),
            };
            var coinsToReceive = new[]
            {
                new CoinToReceive(0,
                    new AssetId("assetId"),
                    CoinsAmount.FromDecimal(1000, 4),
                    new Address("0x2"),
                    new AddressTag("tag"),
                    AddressTagType.Text
                    ),
            };
            var expirationOptions = new ExpirationOptions(DateTime.Now + TimeSpan.FromDays(1));

            Assert.ThrowsAsync<TransactionBuildingWebApiException>(async () =>
            {
                var request = new BuildTransferCoinsTransactionRequest(coinsToSpend, coinsToReceive, expirationOptions);
                var result = await client.BuildTransferCoinsTransactionAsync(request);
            });
        }

        [Test]
        public async Task Internal_server_error_transfer_coins_transaction()
        {
            //ARRANGE
            string transactionResponse = "transactionResponse";

            var client = PrepareClient<AppSettings>((options) =>
            {
                var aggregator = CreateMocks();

                options.IntegrationName = $"{nameof(TransactionExecutorClientTests)}+{nameof(Internal_server_error_transfer_coins_transaction)}";
                aggregator.HealthProvider.Setup(x => x.GetDiseaseAsync()).ReturnsAsync(Disease);
                aggregator.TransferCoinsTransactionsBuilder
                    .Setup(x => x.BuildTransferCoinsAsync(It.IsAny<BuildTransferCoinsTransactionRequest>()))
                    .ThrowsAsync(new Exception("some error"));

                ConfigureFactories(options,
                    aggregator.AddressValidator,
                    aggregator.HealthProvider,
                    aggregator.IntegrationInfoService,
                    aggregator.TransactionEstimator,
                    aggregator.TransactionBroadcaster,
                    aggregator.TransferAmountTransactionBuilder,
                    aggregator.AddressFormatsProvider,
                    aggregator.TransactionStateProvider,
                    aggregator.TransferCoinsTransactionsBuilder,
                    aggregator.TransferCoinsTransactionsEstimator);
            });

            //ACT && ASSERT
            var coinsToSpend = new[]
            {
                new CoinToSpend(new CoinReference("tx1", 0),
                    new AssetId("assetId"),
                    CoinsAmount.FromDecimal(1000, 4),
                    new Address("0x1"),
                    Base58String.Encode("context"),
                    1),
            };
            var coinsToReceive = new[]
            {
                new CoinToReceive(0,
                    new AssetId("assetId"),
                    CoinsAmount.FromDecimal(1000, 4),
                    new Address("0x2"),
                    new AddressTag("tag"),
                    AddressTagType.Text
                    ),
            };
            var expirationOptions = new ExpirationOptions(DateTime.Now + TimeSpan.FromDays(1));

            Assert.ThrowsAsync<InternalServerErrorWebApiException>(async () =>
            {
                var request = new BuildTransferCoinsTransactionRequest(coinsToSpend, coinsToReceive, expirationOptions);
                var result = await client.BuildTransferCoinsTransactionAsync(request);
            });
        }

        [Test]
        public async Task Estimate_transfer_coins_transaction()
        {
            //ARRANGE
            var dict = new Dictionary<AssetId, CoinsAmount>()
            {
                { new AssetId("asset"), CoinsAmount.FromDecimal(1000, 4) }
            };

            var client = PrepareClient<AppSettings>((options) =>
            {
                var aggregator = CreateMocks();

                options.IntegrationName = $"{nameof(TransactionExecutorClientTests)}+{nameof(Estimate_transfer_coins_transaction)}";
                aggregator.HealthProvider.Setup(x => x.GetDiseaseAsync()).ReturnsAsync(Disease);
                aggregator.TransferCoinsTransactionsEstimator
                    .Setup(x => x.EstimateTransferCoinsAsync(It.IsAny<EstimateTransferCoinsTransactionRequest>()))
                    .ReturnsAsync(new EstimateTransactionResponse(dict));

                ConfigureFactories(options,
                    aggregator.AddressValidator,
                    aggregator.HealthProvider,
                    aggregator.IntegrationInfoService,
                    aggregator.TransactionEstimator,
                    aggregator.TransactionBroadcaster,
                    aggregator.TransferAmountTransactionBuilder,
                    aggregator.AddressFormatsProvider,
                    aggregator.TransactionStateProvider,
                    aggregator.TransferCoinsTransactionsBuilder,
                    aggregator.TransferCoinsTransactionsEstimator);
            });

            //ACT
            var coinsToSpend = new[]
            {
                new CoinToSpend(new CoinReference("tx1", 0),
                    new AssetId("assetId"),
                    CoinsAmount.FromDecimal(1000, 4),
                    new Address("0x1"),
                    Base58String.Encode("context"),
                    1),
            };
            var coinsToReceive = new[]
            {
                new CoinToReceive(0,
                    new AssetId("assetId"),
                    CoinsAmount.FromDecimal(1000, 4),
                    new Address("0x2"),
                    new AddressTag("tag"),
                    AddressTagType.Text
                    ),
            };
            var expirationOptions = new ExpirationOptions(DateTime.Now + TimeSpan.FromDays(1));

            var request = new EstimateTransferCoinsTransactionRequest(coinsToSpend, coinsToReceive);
            var result = await client.EstimateTransferCoinsTransactionAsync(request);

            //ASSERT

            var estimation = result.EstimatedFee.First();
            Assert.True(result != null);
            Assert.True(estimation.Key == "asset");
            Assert.True(estimation.Value.ToDecimal() == CoinsAmount.FromDecimal(1000, 4).ToDecimal());
        }

        [Test]
        public async Task Internal_server_error_estimate_transfer_coins_transaction()
        {
            //ARRANGE
            var client = PrepareClient<AppSettings>((options) =>
            {
                var aggregator = CreateMocks();

                options.IntegrationName = 
                    $"{nameof(TransactionExecutorClientTests)}+{nameof(Internal_server_error_estimate_transfer_coins_transaction)}";
                aggregator.HealthProvider.Setup(x => x.GetDiseaseAsync()).ReturnsAsync(Disease);
                aggregator.TransferCoinsTransactionsEstimator
                    .Setup(x => x.EstimateTransferCoinsAsync(It.IsAny<EstimateTransferCoinsTransactionRequest>()))
                    .ThrowsAsync(new Exception("some error"));

                ConfigureFactories(options,
                    aggregator.AddressValidator,
                    aggregator.HealthProvider,
                    aggregator.IntegrationInfoService,
                    aggregator.TransactionEstimator,
                    aggregator.TransactionBroadcaster,
                    aggregator.TransferAmountTransactionBuilder,
                    aggregator.AddressFormatsProvider,
                    aggregator.TransactionStateProvider,
                    aggregator.TransferCoinsTransactionsBuilder,
                    aggregator.TransferCoinsTransactionsEstimator);
            });

            //ACT && ASSERT
            var coinsToSpend = new[]
            {
                new CoinToSpend(new CoinReference("tx1", 0),
                    new AssetId("assetId"),
                    CoinsAmount.FromDecimal(1000, 4),
                    new Address("0x1"),
                    Base58String.Encode("context"),
                    1),
            };
            var coinsToReceive = new[]
            {
                new CoinToReceive(0,
                    new AssetId("assetId"),
                    CoinsAmount.FromDecimal(1000, 4),
                    new Address("0x2"),
                    new AddressTag("tag"),
                    AddressTagType.Text
                    ),
            };
            var expirationOptions = new ExpirationOptions(DateTime.Now + TimeSpan.FromDays(1));

            Assert.ThrowsAsync<InternalServerErrorWebApiException>(async () =>
            {
                var request = new EstimateTransferCoinsTransactionRequest(coinsToSpend, coinsToReceive);
                var result = await client.EstimateTransferCoinsTransactionAsync(request);
            });
        }

        [Test]
        public async Task Bad_request_estimate_transfer_coins_transaction()
        {
            //ARRANGE
            var client = PrepareClient<AppSettings>((options) =>
            {
                var aggregator = CreateMocks();

                options.IntegrationName =
                    $"{nameof(TransactionExecutorClientTests)}+{nameof(Bad_request_estimate_transfer_coins_transaction)}";
                aggregator.HealthProvider.Setup(x => x.GetDiseaseAsync()).ReturnsAsync(Disease);
                aggregator.TransferCoinsTransactionsEstimator
                    .Setup(x => x.EstimateTransferCoinsAsync(It.IsAny<EstimateTransferCoinsTransactionRequest>()))
                    .ThrowsAsync(new RequestValidationException("some error"));

                ConfigureFactories(options,
                    aggregator.AddressValidator,
                    aggregator.HealthProvider,
                    aggregator.IntegrationInfoService,
                    aggregator.TransactionEstimator,
                    aggregator.TransactionBroadcaster,
                    aggregator.TransferAmountTransactionBuilder,
                    aggregator.AddressFormatsProvider,
                    aggregator.TransactionStateProvider,
                    aggregator.TransferCoinsTransactionsBuilder,
                    aggregator.TransferCoinsTransactionsEstimator);
            });

            //ACT && ASSERT
            var coinsToSpend = new[]
            {
                new CoinToSpend(new CoinReference("tx1", 0),
                    new AssetId("assetId"),
                    CoinsAmount.FromDecimal(1000, 4),
                    new Address("0x1"),
                    Base58String.Encode("context"),
                    1),
            };
            var coinsToReceive = new[]
            {
                new CoinToReceive(0,
                    new AssetId("assetId"),
                    CoinsAmount.FromDecimal(1000, 4),
                    new Address("0x2"),
                    new AddressTag("tag"),
                    AddressTagType.Text
                    ),
            };
            var expirationOptions = new ExpirationOptions(DateTime.Now + TimeSpan.FromDays(1));

            Assert.ThrowsAsync<BadRequestWebApiException>(async () =>
            {
                var request = new EstimateTransferCoinsTransactionRequest(coinsToSpend, coinsToReceive);
                var result = await client.EstimateTransferCoinsTransactionAsync(request);
            });
        }

        [Test]
        [TestCase(TransactionState.Accepted)]
        [TestCase(TransactionState.Broadcasted)]
        [TestCase(TransactionState.Mined)]
        [TestCase(TransactionState.Unknown)]
        public async Task Get_transaction_state(TransactionState state)
        {
            //ARRANGE
            var request = "transactionId";

            var client = PrepareClient<AppSettings>((options) =>
            {
                var aggregator = CreateMocks();

                options.IntegrationName = $"{nameof(TransactionExecutorClientTests)}+{nameof(Get_transaction_state)}";
                aggregator.HealthProvider.Setup(x => x.GetDiseaseAsync()).ReturnsAsync(Disease);
                aggregator.TransactionStateProvider
                    .Setup(x => x.GetStateAsync(It.IsAny<string>()))
                    .ReturnsAsync(state);

                ConfigureFactories(options,
                    aggregator.AddressValidator,
                    aggregator.HealthProvider,
                    aggregator.IntegrationInfoService,
                    aggregator.TransactionEstimator,
                    aggregator.TransactionBroadcaster,
                    aggregator.TransferAmountTransactionBuilder,
                    aggregator.AddressFormatsProvider,
                    aggregator.TransactionStateProvider,
                    aggregator.TransferCoinsTransactionsBuilder,
                    aggregator.TransferCoinsTransactionsEstimator);
            });

            //ACT
            var result = await client.GetTransactionStateAsync(request);

            //ASSERT

            Assert.True(result != null);
            Assert.True(result.State == state);
        }


        private static MockAggregator CreateMocks()
        {
            var aggregator = new MockAggregator();
            aggregator.AddressValidator = new Mock<IAddressValidator>();
            aggregator.HealthProvider = new Mock<IHealthProvider>();
            aggregator.IntegrationInfoService = new Mock<IIntegrationInfoService>();
            aggregator.TransactionEstimator = new Mock<ITransferAmountTransactionsEstimator>();
            aggregator.TransactionBroadcaster = new Mock<ITransactionBroadcaster>();
            aggregator.TransferAmountTransactionBuilder = new Mock<ITransferAmountTransactionsBuilder>();
            aggregator.AddressFormatsProvider = new Mock<IAddressFormatsProvider>();
            aggregator.TransactionStateProvider = new Mock<ITransactionsStateProvider>();
            aggregator.TransferCoinsTransactionsBuilder = new Mock<ITransferCoinsTransactionsBuilder>();
            aggregator.TransferCoinsTransactionsEstimator= new Mock<ITransferCoinsTransactionsEstimator>();

            return aggregator;
        }

        private static void ConfigureFactories(TransactionsExecutorServiceOptions<AppSettings> options,
            Mock<IAddressValidator> addressValidator,
            Mock<IHealthProvider> healthProvider,
            Mock<IIntegrationInfoService> integrationInfoService,
            Mock<ITransferAmountTransactionsEstimator> transactionEstimator,
            Mock<ITransactionBroadcaster> transactionBroadcaster,
            Mock<ITransferAmountTransactionsBuilder> transferAmountTransactionBuilder,
            Mock<IAddressFormatsProvider> addressFormatsProvider,
            Mock<ITransactionsStateProvider> transactionStateProvider,
            Mock<ITransferCoinsTransactionsBuilder> transferCoinsTransactionsBuilder = null,
            Mock<ITransferCoinsTransactionsEstimator> transferCoinsTransactionsEstimator = null)
        {
            options.AddressFormatsProviderFactory = c => addressFormatsProvider.Object;
            options.AddressValidatorFactory = c => addressValidator.Object;
            options.HealthProviderFactory = c => healthProvider.Object;
            options.IntegrationInfoServiceFactory = c => integrationInfoService.Object;
            options.TransferAmountTransactionsEstimatorFactory = c => transactionEstimator.Object;
            options.TransactionBroadcasterFactory = c => transactionBroadcaster.Object;
            options.TransferAmountTransactionsBuilderFactory = c => transferAmountTransactionBuilder.Object;
            options.TransactionsStateProviderFactory = c => transactionStateProvider.Object;
            options.DisableLogging = true;

            if (transferCoinsTransactionsBuilder != null)
                options.TransferCoinsTransactionsBuilderFactory = c => transferCoinsTransactionsBuilder.Object;

            if (transferCoinsTransactionsEstimator != null)
                options.TransferCoinsTransactionsEstimatorFactory = c => transferCoinsTransactionsEstimator.Object;
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
