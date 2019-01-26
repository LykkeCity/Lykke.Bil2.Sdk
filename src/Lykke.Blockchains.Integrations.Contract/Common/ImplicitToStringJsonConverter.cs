using System;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Lykke.Blockchains.Integrations.Contract.Common
{
    [PublicAPI]
    public class ImplicitToStringJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
            }
            else
            {
                writer.WriteValue(value.ToString());
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
                    var value = Activator.CreateInstance(objectType, reader.Value);
                    return value;
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
            return objectType.IsClass && 
                   !objectType.IsAbstract && 
                   !objectType.IsGenericType && 
                   objectType
                       .GetConstructors(BindingFlags.Public)
                       .Count(c => c.GetParameters().Length == 1 &&
                                   c.GetParameters().Count(p => p.ParameterType == typeof(string) && p.IsIn) == 1) == 1;
        }
    }
}
