using Deliveryix.Commons.Application.Abstractions;
using Modules.Identity.IntegrationEvents;

namespace Modules.Identity.Application.Abstractions
{
    public sealed class ModuleInfo : IModuleInfo
    {
        public string Name => ModuleExtensions.ModuleName;
    }
}