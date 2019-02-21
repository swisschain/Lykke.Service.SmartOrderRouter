using JetBrains.Annotations;
using Lykke.Sdk.Settings;
using Lykke.Service.SmartOrderRouter.Settings.ServiceSettings;

namespace Lykke.Service.SmartOrderRouter.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class AppSettings : BaseAppSettings
    {
        public SmartOrderRouterSettings SmartOrderRouterService { get; set; }
    }
}
