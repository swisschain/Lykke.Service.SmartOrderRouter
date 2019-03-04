using System.Collections.Generic;
using Autofac;
using JetBrains.Annotations;
using Lykke.Service.SmartOrderRouter.Domain.Services;
using Lykke.Service.SmartOrderRouter.DomainServices.Exchanges;
using Lykke.Service.SmartOrderRouter.DomainServices.OrderBooks;

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


            builder.RegisterType<SettingsService>()
                .As<ISettingsService>()
                .WithParameter(TypedParameter.From(_name))
                .SingleInstance();
        }
    }
}
