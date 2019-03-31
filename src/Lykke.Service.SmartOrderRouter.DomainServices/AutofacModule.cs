using System.Collections.Generic;
using Autofac;
using JetBrains.Annotations;
using Lykke.Service.SmartOrderRouter.Domain;
using Lykke.Service.SmartOrderRouter.Domain.Services;
using Lykke.Service.SmartOrderRouter.DomainServices.Balances;
using Lykke.Service.SmartOrderRouter.DomainServices.Exchanges;
using Lykke.Service.SmartOrderRouter.DomainServices.OrderBooks;
using Lykke.Service.SmartOrderRouter.DomainServices.Orders;
using Lykke.Service.SmartOrderRouter.DomainServices.Quotes;
using Lykke.Service.SmartOrderRouter.DomainServices.Reports;
using Lykke.Service.SmartOrderRouter.DomainServices.Settings;

namespace Lykke.Service.SmartOrderRouter.DomainServices
{
    [UsedImplicitly]
    public class AutofacModule : Module
    {
        private readonly string _name;
        private readonly IReadOnlyList<string> _exchanges;

        public AutofacModule(
            string name,
            IReadOnlyList<string> exchanges)
        {
            _name = name;
            _exchanges = exchanges;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<BalanceService>()
                .As<IBalanceService>()
                .SingleInstance();


            foreach (string exchange in _exchanges)
            {
                builder.RegisterType<Exchange>()
                    .As<IExchange>()
                    .WithParameter(TypedParameter.From(exchange))
                    .SingleInstance();
            }

            builder.RegisterType<ExchangeSettingsService>()
                .As<IExchangeSettingsService>()
                .WithParameter(TypedParameter.From(_exchanges))
                .SingleInstance();


            builder.RegisterType<AggregatedOrderBookService>()
                .As<IAggregatedOrderBookService>()
                .SingleInstance();

            builder.RegisterType<ExternalOrderBookService>()
                .As<IExternalOrderBookService>()
                .SingleInstance();

            builder.RegisterType<InternalOrderBookService>()
                .As<IInternalOrderBookService>()
                .SingleInstance();

            builder.RegisterType<OrderBookService>()
                .As<IOrderBookService>()
                .SingleInstance();


            builder.RegisterType<ExternalLimitOrderService>()
                .As<IExternalLimitOrderService>()
                .SingleInstance();

            builder.RegisterType<MarketOrderService>()
                .As<IMarketOrderService>()
                .SingleInstance();


            builder.RegisterType<QuoteService>()
                .As<IQuoteService>()
                .SingleInstance();


            builder.RegisterType<BalanceReportService>()
                .As<IBalanceReportService>()
                .SingleInstance();


            builder.RegisterType<SettingsService>()
                .As<ISettingsService>()
                .WithParameter(TypedParameter.From(_name))
                .SingleInstance();


            builder.RegisterType<SmartOrderRouter>()
                .As<ISmartOrderRouter>()
                .SingleInstance();
        }
    }
}
