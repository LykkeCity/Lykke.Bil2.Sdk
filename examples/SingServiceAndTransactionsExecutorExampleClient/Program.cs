using System;
using System.Threading.Tasks;
using Lykke.Bil2.Client.SignService;
using Lykke.Bil2.Client.SignService.Services;
using Lykke.Bil2.Client.TransactionsExecutor;
using Lykke.Bil2.Client.TransactionsExecutor.Services;
using Lykke.Bil2.Contract.Common.Extensions;
using Lykke.Bil2.Contract.SignService.Requests;
using Lykke.Bil2.Contract.TransactionsExecutor.Requests;
using Lykke.Bil2.SharedDomain;
using Lykke.Common.Log;
using Lykke.Logs;
using Lykke.Logs.Loggers.LykkeConsole;
using Lykke.Numerics;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace SingServiceAndTransactionsExecutorExampleClient
{
    internal sealed class Program
    {
        private static readonly Base58String MyPublicKey = new Base58String("Vj75CuZgqYqhewfDfF9KQEdejjqbnoDiRjuXUnwCo2jkLo8AJpqBF6jFovufKrvwqUaubTRrAwr3wBBHtFVWhxhrxwFMoeB3mrBXnreVkfRdL1L9NUpyn4qDTB1Hwm3kBjmnhdVm2ZxmZ696FKj6yeBMnPB7Lkoa4XxKg6a9TwPCfgUbNV9b8dCPXm1YdYMdFK9Hf8sestxpp6FphTxbrjnicpeiU");

        private static readonly Base58String MyPrivateKey = new Base58String("hLLPYvqqGdBtPBMGWp98NbAHtDHuoj4vnrQqNPuMBMZpRFCX2kVtbC9HgrMQthM6NzEDjQw6c88PfiRCqzegb2KpTnBN1hqLyQKfT8FuGVDGRBJEYFZbgUbbb5e5PZZVnRwabyy4Rup8c5Yb5fWJ1Lw73yUtcxvmx52ymwm8m9j3NtVRTkcb5NsXGbMdanUpa9MdAMHReFDDh1qWx8N6zTb4st4WbaTuTamhcTEueuPKosaum5TLv2udcUDj2JtaZYUhDJ6EvotsHScwLQnwcW4ZkEydSCJgqefYp4n7qgNpzNKU2ffsDmPuQDfVBLNiFkqv99TMYsvrYbemcLLRoEUiqvGTaGZBpHErd5quYq1G9187nbySGy3aJCMVqVWzApuYKewUQDRND1fx6kybQpuDy6CjBp4i5sdAub4eFNHjRwVVUUsYQjjhy8PQpES5AWszv2DgMyrqEaAGYxr3akK6HmbYyBquWriAdYjp9v8NW29yhpSBXwyGpCT5j54KA2F5yaxv6MfUiucGHxYdgNFdkhSZYRB3FgZJQzTAy6utSZTapjcBmu25nzQcdaevnSW4rMd9kuhMGJL6PtyG6niHn48bsmwxSukGZsMvfRb8UDf3ZnGnNFHXChADzywM7ghZBtohBRgbnAyduUQ7GbLv6vhuyXV7qxAkvAwuz4mipe8MbTem6cjhKJrV1Sa4Ya6pUnWR7BhqNjWEKuVbCt8D9Et2no6u8hXxyxV6kD5tKEJ932PVHYBmnLQV9jCfS2RRv2BhBjhA6bUp8zCSuW2CyVYPCgmqQCa4EahA6iiHhymdaPQ8YGChbH8san");

        private const string IntegrationName = "SampleIntegration";

        // ReSharper disable once UnusedParameter.Global
        public static async Task Main(string[] args)
        {
            Console.WriteLine("Creating clients...");

            var services = new ServiceCollection();

            services.AddSingleton(s => LogFactory.Create().AddUnbufferedConsole());
            services.AddSignServiceClient(options =>
            {
                options.AddIntegration(
                    IntegrationName,
                    integrationOptions =>
                    {
                        integrationOptions.Url = "http://localhost:5000";
                    });
            });
            services.AddTransactionsExecutorClient(options =>
            {
                options.AddIntegration(
                    IntegrationName,
                    integrationOptions =>
                    {
                        integrationOptions.Url = "http://localhost:5001";
                    });
            });

            using (var serviceProvider = services.BuildServiceProvider())
            {
                var log = serviceProvider.GetRequiredService<ILogFactory>().CreateLog(typeof(Program));
                var signServiceApiProvider = serviceProvider.GetRequiredService<ISignServiceApiProvider>();
                var signServiceApi = signServiceApiProvider.GetApi(IntegrationName);
                var transactionsExecutorApiProvider = serviceProvider.GetRequiredService<ITransactionsExecutorApiProvider>();
                var transactionsExecutorApi = transactionsExecutorApiProvider.GetApi(IntegrationName);

                Console.WriteLine("Press any key to start.");

                Console.ReadKey(true);

                var tests = new Func<Task<object>>[]
                {
                    () => GetIsAliveAsync(signServiceApi),
                    () => GetIsAliveAsync(transactionsExecutorApi),
                    () => CreateAddressAsync(signServiceApi),
                    () => CreateAddressTagAsync(signServiceApi),
                    () => BuildTransactionAsync(transactionsExecutorApi),
                    () => BuildInvalidTransactionAsync(transactionsExecutorApi),
                    () => SignTransactionAsync(signServiceApi),
                    () => BroadcastTransactionAsync(transactionsExecutorApi)
                };

                foreach (var step in tests)
                {
                    try
                    {
                        var response = await step.Invoke();

                        if (response != null)
                        {
                            Console.WriteLine("Response:");

                            var jsonResponse = JsonConvert.SerializeObject(response, Formatting.Indented);

                            Console.ForegroundColor = ConsoleColor.DarkGreen;
                            Console.WriteLine(jsonResponse);
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.DarkGreen;
                            Console.WriteLine("Response is empty");
                        }

                        Console.ForegroundColor = ConsoleColor.Gray;
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex);
                    }
                }
            }

            Console.WriteLine("Press any key to exit.");

            Console.ReadKey(true);
        }

        private static async Task<object> GetIsAliveAsync(ISignServiceApi client)
        {
            Console.WriteLine("Getting isalive of the sign service...");

            return await client.GetIsAliveAsync();
        }

        private static async Task<object> GetIsAliveAsync(ITransactionsExecutorApi client)
        {
            Console.WriteLine("Getting isalive of the transactions executor...");

            return await client.GetIsAliveAsync();
        }

        private static async Task<object> CreateAddressAsync(ISignServiceApi client)
        {
            Console.WriteLine("Creating address...");

            var response = await client.CreateAddressAsync(new CreateAddressRequest(MyPublicKey));

            Console.WriteLine($"Decrypted private key: {response.PrivateKey.DecryptToString(MyPrivateKey)}");

            return response;
        }

        private static async Task<object> CreateAddressTagAsync(ISignServiceApi client)
        {
            Console.WriteLine("Creating address tag...");

            return await client.CreateAddressTagAsync
            (
                "Test:c021d892538b4a7a8520ae46f368c00f",
                new CreateAddressTagRequest
                (
                    new Base58String("2HQaV1"),
                    AddressTagType.Number
                )
            );
        }

        private static async Task<object> BuildTransactionAsync(ITransactionsExecutorApi client)
        {
            Console.WriteLine("Building the transaction...");

            return await client.BuildTransferAmountTransactionAsync
            (
                new BuildTransferAmountTransactionRequest
                (
                    new[]
                    {
                        new Transfer
                        ( 
                            new Asset( "STEEM"),
                            UMoney.Create(100, 3),
                            "Test:c021d892538b4a7a8520ae46f368c00f",
                            "Test:0662c0c7b9954373a5803fab41d97774"
                        )
                    },
                    new []
                    {
                        new Fee(new Asset( "STEEM"), UMoney.Create(0.001m, 3))
                    }
                )
            );
        }

        private static async Task<object> BuildInvalidTransactionAsync(ITransactionsExecutorApi client)
        {
            Console.WriteLine("Building the invalid transaction...");

            return await client.BuildTransferAmountTransactionAsync
            (
                new BuildTransferAmountTransactionRequest
                (
                    new[]
                    {
                        new Transfer
                        ( 
                            new Asset( "STEEM"),
                            UMoney.Create(100, 3),
                            "Test:c021d892538b4a7a8520ae46f368c00f",
                            "Test:0662c0c7b9954373a5803fab41d97774"
                        ),
                        new Transfer
                        ( 
                            new Asset( "STEEM"),
                            UMoney.Create(50, 3),
                            "Test:c021d892538b4a7a8520ae46f368c00f",
                            "Test:0662c0c7b9954373a5803fab41d97774"
                        )
                    },
                    new []
                    {
                        new Fee(new Asset( "STEEM"), UMoney.Create(0.001m, 3))
                    }
                )
            );
        }

        private static async Task<object> SignTransactionAsync(ISignServiceApi client)
        {
            Console.WriteLine("Signing the transaction...");

            return await client.SignTransactionAsync
            (
                new SignTransactionRequest
                (
                    new[]
                    {
                        "ab8b89f8".Encrypt(MyPublicKey),

                    },
                    new Base58String("8DwvGw29pzwXkL4AzkpPr94qEYf77hznJvfYKN4NMxxaDAwUFXunY2eQqaL3pji4e2PaA9Cyxf7NKjTQJbzKs2W4vvaqit48PoLpk6vGyexkACnd77CCpjf7xqdBnkYG151JexnrmYHhF8FNk5bswDnxKS4vzw4XKEf99rYqH9orrNzicwZPrBxNM6jYodXjyPGocfH4reVYJS84iBvRXRMMZbhciVG5qRTdwDU2kiUa3XfDzvvqYCiQxaLfdpuiBb969YCME3p7BpS4BBaAt6c4amKykb4GDQt3W8tMbb725gF4FCvysTn49t2iyAdfP7iYiqoVwubF9SQZCJGwsoVKnvVXRMQyA7gy1iYbKfQfFeMVQyXYB3SJsZZi3hsoiGKuaEtWg7yzVXihHo3iF81kmkD1rYcENjWuXoiFMv6yZotTsQ5FQkwAQGZjdQi4QDa")
                )
            );
        }

        private static async Task<object> BroadcastTransactionAsync(ITransactionsExecutorApi client)
        {
            Console.WriteLine("Broadcasting the transaction...");

            await client.BroadcastTransactionAsync
            (
                new BroadcastTransactionRequest
                (
                    new Base58String("24XMnrEWxpTKMEezkUvwmmtFW1Wivg48WfY7PKXMTfoaL8LetbYUpSXJ6ucskXjrpHUfH5cpuxL4ufbt4C2kNcFHe3JyBFPYXm8TMjNnVqB3gPAXADvvCRe5kguu757Cdm5xaVPb75TNqMhksUJkis6YrV2Q8sFfLoxFyvuhoPXmU3NS2n9j3kj8j3bTSonQocGDYQinWPg9AYk7vDL152GMVQ2w4BYSkMZ6ZnhGWZe6GErwrqs7BxeGqjyGWXTb5ToRJMVEJ1rcfDUMX4a76e6WMY18RDeh92Pws5byP26NkCxLV8NfPcyxr4aQZ1Uk2zGVnNUZa5b7DwmJm3tEy1k6q5S63FFd247ngF4oKJYyWLhtkELRuE227ec4bkv5XNg6VsLNe8Fqak1W3ZBz8Y4dMmwtFuJCic3c9YSesr5GvTQsVhpvmNycdfDvNgEQL7Qk1LVBz2dBDMWHkAWCxXMpE6kuCfzFcu4Mbz6svk4Y38XYjB3y3Yr776wDSWbZ6sjZwCkmbZW1nKczQSdNYn2WugBm2eM4qvCNXjLruLdXRtrbLqfbKwq3pTV2Rnz7te8tDR8WkFEnsnZMF2DUJFBKUc12jXvZ7eDJNdoSzejPHM7nXznGKAxTm48YfNgEepMs2LLVjZKKBRFDFncTXkpPWKsbU7AorwqZTJFjS9URr")
                )
            );

            return null;
        }
    }
}
