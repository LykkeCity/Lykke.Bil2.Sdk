using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using MessagePack;
using MessagePack.Formatters;

namespace Lykke.Bil2.RabbitMq.MessagePack
{
    public class TypeConverterFormatterResolver : IFormatterResolver
    {
        public static TypeConverterFormatterResolver Instance { get; }
            = new TypeConverterFormatterResolver();


        private readonly ConcurrentDictionary<Type, IMessagePackFormatter> _formattersCache;
        
        
        private TypeConverterFormatterResolver()
        {
            _formattersCache = new ConcurrentDictionary<Type, IMessagePackFormatter>();
        }
        

        public IMessagePackFormatter<T> GetFormatter<T>()
        {
            var requestedType = typeof(T);
            
            return (IMessagePackFormatter<T>) _formattersCache.GetOrAdd(requestedType, type =>
            {
                var converter = TypeDescriptor.GetConverter(requestedType);
                
                if (converter.CanConvertFromAndTo<byte[]>())
                {
                    return new ByteArrayConverterFormatter<T>(converter);
                }
            
                if (converter.CanConvertFromAndTo<string>())
                {
                    return new StringConverterFormatter<T>(converter);
                }

                return null;
            });
        }
    }
}
