using System.Collections.Generic;
using JetBrains.Annotations;

namespace Lykke.Service.SmartOrderRouter.Client.Models.OrderBooks
{
    /// <summary>
    /// Represents a price level in the aggregated order book.
    /// </summary>
    [PublicAPI]
    public class AggregatedOrderBookLevelModel : OrderBookLevelModel
    {
        /// <summary>
        /// A collection of volumes on exchange on price level.
        /// </summary>
        public IReadOnlyDictionary<string, decimal> ExchangeVolumes { get; set; }
    }
}
