using System;
using Newtonsoft.Json;

namespace Lykke.Blockchains.Integrations.Contract.Common
{
    internal sealed class CoinsJsonReader
    {
        public TTarget ReadJson<TTarget>(JsonReader reader, Func<string, TTarget> factory)
            where TTarget : CoinsBase
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            if (reader.TokenType == JsonToken.String)
            {
                try
                {
                    var value = factory((string) reader.Value);
                    return value;
                }
                catch (CoinsConversionException ex)
                {
                    throw new RequestValidationException($"Failed to parse {typeof(TTarget)}", reader.Value, ex, reader.Path);
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException($"Error parsing {typeof(TTarget)}: [{reader.Value}] at path [{reader.Path}]", ex);
                }
            }

            throw new JsonSerializationException($"Unexpected token or value when parsing {typeof(TTarget)}. Token [{reader.TokenType}], value [{reader.Value}]");
        }
    }
}
