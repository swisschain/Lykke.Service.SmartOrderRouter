using System;
using JetBrains.Annotations;

namespace Lykke.Service.SmartOrderRouter.Client.Models.Settings
{
    /// <summary>
    /// Represents a settings of timers.
    /// </summary>
    [PublicAPI]
    public class TimerSettingsModel
    {
        /// <summary>
        /// The timer interval of market orders execution.
        /// </summary>
        public TimeSpan MarketOrders { get; set; }

        /// <summary>
        /// The timer interval of balances.
        /// </summary>
        public TimeSpan Balances { get; set; }
    }
}
