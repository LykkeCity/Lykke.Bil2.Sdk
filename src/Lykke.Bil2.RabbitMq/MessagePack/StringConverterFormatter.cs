using System.ComponentModel;
using System.Globalization;
using MessagePack;
using MessagePack.Formatters;

namespace Lykke.Bil2.RabbitMq.MessagePack
{
    public class StringConverterFormatter<T> : IMessagePackFormatter<T>
    {
        private readonly TypeConverter _converter;
        
        
        public StringConverterFormatter(
            TypeConverter converter)
        {
            _converter = converter;
        }
        
        
        public int Serialize(
            ref byte[] bytes,
            int offset,
            T value,
            IFormatterResolver formatterResolver)
        {
            var convertedValue = (string) _converter.ConvertTo(default, CultureInfo.InvariantCulture, value, typeof(string));

            return NullableStringFormatter.Instance.Serialize(ref bytes, offset, convertedValue, formatterResolver);
        }

        public T Deserialize(
            byte[] bytes,
            int offset, 
            IFormatterResolver formatterResolver, 
            out int readSize)
        {
            var convertedValue = NullableStringFormatter.Instance.Deserialize(bytes, offset, formatterResolver, out readSize);

            // ReSharper disable once AssignNullToNotNullAttribute
            return (T) _converter.ConvertFrom(default, CultureInfo.InvariantCulture, convertedValue);
        }
    }
}
