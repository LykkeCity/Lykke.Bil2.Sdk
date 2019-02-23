using System;
using System.Threading.Tasks;
using Lykke.Bil2.Contract.Common;
using Lykke.Bil2.Contract.TransactionsExecutor;
using Lykke.Bil2.Contract.TransactionsExecutor.Requests;
using Lykke.Bil2.Contract.TransactionsExecutor.Responses;
using Lykke.Bil2.Sdk.Exceptions;
using Lykke.Bil2.Sdk.TransactionsExecutor.Exceptions;
using Lykke.Bil2.Sdk.TransactionsExecutor.Services;
using Newtonsoft.Json;

namespace TransactionsExecutorExample.Services
{
    public class TransactionsExecutor : ITransactionExecutor
    {
        private readonly Random _random;

        public TransactionsExecutor()
        {
            _random = new Random();
        }

        public Task<BuildSendingTransactionResponse> BuildSendingAsync(BuildSendingTransactionRequest request)
        {
            var entropy = _random.Next(0, 100);

            if (entropy < 10)
            {
                throw new SendingTransactionBuildingException(SendingTransactionBuildingError.RetryLater, "Node is too busy");
            }

            if (request.Transfers.Count > 1)
            {
                throw new RequestValidationException("Only single input is supported", request.Transfers.Count, nameof(request.Transfers.Count));
            }

            var context = JsonConvert.SerializeObject(request).ToBase58();

            return Task.FromResult(new BuildSendingTransactionResponse(context));
        }

        public Task<BuildReceivingTransactionResponse> BuildReceivingAsync(BuildReceivingTransactionRequest request)
        {
            throw new OperationNotSupportedException("Receiving transactions are not supported");
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

            BuildSendingTransactionRequest context;

            try
            {
                context = JsonConvert.DeserializeObject<BuildSendingTransactionRequest>(serializedContext);
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
