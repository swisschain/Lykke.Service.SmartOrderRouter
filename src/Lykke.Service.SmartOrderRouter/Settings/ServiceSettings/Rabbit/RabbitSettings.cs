using JetBrains.Annotations;
using Lykke.Service.SmartOrderRouter.Settings.ServiceSettings.Rabbit.Subscribers;

namespace Lykke.Service.SmartOrderRouter.Settings.ServiceSettings.Rabbit
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class RabbitSettings
    {
        public RabbitSubscribers Subscribers { get; set; }
    }
}
