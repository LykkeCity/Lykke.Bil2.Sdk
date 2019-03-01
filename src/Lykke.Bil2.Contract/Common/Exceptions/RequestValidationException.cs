using System;
using System.Linq;
using System.Text;
using JetBrains.Annotations;

namespace Lykke.Bil2.Contract.Common.Exceptions
{
    /// <summary>
    /// Blockchain integration incoming request validation exception.
    /// </summary>
    [PublicAPI]
    public class RequestValidationException : Exception
    {
        public RequestValidationException(string message, object actualValue, [InvokerParameterName] string parameterName) :
            base(BuildMessage(message, actualValue, parameterName))
        {
        }

        public RequestValidationException(string message, object actualValue, Exception inner, [InvokerParameterName] string parameterName) :
            base(BuildMessage(message, actualValue, parameterName), inner)
        {
        }

        public RequestValidationException(string message, [InvokerParameterName] string parameterName) :
            base(BuildMessage(message, parameterName))
        {
        }

        public RequestValidationException(string message, Exception inner, [InvokerParameterName] string parameterName) :
            base(BuildMessage(message, parameterName), inner)
        {
        }

        public RequestValidationException(string message, string[] parameterNames) :
            base(BuildMessage(message, parameterNames))
        {
        }

        public RequestValidationException(string message, Exception inner, string[] parameterNames) :
            base(BuildMessage(message, parameterNames), inner)
        {
        }

        public RequestValidationException(string message, Exception inner) :
            base(message, inner)
        {
        }

        public RequestValidationException(string message) :
            base(message)
        {
        }

        public static RequestValidationException ShouldBeNotEmptyString([InvokerParameterName] string parameterName)
        {
            return new RequestValidationException("Should be not empty string", parameterName);
        }

        public static RequestValidationException ShouldBeNotEmptyCollection([InvokerParameterName] string parameterName)
        {
            return new RequestValidationException("Should be not empty collection", parameterName);
        }

        public static RequestValidationException ShouldBeNotNull([InvokerParameterName] string parameterName)
        {
            return new RequestValidationException("Should be not null", parameterName);
        }

        public static RequestValidationException ShouldBePositiveNumber(object actualValue, [InvokerParameterName] string parameterName)
        {
            return new RequestValidationException("Should be positive number", actualValue, parameterName);
        }

        public static Exception ShouldBeZeroOrPositiveNumber(object actualValue, string parameterName)
        {
            return new RequestValidationException("Should be zero or positive number", actualValue, parameterName);
        }

        private static string BuildMessage(string message, object actualValue, string parameterName)
        {
            var sb = new StringBuilder();

            sb.AppendLine(message);
            sb.AppendLine($"Parameter: {parameterName}");
            sb.AppendLine($"Actual value: {actualValue}");

            return sb.ToString();
        }

        private static string BuildMessage(string message, string parameterName)
        {
            var sb = new StringBuilder();

            sb.AppendLine(message);
            sb.AppendLine($"Parameter: {parameterName}");

            return sb.ToString();
        }

        private static string BuildMessage(string message, string[] parameterNames)
        {
            var sb = new StringBuilder();

            sb.AppendLine(message);

            if (parameterNames != null && parameterNames.Any())
            {
                sb.Append($"Parameters: {parameterNames.First()}");

                foreach (var parameterName in parameterNames.Skip(1))
                {
                    sb.Append($", {parameterName}");
                }
            }

            return sb.ToString();
        }
    }
}
