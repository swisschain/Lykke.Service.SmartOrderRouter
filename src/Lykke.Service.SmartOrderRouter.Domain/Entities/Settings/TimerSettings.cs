using System;

namespace Lykke.Service.SmartOrderRouter.Domain.Entities.Settings
{
    /// <summary>
    /// Represents a settings of timers.
    /// </summary>
    public class TimerSettings
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
