using System.Linq;
using Autofac;
using JetBrains.Annotations;
using Lykke.Sdk;
using Lykke.Service.MarketInstruments.Client;
using Lykke.Service.SmartOrderRouter.Managers;
using Lykke.Service.SmartOrderRouter.Rabbit.Subscribers;
using Lykke.Service.SmartOrderRouter.Settings;
using Lykke.Service.SmartOrderRouter.Settings.ServiceSettings.Rabbit.Subscribers;
using Lykke.SettingsReader;

namespace Lykke.Service.SmartOrderRouter
{
    [UsedImplicitly]
    public class AutofacModule : Module
    {
        private readonly IReloadingManager<AppSettings> _settings;

        public AutofacModule(IReloadingManager<AppSettings> settings)
        {
            _settings = settings;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterModule(new DomainServices.AutofacModule(
                _settings.CurrentValue.SmartOrderRouterService.Name,
                _settings.CurrentValue.SmartOrderRouterService.ExchangeAdapters.Select(o => o.Name).ToList()));
            builder.RegisterModule(new AzureRepositories.AutofacModule(
                _settings.Nested(o => o.SmartOrderRouterService.Db.DataConnectionString)));

            RegisterRabbit(builder);

            RegisterClients(builder);

            builder.RegisterType<StartupManager>()
                .As<IStartupManager>();

            builder.RegisterType<ShutdownManager>()
                .As<IShutdownManager>();
        }

        private void RegisterRabbit(ContainerBuilder builder)
        {
            MultiSourceSettings orderBooksSettings =
                _settings.CurrentValue.SmartOrderRouterService.Rabbit.Subscribers.OrderBooks;

            foreach (string exchange in orderBooksSettings.Exchanges)
            {
                builder.RegisterType<OrderBookSubscriber>()
                    .AsSelf()
                    .WithParameter(TypedParameter.From(new SubscriberSettings
                    {
                        Exchange = exchange,
                        QueueSuffix = orderBooksSettings.QueueSuffix,
                        ConnectionString = orderBooksSettings.ConnectionString
                    }))
                    .SingleInstance();
            }
        }

        private void RegisterClients(ContainerBuilder builder)
        {
            builder.RegisterMarketInstrumentsClient(_settings.CurrentValue.MarketInstrumentsServiceClient, null);
        }
    }
}
