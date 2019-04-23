using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using MessagePack;
using MessagePack.Formatters;
using MessagePack.Resolvers;

namespace Lykke.Bil2.RabbitMq.MessagePack
{
    internal class CompositeFormatterResolver : ICompositeFormatterResolver
    {
        private readonly ConcurrentDictionary<Type, IMessagePackFormatter> _formattersCache;
        private readonly List<IFormatterResolver> _resolvers;
        
        public CompositeFormatterResolver()
        {
            _formattersCache = new ConcurrentDictionary<Type, IMessagePackFormatter>();
            _resolvers = new List<IFormatterResolver>();
        }

        private IEnumerable<IFormatterResolver> Resolvers
        {
            get
            {
                foreach (var resolver in _resolvers)
                {
                    yield return resolver;
                }

                yield return StandardResolver.Instance;

                yield return TypeConverterFormatterResolver.Instance;
            }
        }

        public IMessagePackFormatter<T> GetFormatter<T>()
        {
            var requestedType = typeof(T);

            return (IMessagePackFormatter<T>) _formattersCache.GetOrAdd(requestedType, type =>
            {
                return Resolvers
                    .Select(resolver => resolver.GetFormatter<T>())
                    .FirstOrDefault(formatter => formatter != null);
            });
        }

        public void RegisterResolvers(
            params IFormatterResolver[] resolvers)
        {
            _resolvers.AddRange(resolvers.Where(x => !_resolvers.Contains(x)));
        }
    }
}
