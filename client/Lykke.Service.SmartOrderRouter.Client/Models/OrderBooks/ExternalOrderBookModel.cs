using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Lykke.Service.SmartOrderRouter.Client.Models.OrderBooks
{
    /// <summary>
    /// Represents an external order book.
    /// </summary>
    [PublicAPI]
    public class ExternalOrderBookModel
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
        public IReadOnlyList<ExternalOrderBookLevelModel> SellLevels { get; set; }

        /// <summary>
        /// A collection of buy order book levels.
        /// </summary>
        public IReadOnlyList<ExternalOrderBookLevelModel> BuyLevels { get; set; }
    }
}
