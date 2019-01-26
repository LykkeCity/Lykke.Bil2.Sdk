using System;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Lykke.Blockchains.Integrations.Contract.Common
{
    [PublicAPI]
    public class Base58StringJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
            }
            else if (value is Base58String base58String)
            {
                writer.WriteValue(base58String.Value);
            }
            else
            {
                throw new JsonSerializationException("Expected Base58String object value");
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            if (reader.TokenType == JsonToken.String)
            {
                try
                {
                    var value = Base58String.Create((string) reader.Value);
                    return value;
                }
                catch (Base58StringConversionException ex)
                {
                    throw new RequestValidationException("Failed to parse Base58String", reader.Value, ex, reader.Path);
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException($"Error parsing Base58String: [{reader.Value}] at path [{reader.Path}]", ex);
                }
            }

            throw new JsonSerializationException($"Unexpected token or value when parsing Base58String. Token [{reader.TokenType}], value [{reader.Value}]");
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Base58String);
        }
    }
}
