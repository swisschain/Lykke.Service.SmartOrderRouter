using JetBrains.Annotations;

namespace Lykke.Service.SmartOrderRouter.Settings.ServiceSettings.Rabbit.Subscribers
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class RabbitSubscribers
    {
        public MultiSourceSettings OrderBooks { get; set; }

        public MultiSourceSettings Quotes { get; set; }
    }
}
