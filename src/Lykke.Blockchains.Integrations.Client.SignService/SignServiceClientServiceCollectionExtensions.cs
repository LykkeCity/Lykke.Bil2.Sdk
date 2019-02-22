﻿using System;
using System.Linq;
using JetBrains.Annotations;
using Lykke.Blockchains.Integrations.WebClient;
using Lykke.Common.Log;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.Blockchains.Integrations.Client.SignService
{
    [PublicAPI]
    public static class SignServiceClientServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the client of the blockchain integration sign service to the app services as <see cref="ISignServiceApi"/>
        /// </summary>
        public static IServiceCollection AddSignServiceClient(this IServiceCollection services, string url)
        {
            if (string.IsNullOrWhiteSpace(url) || !Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out _))
            {
                throw new ArgumentException($"Should be a valid URL. Actual value: {url}", nameof(url));
            }

            if (services.All(x => x.ServiceType != typeof(ILogFactory)))
            {
                throw new InvalidOperationException($"{typeof(ILogFactory)} service should be registered");
            }

            services.AddSingleton(s =>
            {
                var generator = HttpClientGeneratorFactory.Create(url, s.GetRequiredService<ILogFactory>());

                return generator.Generate<ISignServiceApi>();
            });

            return services;
        }
    }
}