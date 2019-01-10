using System;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Lykke.Blockchains.Integrations.Contract.Common
{
    [PublicAPI]
    public class CoinsAmountJsonConverter : JsonConverter
    {
        private CoinsJsonWriter _writer;
        private StringValueJsonReader _reader;

        public CoinsAmountJsonConverter()
        {
            _writer = new CoinsJsonWriter();
            _reader = new StringValueJsonReader();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            _writer.WriteJson(writer, value);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            return _reader.ReadJson(reader, CoinsAmount.Create);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(CoinsAmount);
        }
    }
}
