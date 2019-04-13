using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Lykke.Bil2.BaseTests;
using Lykke.Bil2.Client.SignService.Tests.Configuration;
using Lykke.Bil2.Contract.Common;
using Lykke.Bil2.Contract.Common.Responses;
using Lykke.Bil2.Contract.SignService.Requests;
using Lykke.Bil2.Contract.SignService.Responses;
using Lykke.Bil2.Sdk.Exceptions;
using Lykke.Bil2.Sdk.SignService;
using Lykke.Bil2.Sdk.SignService.Models;
using Lykke.Bil2.Sdk.SignService.Services;
using Lykke.Bil2.Sdk.SignService.Settings;
using Lykke.Bil2.SharedDomain;
using Lykke.Bil2.WebClient.Exceptions;
using Lykke.Sdk.Settings;
using Moq;
using NUnit.Framework;

namespace Lykke.Bil2.Client.SignService.Tests.Tests
{
    [TestFixture]
    public class SignServiceClientTests : SignServiceClientBase
    {
        private static readonly Base58String MyPublicKey = new Base58String("Vj75CuZgqYqhewfDfF9KQEdejjqbnoDiRjuXUnwCo2jkLo8AJpqBF6jFovufKrvwqUaubTRrAwr3wBBHtFVWhxhrxwFMoeB3mrBXnreVkfRdL1L9NUpyn4qDTB1Hwm3kBjmnhdVm2ZxmZ696FKj6yeBMnPB7Lkoa4XxKg6a9TwPCfgUbNV9b8dCPXm1YdYMdFK9Hf8sestxpp6FphTxbrjnicpeiU");
        private static readonly Base58String MyPrivateKey = new Base58String("hLLPYvqqGdBtPBMGWp98NbAHtDHuoj4vnrQqNPuMBMZpRFCX2kVtbC9HgrMQthM6NzEDjQw6c88PfiRCqzegb2KpTnBN1hqLyQKfT8FuGVDGRBJEYFZbgUbbb5e5PZZVnRwabyy4Rup8c5Yb5fWJ1Lw73yUtcxvmx52ymwm8m9j3NtVRTkcb5NsXGbMdanUpa9MdAMHReFDDh1qWx8N6zTb4st4WbaTuTamhcTEueuPKosaum5TLv2udcUDj2JtaZYUhDJ6EvotsHScwLQnwcW4ZkEydSCJgqefYp4n7qgNpzNKU2ffsDmPuQDfVBLNiFkqv99TMYsvrYbemcLLRoEUiqvGTaGZBpHErd5quYq1G9187nbySGy3aJCMVqVWzApuYKewUQDRND1fx6kybQpuDy6CjBp4i5sdAub4eFNHjRwVVUUsYQjjhy8PQpES5AWszv2DgMyrqEaAGYxr3akK6HmbYyBquWriAdYjp9v8NW29yhpSBXwyGpCT5j54KA2F5yaxv6MfUiucGHxYdgNFdkhSZYRB3FgZJQzTAy6utSZTapjcBmu25nzQcdaevnSW4rMd9kuhMGJL6PtyG6niHn48bsmwxSukGZsMvfRb8UDf3ZnGnNFHXChADzywM7ghZBtohBRgbnAyduUQ7GbLv6vhuyXV7qxAkvAwuz4mipe8MbTem6cjhKJrV1Sa4Ya6pUnWR7BhqNjWEKuVbCt8D9Et2no6u8hXxyxV6kD5tKEJ932PVHYBmnLQV9jCfS2RRv2BhBjhA6bUp8zCSuW2CyVYPCgmqQCa4EahA6iiHhymdaPQ8YGChbH8san");

        private static readonly Base58String MyPublicKey2 = new Base58String("HHhSTuePBZCYtqDNEPCSy3UJnH77zm4u9sSDX3EZXuCFuHAjFYxbAHJvK1VEyRRnBqSog71ek64MtzC9WvQoojzS1qRSFwEBFcvjx5JDecTQo8MDXUW4LufoxG9teN572QtmQPeahYThUDvTxhVHRxTNN5Em7gvd5SYMLgThREzTXV1Sn7ZUiJ8Fs3j7DbpwDhUaJEpcqdzB4Hd8YmoncUqAFYM6Hnpf7ZEfUaHfBf6L4bvjjQa4fx7KCNH1W6aP622UWemzHu1E1r6poDzqiyVZcKN1nbfHHHz75F7");
        private static readonly Base58String MyPrivateKey2 = new Base58String("DNfv4gHkDhJHoTo2F653XjAScBXXw5qyvyqKK41AkcfVLpcgURa8KLoF9FSxUfvt7jxUUuv2yFhV1ede8LANCzdFMSPLV6pNBm5EpU4tY6sTKzhJeLQCw7KBhA1us58yZRXFAeakozo8fjA32v94wefe6eSzmiZv6tukTzie1vdKRNakuxbGodzmMmQocpC3wyp26aheCLHkbny5vCGwpUHAmWPMc4LpMkyCoE5cVdvgSdc8a9bpJ4RgHMEtKfBkqY1pjCzrrdhrtcKve9CMuuS2tftRZGMPpZZwqj25th9b9P1K3jRbzt8Z3fvjDYNYGuq2165yRpgNzihs9XAYwL9DdXWtFS1K8pAKJoVBqMZfdXqvcVtAAMyTH7JxMtDmUFZgJZfg3SGGQrvMYZCRTJq7CjxsynsPagVzJoEhEqGzdbXwbX91b9uNaAP1zzeoXpnXZWHpYXmKqb3yUAKe7ymf4ibnM3JA7UYVNW9bkUbNmcMfMbi9rtCvTb5c9D2tss56JYsbuYnXoPBnwiwSZzhJuccBDa5KPVqzApRf4ZsmgjqTxEYWWGZV8jWzp1qsdR7kuxCQKjQ8ubS24uWMi28Ppxho6NSuppAQzPiFk4owfAzqnzbtDA5W9oCQoBXyxa7Cv1w9m9GPn4dqMVGchmbjLnTHiekkmaiXSG7HtjZUPEBJxsguiRPFEVCMywkcau7hnoFv9acn2fL3pxdYwvE6Q5ZxPnQfZKFaZurF2427L3QoBuoJsVa1oesysYb8tWT9r21gRpddT2yMnPFQeDhje9yQZxQqpGYLr6jAmATtovZ4p9ncRS5fFH3iYr49fT93pSYHdD4hXe9bkY34QZnTMMYrurHSr9L91iAaQm7WBFEUHymBgddMfUcBakfJiifgHf8GBf2cC9PH146HJ5AwkooTUNZzrZUP2Pu8S3WgJk7byRajDty8H1qANSaXRqbXCfNmnLsvXKPA47GepfywQBhcFtQns8mzrBuvvSTry166n2porcEodMkhAb5pvN11BET2ebMSLUKWKZes1Qj4ezoZmudA9pLCoNSkrUWk4GTfn9A37TLSjxQuiw6uBUMB2uepVJ81Dy4VCNsqW");

        private static readonly string _pathToSettings = "appsettings.tests.json";

        [OneTimeSetUp]
        public void GlobalSetup()
        {
            var prepareSettings = new AppSettings()
            {
                EncryptionPrivateKey = MyPrivateKey.ToString(),
                MonitoringServiceClient = new MonitoringServiceClientSettings()
                    { MonitoringServiceUrl = "http://localhost:5431" }
            };

            var settingsMock = new SettingsMock(_pathToSettings);
            settingsMock.PrepareSettings(prepareSettings);
        }

        [OneTimeTearDown]
        public void GlobalTeardown()
        {
        }

        [Test]
        public async Task Can_create_private_key()
        {
            //ARRANGE
            var address = Guid.NewGuid().ToString();
            var privateKey = Guid.NewGuid().ToString();
            var addressContext = "AddressContext";

            var client = PrepareClient<AppSettings>((options) =>
            {
                Mock<IAddressGenerator> addressGenerator = new Mock<IAddressGenerator>();
                Mock<ITransactionSigner> transactionSigner = new Mock<ITransactionSigner>();

                addressGenerator.Setup(x => x.CreateAddressAsync())
                    .ReturnsAsync(new AddressCreationResult(
                        address,
                        privateKey,
                        Base58String.Encode(addressContext)));

                options.IntegrationName = $"{nameof(SignServiceClientTests)}+{nameof(Can_create_private_key)}";
                options.AddressGeneratorFactory = (context) => addressGenerator.Object;
                options.TransactionSignerFactory = (context) => transactionSigner.Object;
            });

            //ACT
            var base58EncryptionKey = MyPublicKey;
            var request = new CreateAddressRequest(base58EncryptionKey);
            CreateAddressResponse result = await client.CreateAddressAsync(request);
            var decryptedPk = result?.PrivateKey?.DecryptToString(MyPrivateKey);

            //ASSERT
            Assert.True(result != null);
            Assert.True(!string.IsNullOrEmpty(result.Address.Value));
            Assert.True(result.Address.Value == address);
            Assert.True(decryptedPk == privateKey);
            Assert.True(result.AddressContext?.DecodeToString() == addressContext);
        }

        [Test]
        public async Task Can_create_address_tag()
        {
            //ARRANGE
            var address = Guid.NewGuid().ToString();
            var tag = Guid.NewGuid().ToString();
            var addressContext = "AddressContext";

            var client = PrepareClient<AppSettings>((options) =>
            {
                Mock<IAddressGenerator> addressGenerator = new Mock<IAddressGenerator>();
                Mock<ITransactionSigner> transactionSigner = new Mock<ITransactionSigner>();

                addressGenerator.Setup(x => x.CreateAddressTagAsync(It.IsAny<string>(), It.IsAny<CreateAddressTagRequest>()))
                    .ReturnsAsync(new CreateAddressTagResponse(new AddressTag(tag), 
                        Base58String.Encode(addressContext)));

                options.IntegrationName = $"{nameof(SignServiceClientTests)}+{nameof(Can_create_address_tag)}";
                options.AddressGeneratorFactory = (context) => addressGenerator.Object;
                options.TransactionSignerFactory = (context) => transactionSigner.Object;
            });

            //ACT
            var request = new CreateAddressTagRequest(Base58String.Encode(addressContext), AddressTagType.Text);
            CreateAddressTagResponse result = await client.CreateAddressTagAsync(address, request);

            //ASSERT
            Assert.True(result != null);
            Assert.True(result.Tag?.Value == tag);
            Assert.True(result.TagContext?.DecodeToString() == addressContext);
        }

        [Test]
        public async Task Can_sign_transaction()
        {
            //ARRANGE
            var privateKeys = new List<EncryptedString>
            {
                EncryptedString.Encrypt(MyPublicKey, MyPrivateKey.DecodeToString()),
                EncryptedString.Encrypt(MyPublicKey, MyPrivateKey2.DecodeToString()),
            };

            var transactionId = "TransactionId";
            var signedTransaction = "From.x01.To.x02.Amount.100.Signature.F1T2A100";

            var client = PrepareClient<AppSettings>((options) =>
            {
                Mock<IAddressGenerator> addressGenerator = new Mock<IAddressGenerator>();
                Mock<ITransactionSigner> transactionSigner = new Mock<ITransactionSigner>();

                transactionSigner.Setup(x => x.SignAsync(It.IsAny<IReadOnlyCollection<string>>(), It.IsAny<Base58String>()))
                    .ReturnsAsync(new SignTransactionResponse(Base58String.Encode(signedTransaction), transactionId));

                options.IntegrationName = $"{nameof(SignServiceClientTests)}+{nameof(Can_sign_transaction)}";
                options.AddressGeneratorFactory = (context) => addressGenerator.Object;
                options.TransactionSignerFactory = (context) => transactionSigner.Object;
            });

            //ACT
            var request = new SignTransactionRequest(privateKeys, Base58String.Encode(signedTransaction));
            SignTransactionResponse result = await client.SignTransactionAsync(request);

            //ASSERT
            Assert.True(result != null);
            Assert.True(result.TransactionId == transactionId);
            Assert.True(result.SignedTransaction.DecodeToString() == signedTransaction);
        }

        [Test]
        public async Task Get_is_alive()
        {
            //ARRANGE
            var client = PrepareClient<AppSettings>((options) =>
            {
                Mock<IAddressGenerator> addressGenerator = new Mock<IAddressGenerator>();
                Mock<ITransactionSigner> transactionSigner = new Mock<ITransactionSigner>();

                options.IntegrationName = $"{nameof(SignServiceClientTests)}+{nameof(Can_sign_transaction)}";
                options.AddressGeneratorFactory = (context) => addressGenerator.Object;
                options.TransactionSignerFactory = (context) => transactionSigner.Object;
            });

            //ACT
            BlockchainIsAliveResponse result = await client.GetIsAliveAsync();

            //ASSERT
            Assert.True(result != null);
        }

        [Test]
        public void Not_supported_private_key_creation()
        {
            //ARRANGE
            var client = PrepareClient<AppSettings>((options) =>
            {
                Mock<IAddressGenerator> addressGenerator = new Mock<IAddressGenerator>();
                Mock<ITransactionSigner> transactionSigner = new Mock<ITransactionSigner>();

                addressGenerator.Setup(x => x.CreateAddressAsync())
                    .ThrowsAsync(new OperationNotSupportedException("Address creation is not supported"));

                options.IntegrationName = $"{nameof(SignServiceClientTests)}+{nameof(Can_create_private_key)}";
                options.AddressGeneratorFactory = (context) => addressGenerator.Object;
                options.TransactionSignerFactory = (context) => transactionSigner.Object;
            });

            //ACT && ASSERT
            var base58EncryptionKey = MyPublicKey;
            var request = new CreateAddressRequest(base58EncryptionKey);

            Assert.ThrowsAsync<NotImplementedWebApiException>(async () =>
            {
                CreateAddressResponse result = await client.CreateAddressAsync(request);
                var decryptedPk = result?.PrivateKey?.DecryptToString(MyPrivateKey);
            });
        }

        [Test]
        public void Not_supported_address_tag_creation()
        {
            //ARRANGE
            var address = Guid.NewGuid().ToString();
            var addressContext = "AddressContext";

            var client = PrepareClient<AppSettings>((options) =>
            {
                Mock<IAddressGenerator> addressGenerator = new Mock<IAddressGenerator>();
                Mock<ITransactionSigner> transactionSigner = new Mock<ITransactionSigner>();

                addressGenerator.Setup(x => x.CreateAddressTagAsync(It.IsAny<string>(), It.IsAny<CreateAddressTagRequest>()))
                    .ThrowsAsync(new OperationNotSupportedException("Tag creation operation is not supported."));

                options.IntegrationName = $"{nameof(SignServiceClientTests)}+{nameof(Can_create_address_tag)}";
                options.AddressGeneratorFactory = (context) => addressGenerator.Object;
                options.TransactionSignerFactory = (context) => transactionSigner.Object;
            });

            //ACT && ASSERT

            Assert.ThrowsAsync<NotImplementedWebApiException>(async () =>
            {
                var request = new CreateAddressTagRequest(Base58String.Encode(addressContext), AddressTagType.Text);
                CreateAddressTagResponse result = await client.CreateAddressTagAsync(address, request);
            });
            
        }

        [Test]
        public void Check_timeout()
        {
            //ARRANGE
            var address = Guid.NewGuid().ToString();
            var tag = Guid.NewGuid().ToString();
            var addressContext = "AddressContext";
            var timeout = TimeSpan.FromMilliseconds(100);

            var client = PrepareClient<AppSettings>((options) =>
            {
                Mock<IAddressGenerator> addressGenerator = new Mock<IAddressGenerator>();
                Mock<ITransactionSigner> transactionSigner = new Mock<ITransactionSigner>();

                addressGenerator.Setup(x =>
                        x.CreateAddressTagAsync(It.IsAny<string>(), It.IsAny<CreateAddressTagRequest>()))
                    .ReturnsAsync(() =>
                    {
                        Thread.Sleep(timeout);
                        return new CreateAddressTagResponse(new AddressTag(tag),
                            Base58String.Encode(addressContext));
                    });

                options.IntegrationName = $"{nameof(SignServiceClientTests)}+{nameof(Can_create_address_tag)}";
                options.AddressGeneratorFactory = (context) => addressGenerator.Object;
                options.TransactionSignerFactory = (context) => transactionSigner.Object;
            }, timeout);

            //ACT && ASSERT
            Assert.ThrowsAsync<TimeoutException>(async () =>
            {
                var request = new CreateAddressTagRequest(Base58String.Encode(addressContext), AddressTagType.Text);
                var result = await client.CreateAddressTagAsync(address, request);
            });
        }

        private ISignServiceApi PrepareClient<TAppSettings>(Action<SignServiceOptions<TAppSettings>> config, TimeSpan? timeout = null)
            where TAppSettings : BaseSignServiceSettings
        {
            StartupDependencyFactorySingleton.Instance = new StartupDependencyFactory<TAppSettings>(config);
            var client = CreateClientApi<StartupTemplate>(
                "TestIntegration",
                "http://localhost:5000", 
                timeout);

            return client;
        }
    }
}
