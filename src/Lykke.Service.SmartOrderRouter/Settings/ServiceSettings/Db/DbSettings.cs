using JetBrains.Annotations;
using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.SmartOrderRouter.Settings.ServiceSettings.Db
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class DbSettings
    {
        [AzureTableCheck]
        public string DataConnectionString { get; set; }

        [AzureTableCheck]
        public string LogsConnectionString { get; set; }
    }
}
