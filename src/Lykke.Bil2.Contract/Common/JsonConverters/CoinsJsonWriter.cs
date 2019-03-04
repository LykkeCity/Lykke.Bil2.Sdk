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
            else if (value is CoinsValueBase coins)
            {
                writer.WriteValue(coins.StringValue);
            }
            else
            {
                throw new JsonSerializationException($"Expected {typeof(CoinsValueBase)} object value");
            }
        }
    }
}
