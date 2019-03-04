using System.Collections.Generic;

namespace Lykke.Service.SmartOrderRouter.Domain.Entities.OrderBooks
{
    /// <summary>
    /// Represents a price level in the aggregated order book.
    /// </summary>
    public class AggregatedOrderBookLevel : OrderBookLevel
    {
        /// <summary>
        /// A collection of volumes on exchange on price level.
        /// </summary>
        public IReadOnlyDictionary<string, decimal> ExchangeVolumes { get; set; }
    }
}
