using System.Collections.Generic;
using JetBrains.Annotations;

namespace Lykke.Service.SmartOrderRouter.Client.Models.Reports
{
    /// <summary>
    /// Represents a balance report.
    /// </summary>
    [PublicAPI]
    public class BalanceReportModel
    {
        /// <summary>
        /// The items of balance report.
        /// </summary>
        public IReadOnlyList<BalanceReportItemModel> Items { get; set; }
    }
}
