using System.ComponentModel;

namespace Lykke.Bil2.RabbitMq.MessagePack
{
    internal static class TypeConverterExtensions
    {
        public static bool CanConvertFromAndTo<T>(
            this TypeConverter converter)
        {
            var type = typeof(T);

            return converter.CanConvertFrom(type)
                && converter.CanConvertTo(type);
        }
    }
}
