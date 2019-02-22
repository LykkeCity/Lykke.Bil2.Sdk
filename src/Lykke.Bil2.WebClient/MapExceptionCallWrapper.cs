using System;
using System.Collections.Concurrent;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Lykke.Bil2.WebClient.Exceptions;
using Lykke.HttpClientGenerator.Infrastructure;
using Refit;

namespace Lykke.Bil2.WebClient
{
    internal class MapExceptionCallWrapper : ICallsWrapper
    {
        private readonly ConcurrentDictionary<MethodInfo, IExceptionMapper> _mappersCache;

        public MapExceptionCallWrapper()
        {
            _mappersCache = new ConcurrentDictionary<MethodInfo, IExceptionMapper>();
        }

        public async Task<object> HandleMethodCall(MethodInfo targetMethod, object[] args, Func<Task<object>> innerHandler)
        {
            try
            {
                return await innerHandler();
            }
            catch (ApiException ex)
            {
                var mapper = _mappersCache.GetOrAdd(targetMethod, m =>
                {
                    var mapperAttribute = m.GetCustomAttribute<ExceptionMapperAttribute>();

                    if (mapperAttribute == null)
                    {
                        return null;
                    }

                    return (IExceptionMapper) Activator.CreateInstance(mapperAttribute.MapperType);
                });

                mapper?.ThrowMappedExceptionOrPassThrough(ex);

                switch (ex.StatusCode)
                {
                    case HttpStatusCode.BadRequest:
                        throw new BadRequestWebApiException(ex);
                    case HttpStatusCode.NotFound:
                        throw new NotFoundWebApiException(ex);
                    case HttpStatusCode.InternalServerError:
                        throw new InternalServerErrorWebApiException(ex);
                    case HttpStatusCode.NotImplemented:
                        throw new NotImplementedWebApiException(ex);
                    default:
                        throw;
                }
            }
        }
    }
}
