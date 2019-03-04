using Autofac;
using AzureStorage.Tables;
using JetBrains.Annotations;
using Lykke.Common.Log;
using Lykke.Service.SmartOrderRouter.AzureRepositories.Exchanges;
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
        }
    }
}
