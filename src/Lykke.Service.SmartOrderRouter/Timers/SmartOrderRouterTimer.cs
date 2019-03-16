using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Common.Log;
using Lykke.Service.SmartOrderRouter.Domain;
using Lykke.Service.SmartOrderRouter.Domain.Entities.Settings;
using Lykke.Service.SmartOrderRouter.Domain.Services;

namespace Lykke.Service.SmartOrderRouter.Timers
{
    [UsedImplicitly]
    public class SmartOrderRouterTimer : Timer
    {
        private readonly ISmartOrderRouter _smartOrderRouter;
        private readonly ISettingsService _settingsService;

        public SmartOrderRouterTimer(
            ISmartOrderRouter balanceService,
            ISettingsService settingsService,
            ILogFactory logFactory)
        {
            _smartOrderRouter = balanceService;
            _settingsService = settingsService;
            Log = logFactory.CreateLog(this);
        }

        protected override async Task<TimeSpan> GetDelayAsync()
        {
            TimerSettings timerSettings = await _settingsService.GetTimerSettingsAsync();

            return timerSettings.MarketOrders;
        }

        protected override Task OnExecuteAsync(CancellationToken cancellation)
        {
            return _smartOrderRouter.ExecuteMarketOrdersAsync();
        }
    }
}
