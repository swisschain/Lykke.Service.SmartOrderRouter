using AutoMapper;
using JetBrains.Annotations;
using Lykke.Service.SmartOrderRouter.Client.Models.Balances;
using Lykke.Service.SmartOrderRouter.Client.Models.Exchanges;
using Lykke.Service.SmartOrderRouter.Client.Models.ExternalLimitOrders;
using Lykke.Service.SmartOrderRouter.Client.Models.OrderBooks;
using Lykke.Service.SmartOrderRouter.Client.Models.Reports;
using Lykke.Service.SmartOrderRouter.Client.Models.Settings;
using Lykke.Service.SmartOrderRouter.Domain.Entities.Balances;
using Lykke.Service.SmartOrderRouter.Domain.Entities.Exchanges;
using Lykke.Service.SmartOrderRouter.Domain.Entities.OrderBooks;
using Lykke.Service.SmartOrderRouter.Domain.Entities.Orders;
using Lykke.Service.SmartOrderRouter.Domain.Entities.Reports.Balances;
using Lykke.Service.SmartOrderRouter.Domain.Entities.Settings;

namespace Lykke.Service.SmartOrderRouter
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Balance, BalanceModel>(MemberList.Source);

            CreateMap<ExchangeSettings, ExchangeSettingsModel>(MemberList.Source);
            CreateMap<ExchangeSettingsModel, ExchangeSettings>(MemberList.Destination);

            CreateMap<ExternalLimitOrder, ExternalLimitOrderModel>(MemberList.Source);

            CreateMap<AggregatedOrderBook, AggregatedOrderBookModel>(MemberList.Source);
            CreateMap<AggregatedOrderBookLevel, AggregatedOrderBookLevelModel>(MemberList.Source);
            CreateMap<AggregatedOrderBookVolume, AggregatedOrderBookVolumeModel>(MemberList.Source);
            CreateMap<InternalOrderBook, InternalOrderBookModel>(MemberList.Source);
            CreateMap<OrderBookLevel, OrderBookLevelModel>(MemberList.Source);
            CreateMap<ExternalOrderBook, ExternalOrderBookModel>(MemberList.Source);
            CreateMap<ExternalOrderBookLevel, ExternalOrderBookLevelModel>(MemberList.Source);

            CreateMap<BalanceReport, BalanceReportModel>(MemberList.Source);
            CreateMap<BalanceReportItem, BalanceReportItemModel>(MemberList.Source);

            CreateMap<TimerSettings, TimerSettingsModel>(MemberList.Source);
            CreateMap<TimerSettingsModel, TimerSettings>(MemberList.Destination);
        }
    }
}
