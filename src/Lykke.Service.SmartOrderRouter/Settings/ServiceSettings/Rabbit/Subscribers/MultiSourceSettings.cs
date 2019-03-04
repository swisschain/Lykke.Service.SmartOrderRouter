using JetBrains.Annotations;
using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.SmartOrderRouter.Settings.ServiceSettings.Rabbit.Subscribers
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class MultiSourceSettings
    {
        [AmqpCheck]
        public string ConnectionString { get; set; }

        public string[] Exchanges { get; set; }

        public string QueueSuffix { get; set; }
    }
}
