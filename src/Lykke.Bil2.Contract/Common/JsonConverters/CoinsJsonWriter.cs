using Newtonsoft.Json;

namespace Lykke.Bil2.Contract.Common.JsonConverters
{
    internal sealed class CoinsJsonWriter
    {
        public void WriteJson(JsonWriter writer, object value)
        {
            if (value == null)
            {
                writer.WriteNull();
            }
            else if (value is CoinsBase coins)
            {
                writer.WriteValue(coins.StringValue);
            }
            else
            {
                throw new JsonSerializationException($"Expected {typeof(CoinsBase)} object value");
            }
        }
    }
}
