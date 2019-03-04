using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Lykke.Service.SmartOrderRouter.Client.Models.OrderBooks
{
    /// <summary>
    /// Represents an aggregated order book.
    /// </summary>
    /// <remarks>
    /// This order book includes orders from all available exchanges grouped by price.
    /// The sell and buy levels can have arbitrage.
    /// </remarks>
    [PublicAPI]
    public class AggregatedOrderBookModel
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
        public IReadOnlyList<AggregatedOrderBookLevelModel> SellLevels { get; set; }

        /// <summary>
        /// A collection of buy order book levels.
        /// </summary>
        public IReadOnlyList<AggregatedOrderBookLevelModel> BuyLevels { get; set; }
    }
}
