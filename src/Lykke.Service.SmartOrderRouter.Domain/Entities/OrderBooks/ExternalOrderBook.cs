using System;
using System.Collections.Generic;

namespace Lykke.Service.SmartOrderRouter.Domain.Entities.OrderBooks
{
    /// <summary>
    /// Represents an external order book.
    /// </summary>
    public class ExternalOrderBook
    {
        /// <summary>
        /// The name of exchange.
        /// </summary>
        public string Exchange { get; set; }

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
        public IReadOnlyList<ExternalOrderBookLevel> SellLevels { get; set; }

        /// <summary>
        /// A collection of buy order book levels.
        /// </summary>
        public IReadOnlyList<ExternalOrderBookLevel> BuyLevels { get; set; }
    }
}
