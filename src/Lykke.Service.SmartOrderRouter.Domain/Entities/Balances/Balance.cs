namespace Lykke.Service.SmartOrderRouter.Domain.Entities.Balances
{
    /// <summary>
    /// Represents a balance of an asset.
    /// </summary>
    public class Balance
    {
        /// <summary>
        /// Initializes a new instance of <see cref="Balance"/> of asset with amount of balance.
        /// </summary>
        /// <param name="exchange">Exchange name.</param>
        /// <param name="asset">The asset name.</param>
        /// <param name="amount">The amount of balance.</param>
        /// <param name="reserved">The amount that currently are reserved.</param>
        public Balance(string exchange, string asset, decimal amount, decimal reserved)
        {
            Exchange = exchange;
            Asset = asset;
            Amount = amount;
            Reserved = reserved;
        }

        /// <summary>
        /// Exchange name.
        /// </summary>
        public string Exchange { get; }

        /// <summary>
        /// The asset name.
        /// </summary>
        public string Asset { get; }

        /// <summary>
        /// The amount of balance.
        /// </summary>
        public decimal Amount { get; }
        
        /// <summary>
        /// The amount that currently are reserved.
        /// </summary>
        public decimal Reserved { get; }
    }
}
