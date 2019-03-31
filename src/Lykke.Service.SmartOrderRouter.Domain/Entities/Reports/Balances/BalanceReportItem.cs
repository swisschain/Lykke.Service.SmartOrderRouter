namespace Lykke.Service.SmartOrderRouter.Domain.Entities.Reports.Balances
{
    /// <summary>
    /// Represents a balance report item.
    /// </summary>
    public class BalanceReportItem
    {
        /// <summary>
        /// The name of the exchange.
        /// </summary>
        public string Exchange { get; set; }

        /// <summary>
        /// The name of the asset.
        /// </summary>
        public string Asset { get; set; }

        /// <summary>
        /// The current amount of balance.
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// The current amount in USD.
        /// </summary>
        public decimal AmountUsd { get; set; }
    }
}
