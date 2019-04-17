using MessagePack;

namespace Lykke.Bil2.RabbitMq.MessagePack
{
    public interface ICompositeFormatterResolver : IFormatterResolver
    {
        void RegisterResolvers(
            params IFormatterResolver[] resolvers);
    }
}
