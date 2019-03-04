using Lykke.Service.SmartOrderRouter.Domain.Services;

namespace Lykke.Service.SmartOrderRouter.DomainServices
{
    public class SettingsService : ISettingsService
    {
        private readonly string _name;

        public SettingsService(string name)
        {
            _name = name;
        }

        public string GetExchangeName()
            => _name;
    }
}
