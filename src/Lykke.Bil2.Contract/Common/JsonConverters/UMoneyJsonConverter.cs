using System;
using JetBrains.Annotations;
using Lykke.Bil2.Contract.Common.Exceptions;
using Lykke.Numerics.Money;
using Newtonsoft.Json;

namespace Lykke.Bil2.Contract.Common.JsonConverters
{
    [PublicAPI]
    public class UMoneyJsonConverter : JsonConverter
    {
        public override void WriteJson(
            JsonWriter writer, 
            object value, 
            JsonSerializer serializer)
        {
            switch (value)
            {
                case null:
                    writer.WriteNull();
                    break;
                case UMoney uMoney:
                    writer.WriteValue(uMoney.ToString());
                    break;
                default:
                    throw new JsonSerializationException($"Expected {typeof(UMoney)} object value");
            }
        }

        public override object ReadJson(
            JsonReader reader,
            Type objectType,
            object existingValue,
            JsonSerializer serializer)
        {
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (reader.TokenType)
            {
                case JsonToken.Null:
                    return null;
                
                case JsonToken.String:
                    try
                    {
                        var value = UMoney.Parse((string) reader.Value);
                    
                        return value;
                    }
                    catch (FormatException ex)
                    {
                        throw new RequestValidationException($"Failed to parse {typeof(UMoney)}", reader.Value, ex, reader.Path);
                    }
                    catch (Exception ex)
                    {
                        throw new JsonSerializationException($"Error parsing {typeof(UMoney)}: [{reader.Value}] at path [{reader.Path}]", ex);
                    }
                    
                default:
                    throw new JsonSerializationException($"Unexpected token or value when parsing {typeof(UMoney)}. Token [{reader.TokenType}], value [{reader.Value}]");
            }
        }

        public override bool CanConvert(
            Type objectType)
        {
            return objectType == typeof(UMoney)
                || objectType == typeof(UMoney?);
        }
    }
}
