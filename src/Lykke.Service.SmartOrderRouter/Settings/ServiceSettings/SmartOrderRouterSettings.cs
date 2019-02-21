using JetBrains.Annotations;
using Lykke.Service.SmartOrderRouter.Settings.ServiceSettings.Db;
using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.SmartOrderRouter.Settings.ServiceSettings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class SmartOrderRouterSettings
    {
        public DbSettings Db { get; set; }
    }
}
