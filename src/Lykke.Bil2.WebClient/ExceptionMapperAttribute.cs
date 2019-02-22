using System;
using JetBrains.Annotations;

namespace Lykke.Bil2.WebClient
{
    /// <summary>
    /// Specify exceptions mapper for the particular web API method
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    [PublicAPI]
    public class ExceptionMapperAttribute : Attribute
    {
        private static Type _mapperInterfaceType = typeof(IExceptionMapper);

        public Type MapperType { get; }

        /// <summary>
        /// Specify exceptions mapper for the particular web API method
        /// </summary>
        /// <param name="mapperType">Type of the exception mapper. The type should implement <see cref="IExceptionMapper"/></param>
        public ExceptionMapperAttribute(Type mapperType)
        {
            MapperType = mapperType ?? throw new ArgumentNullException(nameof(mapperType));

            if (!_mapperInterfaceType.IsAssignableFrom(mapperType))
            {
                throw new ArgumentException($"Should implement {_mapperInterfaceType.FullName}", nameof(mapperType));
            }
        }
    }
}
