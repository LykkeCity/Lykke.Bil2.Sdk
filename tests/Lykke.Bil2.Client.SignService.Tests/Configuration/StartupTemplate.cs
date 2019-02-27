using System;
using JetBrains.Annotations;
using Lykke.Bil2.Sdk.SignService;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.Bil2.SignService.Client.Tests.Configuration
{
    [UsedImplicitly]
    //Startup class is used in configuration of web api 
    public class StartupTemplate
    {
        private const string IntegrationName = "Test";

        [UsedImplicitly]
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            return services.BuildBlockchainSignServiceProvider<AppSettings>(StartupDependencyFactorySingleton.Instance.GetOptionsConfiguration<AppSettings>());
        }

        [UsedImplicitly]
        public void Configure(IApplicationBuilder app)
        {
            app.UseBlockchainSignService(options =>
            {
                options.IntegrationName = IntegrationName;
            });
        }
    }
}
