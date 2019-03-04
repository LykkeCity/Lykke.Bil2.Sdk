﻿using System;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Lykke.Bil2.Contract.Common.JsonConverters
{
    [PublicAPI]
    public class CoinsChangeJsonConverter : JsonConverter
    {
        private CoinsJsonWriter _writer;
        private CoinsJsonReader _reader;

        public CoinsChangeJsonConverter()
        {
            _writer = new CoinsJsonWriter();
            _reader = new CoinsJsonReader();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            _writer.WriteJson(writer, value);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            return _reader.ReadJson(reader, CoinsChange.Parse);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(CoinsChange);
        }
    }
}
