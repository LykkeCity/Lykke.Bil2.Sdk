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
        private static readonly Base64String MyPublicKey = new Base64String("MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQCYK+LcJ2ws2l4OXA3X5LlmKfVIrbcGADjYwAt/SmR/vDUHbWyoL28PswcVXEqw4fO3sq1Ck5c3k677ruHcpmmhaDYc3G8vROYwgobsD+FgZqjyBAsxB3+tJcKlUtDzjOaaq0FxSm2cOEgQK7D0ldmdqceybDfigDRod756xJdUFwIDAQAB");
        private static readonly Base64String MyPrivateKey = new Base64String("MIICXAIBAAKBgQCYK+LcJ2ws2l4OXA3X5LlmKfVIrbcGADjYwAt/SmR/vDUHbWyoL28PswcVXEqw4fO3sq1Ck5c3k677ruHcpmmhaDYc3G8vROYwgobsD+FgZqjyBAsxB3+tJcKlUtDzjOaaq0FxSm2cOEgQK7D0ldmdqceybDfigDRod756xJdUFwIDAQABAoGAFMbJLq3jQyx9cxB2g2ejOKO57bZqKtOU72MpLrQFjLsxslXqY/w1+brD2NLFD+mJ0ScAKPrlxpzPY2W5SNsfyMbpvPXMlxZTQVbd1Xg8oITM2M5R71T+7S4oyzdzEsOkRkXcboFsVQvRTAod9I74fonFNgsEyH584+OK7md7P0ECQQDxNM4/b+4iVv3xSMp5j0EQhAiMWaIuD9cizanWQJNWNTS3pjCfoysq2vHzAGgldhU6VptaCvutJTt8WSl1U8FPAkEAoYEjmPKGvrOZRdnffyTlu5zMTzfcss5qLdCqdPILG5kyZJhERrCfSBD/rtkZDttOsXrv7SF4wUgZK/Ofvui+uQJBAIJhWc7+kMktHq0q/I9CuRfVVs2OsdSWKWMdql0uoLWrouhWQ9g2meHbYYdJxAHj10umfujoIOyRwJrRk1BhSo8CQEvtBCkxSzt3/4ShKrsBQ6dxzXMole7Rr4UeZiRYbfRpjxFPrDl3a0pcA3fVxDwByfsSCp12cOic1oidHeqITLECQFc405EIoK7ksU5rB7+SOIP9NZQfaBzWbwk2xRHC4uUtz7x3hZJQmIOECWXQ9DsldkOrYXGvQWVq+AC1jGRk9oc=");

        private static readonly Base64String MyPublicKey2 = new Base64String("TUlHZk1BMEdDU3FHU0liM0RRRUJBUVVBQTRHTkFEQ0JpUUtCZ1FDbDg1L3puOVpnaGEwZXJEbVRHZnhMb29WY2toWVp5dW1yTnpNMXB6RjZkZmNFZ2NwOWswUGhRcGoxajNmZGNUZGVrYUZZVjRTUlBhZUIwMG9wZ2VFKy9GbkV1TkVrYyttM1NpS0xhYmlSK3JxR3U1QWloei81Z053R0hpbS9HY2pvWkJPTFVBYlJ5czhNVnRycWx1Q3JaNVB2VWUyRno1VGhaT2hMeWlVLzV3SURBUUFC");
        private static readonly Base64String MyPrivateKey2 = new Base64String("TUlJQ1d3SUJBQUtCZ1FDbDg1L3puOVpnaGEwZXJEbVRHZnhMb29WY2toWVp5dW1yTnpNMXB6RjZkZmNFZ2NwOWswUGhRcGoxajNmZGNUZGVrYUZZVjRTUlBhZUIwMG9wZ2VFKy9GbkV1TkVrYyttM1NpS0xhYmlSK3JxR3U1QWloei81Z053R0hpbS9HY2pvWkJPTFVBYlJ5czhNVnRycWx1Q3JaNVB2VWUyRno1VGhaT2hMeWlVLzV3SURBUUFCQW9HQWFZdmpoWDE1U1haN3Z2Qm14ZXBYRG52Vk9pVW5yVXZqQmlSYmk1cEUyOUEvUlR5UFh3TFV2Mll1QnJBeTJrSnhwdElVdmkwYmdZeW5CdWt0Vit6bEpyTzYyR3Z6a2cwemQ1T3YzaThkaVg1Z1dtSEkrc2hJSGJTbGtlL0pxMSs5WmFnOTdESDZXVjQ0ZmE1OHNFQ3VLTkJaeldua1J4NHdmbUphNFZzdWZhRUNRUURyT24zOUNFdTVveFFrdEdpYXF3WVdnK1Vud3hhMWRvMkQzUVZ4dkQwNGhDN3dGbURhTW9nSHpoUURPOXNKM0lmMU0xN1dKU2JTY0s4YlpqKzdGd1haQWtFQXRKc1VuOVBRMHhxU1dQekZ4SWNXZGtDc3BOZVIweEl3Y2cvbXI0YU8razlLdTJVM2JxY0F3ZXg0TVJETkhRWndSdDJHa3BoUy9Dazl6Y2Z4SHVvYnZ3SkFMajlZeCtmYW80dlppUUhqVXcxdTYveFFrSW05ckQyN3d3SXdjTlVXb0ViMHg0Vk8wM2Q5NFRMMklsQ2hWd1lCd1FheGpaN09UQytWYmVpamZSQWd1UUpBZmpuVkZFK0tKOER1Zjl0S3JheHNlb0dCS0VTeDJuOWY5SmhBZG5UcEFQRGlIazhEaXdhVmRQTzhuUzVNN1BoUENIaFRVYW04ZGhpczVXSFVsL0t2S1FKQVNtVVpCTXZDWWdNdHJTSm41amtoT1N5T1lZMDBGUDhUT2xKR2tMTDF2RnpDazNib3lMbmZudEVwQ0xBSUFocnlubWVDWVc5TzdiOUJPcko3U2hMTE5RPT0=");

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
                        Base64String.Encode(addressContext)));

                options.IntegrationName = $"{nameof(SignServiceClientTests)}+{nameof(Can_create_private_key)}";
                options.AddressGeneratorFactory = (context) => addressGenerator.Object;
                options.TransactionSignerFactory = (context) => transactionSigner.Object;
            });

            //ACT
            var base64EncryptionKey = MyPublicKey;
            var request = new CreateAddressRequest(base64EncryptionKey);
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
                        Base64String.Encode(addressContext)));

                options.IntegrationName = $"{nameof(SignServiceClientTests)}+{nameof(Can_create_address_tag)}";
                options.AddressGeneratorFactory = (context) => addressGenerator.Object;
                options.TransactionSignerFactory = (context) => transactionSigner.Object;
            });

            //ACT
            var request = new CreateAddressTagRequest(Base64String.Encode(addressContext), AddressTagType.Text);
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

            var transactionId = new TransactionId("TransactionId");
            var signedTransaction = "From.x01.To.x02.Amount.100.Signature.F1T2A100";

            var client = PrepareClient<AppSettings>((options) =>
            {
                Mock<IAddressGenerator> addressGenerator = new Mock<IAddressGenerator>();
                Mock<ITransactionSigner> transactionSigner = new Mock<ITransactionSigner>();

                transactionSigner.Setup(x => x.SignAsync(It.IsAny<IReadOnlyCollection<string>>(), It.IsAny<Base64String>()))
                    .ReturnsAsync(new SignTransactionResponse(Base64String.Encode(signedTransaction), transactionId));

                options.IntegrationName = $"{nameof(SignServiceClientTests)}+{nameof(Can_sign_transaction)}";
                options.AddressGeneratorFactory = (context) => addressGenerator.Object;
                options.TransactionSignerFactory = (context) => transactionSigner.Object;
            });

            //ACT
            var request = new SignTransactionRequest(privateKeys, Base64String.Encode(signedTransaction));
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
                var addressGenerator = new Mock<IAddressGenerator>();
                var transactionSigner = new Mock<ITransactionSigner>();

                addressGenerator.Setup(x => x.CreateAddressAsync())
                    .ThrowsAsync(new OperationNotSupportedException("Address creation is not supported"));

                options.IntegrationName = $"{nameof(SignServiceClientTests)}+{nameof(Can_create_private_key)}";
                options.AddressGeneratorFactory = (context) => addressGenerator.Object;
                options.TransactionSignerFactory = (context) => transactionSigner.Object;
            });

            //ACT && ASSERT
            var base64EncryptionKey = MyPublicKey;
            var request = new CreateAddressRequest(base64EncryptionKey);

            Assert.ThrowsAsync<NotImplementedWebApiException>(async () =>
            {
                CreateAddressResponse result = await client.CreateAddressAsync(request);
                result?.PrivateKey?.DecryptToString(MyPrivateKey);
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
                var addressGenerator = new Mock<IAddressGenerator>();
                var transactionSigner = new Mock<ITransactionSigner>();

                addressGenerator.Setup(x => x.CreateAddressTagAsync(It.IsAny<string>(), It.IsAny<CreateAddressTagRequest>()))
                    .ThrowsAsync(new OperationNotSupportedException("Tag creation operation is not supported."));

                options.IntegrationName = $"{nameof(SignServiceClientTests)}+{nameof(Can_create_address_tag)}";
                options.AddressGeneratorFactory = (context) => addressGenerator.Object;
                options.TransactionSignerFactory = (context) => transactionSigner.Object;
            });

            //ACT && ASSERT

            Assert.ThrowsAsync<NotImplementedWebApiException>(async () =>
            {
                var request = new CreateAddressTagRequest(Base64String.Encode(addressContext), AddressTagType.Text);
                await client.CreateAddressTagAsync(address, request);
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
                var addressGenerator = new Mock<IAddressGenerator>();
                var transactionSigner = new Mock<ITransactionSigner>();

                addressGenerator.Setup(x =>
                        x.CreateAddressTagAsync(It.IsAny<string>(), It.IsAny<CreateAddressTagRequest>()))
                    .ReturnsAsync(() =>
                    {
                        Thread.Sleep(timeout);
                        return new CreateAddressTagResponse(new AddressTag(tag),
                            Base64String.Encode(addressContext));
                    });

                options.IntegrationName = $"{nameof(SignServiceClientTests)}+{nameof(Can_create_address_tag)}";
                options.AddressGeneratorFactory = (context) => addressGenerator.Object;
                options.TransactionSignerFactory = (context) => transactionSigner.Object;
            }, timeout);

            //ACT && ASSERT
            Assert.ThrowsAsync<TimeoutException>(async () =>
            {
                var request = new CreateAddressTagRequest(Base64String.Encode(addressContext), AddressTagType.Text);
                await client.CreateAddressTagAsync(address, request);
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
