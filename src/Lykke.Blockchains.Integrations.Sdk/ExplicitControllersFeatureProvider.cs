using System.Collections.Generic;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace Lykke.Blockchains.Integrations.Sdk
{
    internal class ExplicitControllersFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
    {
        private IReadOnlyCollection<TypeInfo> ControllerTypes { get; }

        public ExplicitControllersFeatureProvider(IReadOnlyCollection<TypeInfo> controllerTypes)
        {
            ControllerTypes = controllerTypes;
        }

        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
        {
            foreach (var type in ControllerTypes)
            {
                feature.Controllers.Add(type);
            }
        }
    }
}
