using System.Collections.Generic;
using JetBrains.Annotations;
using Lykke.Service.SmartOrderRouter.Settings.ServiceSettings.Db;
using Lykke.Service.SmartOrderRouter.Settings.ServiceSettings.ExchangeAdapters;
using Lykke.Service.SmartOrderRouter.Settings.ServiceSettings.Rabbit;

namespace Lykke.Service.SmartOrderRouter.Settings.ServiceSettings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class SmartOrderRouterSettings
    {
        public string Name { get; set; }
        
        public DbSettings Db { get; set; }
        
        public RabbitSettings Rabbit { get; set; }
        
        public IReadOnlyList<ExchangeAdapterEndpoint> ExchangeAdapters { get; set; }
    }
}
