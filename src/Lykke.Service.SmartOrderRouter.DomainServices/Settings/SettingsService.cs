using System;
using System.Threading.Tasks;
using Lykke.Service.SmartOrderRouter.Domain.Entities.Settings;
using Lykke.Service.SmartOrderRouter.Domain.Repositories;
using Lykke.Service.SmartOrderRouter.Domain.Services;

namespace Lykke.Service.SmartOrderRouter.DomainServices.Settings
{
    public class SettingsService : ISettingsService
    {
        private readonly ITimerSettingsRepository _timerSettingsRepository;
        private readonly string _name;

        private TimerSettings _timerSettings;

        public SettingsService(
            ITimerSettingsRepository timerSettingsRepository,
            string name)
        {
            _timerSettingsRepository = timerSettingsRepository;
            _name = name;
        }

        public string GetExchangeName()
            => _name;

        public async Task<TimerSettings> GetTimerSettingsAsync()
        {
            if (_timerSettings == null)
            {
                TimerSettings timerSettings = await _timerSettingsRepository.GetAsync();

                if (timerSettings == null)
                    timerSettings = new TimerSettings();

                if (timerSettings.Balances == TimeSpan.Zero)
                    timerSettings.Balances = TimeSpan.FromMinutes(1);

                if (timerSettings.MarketOrders == TimeSpan.Zero)
                    timerSettings.MarketOrders = TimeSpan.FromSeconds(30);

                _timerSettings = timerSettings;
            }

            return _timerSettings;
        }

        public async Task UpdateTimerSettingsAsync(TimerSettings timerSettings)
        {
            await _timerSettingsRepository.UpdateAsync(timerSettings);

            _timerSettings = timerSettings;
        }
    }
}
