using AutoMapper;
using JetBrains.Annotations;
using Lykke.Service.SmartOrderRouter.Client.Models.Exchanges;
using Lykke.Service.SmartOrderRouter.Client.Models.OrderBooks;
using Lykke.Service.SmartOrderRouter.Domain.Entities.Exchanges;
using Lykke.Service.SmartOrderRouter.Domain.Entities.OrderBooks;

namespace Lykke.Service.SmartOrderRouter
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<ExchangeSettings, ExchangeSettingsModel>(MemberList.Source);
            CreateMap<ExchangeSettingsModel, ExchangeSettings>(MemberList.Destination);

            CreateMap<AggregatedOrderBook, AggregatedOrderBookModel>(MemberList.Source);
            CreateMap<AggregatedOrderBookLevel, AggregatedOrderBookLevelModel>(MemberList.Source);
            CreateMap<InternalOrderBook, InternalOrderBookModel>(MemberList.Source);
            CreateMap<OrderBookLevel, OrderBookLevelModel>(MemberList.Source);
            CreateMap<ExternalOrderBook, ExternalOrderBookModel>(MemberList.Source);
            CreateMap<ExternalOrderBookLevel, ExternalOrderBookLevelModel>(MemberList.Source);            
        }
    }
}
