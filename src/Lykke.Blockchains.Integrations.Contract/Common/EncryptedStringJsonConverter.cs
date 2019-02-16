using System;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Lykke.Blockchains.Integrations.Contract.Common
{
    [PublicAPI]
    public class EncryptedStringJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
            }
            else if (value is EncryptedString encryptedString)
            {
                writer.WriteValue(encryptedString.EncryptedValue.Value);
            }
            else
            {
                throw new JsonSerializationException("Expected EncryptedString object value");
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
                    var value = new EncryptedString(new Base58String((string) reader.Value));
                    return value;
                }
                catch (Base58StringConversionException ex)
                {
                    throw new RequestValidationException("Failed to parse Encryptedstring as Base58String", reader.Value, ex, reader.Path);
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException($"Error parsing EncryptedString: [{reader.Value}] at path [{reader.Path}]", ex);
                }
            }

            throw new JsonSerializationException($"Unexpected token or value when parsing EncryptedString. Token [{reader.TokenType}], value [{reader.Value}]");
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(EncryptedString);
        }
    }
}
