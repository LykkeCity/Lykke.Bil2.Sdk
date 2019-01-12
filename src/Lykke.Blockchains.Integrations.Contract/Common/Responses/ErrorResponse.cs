﻿using System;
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
        [JsonProperty("code")]
        [JsonConverter(typeof(StringEnumConverter), typeof(CamelCaseNamingStrategy), new object[0], false)]
        public TErrorCode Code { get; }

        public ErrorResponse(TErrorCode code, string message, IDictionary<string, ICollection<string>> details) :
            base(message, details)
        {
            Code = code;
        }
    }

    /// <summary>
    /// General API error response
    /// </summary>
    [PublicAPI]
    public class ErrorResponse
    {
        /// <summary>
        /// Summary error message clear for humans.
        /// </summary>
        [JsonProperty("message")]
        public string Message { get; }

        /// <summary>
        /// Optional.
        /// Details. Key is the request parameter name, value is the list of the errors related to the given parameter.
        /// </summary>
        [CanBeNull]
        [JsonProperty("details")]
        public IDictionary<string, ICollection<string>> Details { get; private set; }

        public ErrorResponse(string message, IDictionary<string, ICollection<string>> details)
        {
            Message = message;
            Details = details;
        }

        /// <summary>
        /// Creates <see cref="ErrorResponse"/> with summary error message
        /// </summary>
        /// <param name="message">Summary error message</param>
        public static ErrorResponse Create(string message)
        {
            return new ErrorResponse(message, null);
        }

        /// <summary>
        /// Creates <see cref="ErrorResponse"/> with summary error message
        /// </summary>
        public static ErrorResponse Create(Exception exception)
        {
            return new ErrorResponse(exception.ToString(), null);
        }

        /// <summary>
        /// Creates <see cref="ErrorResponse{TErrorCode}"/> with specific error code and optional summary error message
        /// </summary>
        /// <param name="errorCode">Error code</param>
        /// <param name="message">Summary error message</param>
        /// <typeparam name="TErrorCode">Type of the error code. Should be enum</typeparam>
        public static ErrorResponse<TErrorCode> CreateFromCode<TErrorCode>(TErrorCode errorCode, string message = null)
        {
            return new ErrorResponse<TErrorCode>(errorCode, message, null);
        }

        /// <summary>
        /// Adds request parameter detail error to the current <see cref="ErrorResponse"/> instance
        /// </summary>
        /// <param name="key">Request parameter name</param>
        /// <param name="message">Error related to the given parameter</param>
        /// <returns></returns>
        public ErrorResponse AddDetail(string key, string message)
        {
            if (Details == null)
            {
                Details = new Dictionary<string, ICollection<string>>();
            }

            if (!Details.TryGetValue(key, out var errors))
            {
                errors = new List<string>();

                Details.Add(key, errors);
            }

            errors.Add(message);

            return this;
        }

        /// <summary>
        /// Adds request parameter detail error to the current <see cref="ErrorResponse"/> instance
        /// </summary>
        /// <param name="key">MRequest parameter name</param>
        /// <param name="exception">Exception which corresponds to the error related to the given parameter</param>
        public ErrorResponse AddDetail(string key, Exception exception)
        {
            return AddDetail(key, exception.ToString());
        }

        public string GetSummaryMessage()
        {
            var sb = new StringBuilder();

            if (Message != null)
            {
                sb.AppendLine($"Error summary: {Message}");
            }

            if (Details == null) 
                return sb.ToString();
            
            sb.AppendLine();

            foreach (var error in Details)
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
