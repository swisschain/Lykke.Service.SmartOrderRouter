using AutoMapper;
using JetBrains.Annotations;
using Lykke.Service.SmartOrderRouter.AzureRepositories.Exchanges;
using Lykke.Service.SmartOrderRouter.AzureRepositories.Orders;
using Lykke.Service.SmartOrderRouter.AzureRepositories.Settings;
using Lykke.Service.SmartOrderRouter.Domain.Entities.Exchanges;
using Lykke.Service.SmartOrderRouter.Domain.Entities.Orders;
using Lykke.Service.SmartOrderRouter.Domain.Entities.Settings;

namespace Lykke.Service.SmartOrderRouter.AzureRepositories
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<ExchangeSettings, ExchangeSettingsEntity>(MemberList.Source);
            CreateMap<ExchangeSettingsEntity, ExchangeSettings>(MemberList.Destination);


            CreateMap<MarketOrder, MarketOrderEntity>(MemberList.Source);
            CreateMap<MarketOrderEntity, MarketOrder>(MemberList.Destination);

            CreateMap<ExternalLimitOrder, ExternalLimitOrderEntity>(MemberList.Source);
            CreateMap<ExternalLimitOrderEntity, ExternalLimitOrder>(MemberList.Destination);


            CreateMap<TimerSettings, TimerSettingsEntity>(MemberList.Source);
            CreateMap<TimerSettingsEntity, TimerSettings>(MemberList.Destination);
        }
    }
}
