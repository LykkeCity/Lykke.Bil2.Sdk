using System;
using System.Text;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Lykke.Bil2.Contract.Common.Responses
{
    /// <inheritdoc />
    /// <typeparam name="TErrorCode">Type of the error code. Should be enum</typeparam>
    [PublicAPI]
    public class BlockchainErrorResponse<TErrorCode> : BlockchainErrorResponse
        where TErrorCode : Enum
    {
        /// <summary>
        /// Error code
        /// </summary>
        [JsonProperty("code")]
        public TErrorCode Code { get; }

        /// <inheritdoc />
        public BlockchainErrorResponse(TErrorCode code, string message) :
            base(message)
        {
            Code = code;
        }
    }

    /// <summary>
    /// General API error response
    /// </summary>
    [PublicAPI]
    public class BlockchainErrorResponse
    {
        /// <summary>
        /// Summary error message clear for humans.
        /// </summary>
        [JsonProperty("message")]
        public string Message { get; }

        /// <summary>
        /// General API error response
        /// </summary>
        public BlockchainErrorResponse(string message)
        {
            Message = message;
        }

        /// <summary>
        /// Creates <see cref="BlockchainErrorResponse"/> with summary error message
        /// </summary>
        /// <param name="message">Summary error message</param>
        public static BlockchainErrorResponse Create(string message)
        {
            return new BlockchainErrorResponse(message);
        }

        /// <summary>
        /// Creates <see cref="BlockchainErrorResponse"/> with detailed (including call stack) summary error message
        /// </summary>
        public static BlockchainErrorResponse Create(Exception exception)
        {
            return new BlockchainErrorResponse(exception.ToString());
        }

        /// <summary>
        /// Creates <see cref="BlockchainErrorResponse"/> with brief (without call stack) summary error message
        /// </summary>
        public static BlockchainErrorResponse CreateBrief(Exception exception)
        {
            var ex = exception;
            var sb = new StringBuilder();

            while (true)
            {
                if (ex.InnerException != null)
                {
                    sb.AppendLine(ex.Message);
                }
                else
                {
                    sb.Append(ex.Message);
                }

                ex = ex.InnerException;

                if (ex == null)
                {
                    return new BlockchainErrorResponse(sb.ToString());
                }

                sb.Append(" -> ");
            }
        }

        /// <summary>
        /// Creates <see cref="BlockchainErrorResponse{TErrorCode}"/> with specific error code and optional summary error message
        /// </summary>
        /// <param name="errorCode">Error code</param>
        /// <param name="message">Summary error message</param>
        /// <typeparam name="TErrorCode">Type of the error code. Should be enum</typeparam>
        public static BlockchainErrorResponse<TErrorCode> CreateFromCode<TErrorCode>(TErrorCode errorCode, string message = null)
            where TErrorCode : Enum
        {
            return new BlockchainErrorResponse<TErrorCode>(errorCode, message);
        }
    }
}
