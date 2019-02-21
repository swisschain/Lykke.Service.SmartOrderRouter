using Autofac;
using JetBrains.Annotations;
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
    }
}
