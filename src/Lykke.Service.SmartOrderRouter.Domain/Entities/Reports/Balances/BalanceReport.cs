using System.Collections.Generic;

namespace Lykke.Service.SmartOrderRouter.Domain.Entities.Reports.Balances
{
    /// <summary>
    /// Represents a balance report.
    /// </summary>
    public class BalanceReport
    {
        /// <summary>
        /// The items of balance report.
        /// </summary>
        public IReadOnlyList<BalanceReportItem> Items { get; set; }
    }
}
