using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Lykke.Service.SmartOrderRouter.Client.Models.OrderBooks
{
    /// <summary>
    /// Represents an internal order book.
    /// </summary>
    [PublicAPI]
    public class InternalOrderBookModel
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
        public IReadOnlyList<OrderBookLevelModel> SellLevels { get; set; }

        /// <summary>
        /// A collection of buy order book levels.
        /// </summary>
        public IReadOnlyList<OrderBookLevelModel> BuyLevels { get; set; }
    }
}
