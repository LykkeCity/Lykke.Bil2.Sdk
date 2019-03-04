using System;
using System.Threading.Tasks;
using Lykke.Bil2.Contract.Common;
using Lykke.Bil2.Contract.Common.Exceptions;
using Lykke.Bil2.Contract.TransactionsExecutor;
using Lykke.Bil2.Contract.TransactionsExecutor.Requests;
using Lykke.Bil2.Sdk.TransactionsExecutor.Exceptions;
using Lykke.Bil2.Sdk.TransactionsExecutor.Services;
using Newtonsoft.Json;

namespace TransactionsExecutorExample.Services
{
    public class TransactionsBroadcaster : ITransactionBroadcaster
    {
        private readonly Random _random;

        public TransactionsBroadcaster()
        {
            _random = new Random();
        }

        public Task BroadcastAsync(BroadcastTransactionRequest request)
        {
            var entropy = _random.Next(0, 100);
            dynamic signed;

            try
            {
                signed = JsonConvert.DeserializeObject<dynamic>(request.SignedTransaction.DecodeToString());
            }
            catch (JsonException ex)
            {
                throw new RequestValidationException("Failed to deserialize signed transaction", request.SignedTransaction, ex, nameof(request.SignedTransaction));
            }

            var serializedContext = new Base58String((string)signed.Context).DecodeToString();

            if (string.IsNullOrWhiteSpace(serializedContext))
            {
                throw new RequestValidationException("Context is empty in the signed transaction");
            }

            BuildTransferAmountTransactionRequest context;

            try
            {
                context = JsonConvert.DeserializeObject<BuildTransferAmountTransactionRequest>(serializedContext);
            }
            catch (JsonException ex)
            {
                throw new RequestValidationException("Failed to deserialize signed transaction context", serializedContext, ex, nameof(serializedContext));
            }

            if (context.Expiration?.AfterMoment < DateTime.UtcNow)
            {
                throw new TransactionBroadcastingException(TransactionBroadcastingError.RebuildRequired, "Transaction is expired");
            }

            if (entropy < 10)
            {
                throw new TransactionBroadcastingException(TransactionBroadcastingError.TransientFailure, "Nod is not available");
            }

            return Task.CompletedTask;
        }
    }
}
