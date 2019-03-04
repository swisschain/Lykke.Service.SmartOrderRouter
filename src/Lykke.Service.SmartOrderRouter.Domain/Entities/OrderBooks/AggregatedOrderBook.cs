using System;
using System.Collections.Generic;

namespace Lykke.Service.SmartOrderRouter.Domain.Entities.OrderBooks
{
    /// <summary>
    /// Represents an aggregated order book.
    /// </summary>
    /// <remarks>
    /// This order book includes orders from all available exchanges grouped by price.
    /// The sell and buy levels can have arbitrage.
    /// </remarks>
    public class AggregatedOrderBook
    {
        /// <summary>
        /// The name of asset pair.
        /// </summary>
        public string AssetPair { get; set; }

        /// <summary>
        /// The order book creation date.
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// A collection of sell order book levels.
        /// </summary>
        public IReadOnlyList<AggregatedOrderBookLevel> SellLevels { get; set; }

        /// <summary>
        /// A collection of buy order book levels.
        /// </summary>
        public IReadOnlyList<AggregatedOrderBookLevel> BuyLevels { get; set; }
    }
}
