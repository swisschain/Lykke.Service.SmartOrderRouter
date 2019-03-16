using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Common.Log;
using Lykke.Service.SmartOrderRouter.Domain.Entities.Settings;
using Lykke.Service.SmartOrderRouter.Domain.Services;

namespace Lykke.Service.SmartOrderRouter.Timers
{
    [UsedImplicitly]
    public class BalancesTimer : Timer
    {
        private readonly IBalanceService _balanceService;
        private readonly ISettingsService _settingsService;

        public BalancesTimer(
            IBalanceService balanceService,
            ISettingsService settingsService,
            ILogFactory logFactory)
        {
            _balanceService = balanceService;
            _settingsService = settingsService;
            Log = logFactory.CreateLog(this);
        }

        protected override async Task<TimeSpan> GetDelayAsync()
        {
            TimerSettings timerSettings = await _settingsService.GetTimerSettingsAsync();

            return timerSettings.Balances;
        }

        protected override Task OnExecuteAsync(CancellationToken cancellation)
        {
            return _balanceService.UpdateAsync();
        }
    }
}
