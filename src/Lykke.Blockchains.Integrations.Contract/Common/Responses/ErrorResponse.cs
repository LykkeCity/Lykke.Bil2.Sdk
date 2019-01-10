using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Lykke.Blockchains.Integrations.Contract.Common.Responses
{
    /// <inheritdoc />
    /// <typeparam name="TErrorCode">Type of the error code. Should be enum</typeparam>
    [PublicAPI]
    public class ErrorResponse<TErrorCode> : ErrorResponse
    {
        /// <summary>
        /// Error code
        /// </summary>
        [JsonProperty("errorCode")]
        [JsonConverter(typeof(StringEnumConverter), typeof(CamelCaseNamingStrategy), new object[0], false)]
        public TErrorCode ErrorCode { get; set; }
    }

    /// <summary>
    /// General API error response
    /// </summary>
    [PublicAPI]
    public class ErrorResponse
    {
        /// <summary>
        /// Optional.
        /// Summary error message
        /// </summary>
        [CanBeNull]
        [JsonProperty("errorCode")]
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Optional.
        /// Model errors. Key is the model field name, value is the list of the errors related to the given model field.
        /// </summary>
        [CanBeNull]
        [JsonProperty("modelErrors")]
        public Dictionary<string, List<string>> ModelErrors { get; set; }

        /// <summary>
        /// Creates <see cref="ErrorResponse"/> with summary error message
        /// </summary>
        /// <param name="message">Summary error message</param>
        public static ErrorResponse Create(string message)
        {
            return new ErrorResponse
            {
                ErrorMessage = message
            };
        }

        /// <summary>
        /// Creates <see cref="ErrorResponse{TErrorCode}"/> with specific error code and optional summary error message
        /// </summary>
        /// <param name="errorCode">Error code</param>
        /// <param name="message">Summary error message</param>
        /// <typeparam name="TErrorCode">Type of the error code. Should be enum</typeparam>
        public static ErrorResponse<TErrorCode> Create<TErrorCode>(TErrorCode errorCode, string message = null)
        {
            return new ErrorResponse<TErrorCode>
            {
                ErrorCode = errorCode,
                ErrorMessage = message
            };
        }

        /// <summary>
        /// Adds model error to the current <see cref="ErrorResponse"/> instance
        /// </summary>
        /// <param name="key">Model field name</param>
        /// <param name="message">Error related to the given model field</param>
        /// <returns></returns>
        public ErrorResponse AddModelError(string key, string message)
        {
            if (ModelErrors == null)
            {
                ModelErrors = new Dictionary<string, List<string>>();
            }

            if (!ModelErrors.TryGetValue(key, out var errors))
            {
                errors = new List<string>();

                ModelErrors.Add(key, errors);
            }

            errors.Add(message);

            return this;
        }

        /// <summary>
        /// Adds model error to the current <see cref="ErrorResponse"/> instance
        /// </summary>
        /// <param name="key">Model field name</param>
        /// <param name="exception">Exception which corresponds to the error related to the given model field</param>
        public ErrorResponse AddModelError(string key, Exception exception)
        {
            return AddModelError(key, exception.ToString());
        }

        public string GetSummaryMessage()
        {
            var sb = new StringBuilder();

            if (ErrorMessage != null)
            {
                sb.AppendLine($"Error summary: {ErrorMessage}");
            }

            if (ModelErrors == null) 
                return sb.ToString();
            
            sb.AppendLine();

            foreach (var error in ModelErrors)
            {
                if (error.Key == null || error.Value == null || error.Value.Count == 0) 
                    continue;
                
                if (!string.IsNullOrWhiteSpace(error.Key))
                {
                    sb.AppendLine($"{error.Key}:");
                }

                foreach (var message in error.Value.Take(error.Value.Count - 1))
                {
                    sb.AppendLine($" - {message}");
                }

                sb.Append($" - {error.Value.Last()}");
            }

            return sb.ToString();
        }
    }

}
