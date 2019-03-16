using Autofac;
using AzureStorage.Tables;
using AzureStorage.Tables.Templates.Index;
using JetBrains.Annotations;
using Lykke.Common.Log;
using Lykke.Service.SmartOrderRouter.AzureRepositories.Exchanges;
using Lykke.Service.SmartOrderRouter.AzureRepositories.Orders;
using Lykke.Service.SmartOrderRouter.AzureRepositories.Settings;
using Lykke.Service.SmartOrderRouter.Domain.Repositories;
using Lykke.SettingsReader;

namespace Lykke.Service.SmartOrderRouter.AzureRepositories
{
    [UsedImplicitly]
    public class AutofacModule : Module
    {
        private readonly IReloadingManager<string> _connectionString;

        public AutofacModule(IReloadingManager<string> connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(container => new ExchangeSettingsRepository(
                    AzureTableStorage<ExchangeSettingsEntity>.Create(_connectionString,
                        "ExchangeSettings", container.Resolve<ILogFactory>())))
                .As<IExchangeSettingsRepository>()
                .SingleInstance();


            builder.Register(container => new ExternalLimitOrderRepository(
                    AzureTableStorage<ExternalLimitOrderEntity>.Create(_connectionString,
                        "ExternalLimitOrders", container.Resolve<ILogFactory>()),
                    AzureTableStorage<AzureIndex>.Create(_connectionString,
                        "ExternalLimitOrderIndices", container.Resolve<ILogFactory>())))
                .As<IExternalLimitOrderRepository>()
                .SingleInstance();

            builder.Register(container => new MarketOrderRepository(
                    AzureTableStorage<MarketOrderEntity>.Create(_connectionString,
                        "MarketOrders", container.Resolve<ILogFactory>())))
                .As<IMarketOrderRepository>()
                .SingleInstance();


            builder.Register(container => new TimerSettingsRepository(
                    AzureTableStorage<TimerSettingsEntity>.Create(_connectionString,
                        "Settings", container.Resolve<ILogFactory>())))
                .As<ITimerSettingsRepository>()
                .SingleInstance();
        }
    }
}
