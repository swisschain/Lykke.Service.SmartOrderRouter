using System;
using System.Collections.Generic;
using System.Linq;
using Lykke.Service.SmartOrderRouter.Domain.Entities.Orders;

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

        public IReadOnlyList<ExchangeVolume> GetSellVolumes(decimal volume,
            IReadOnlyList<ExternalLimitOrder> activeLimitOrders,
            IReadOnlyList<string> excludedExchanges)
            => GetVolumes(volume, activeLimitOrders, excludedExchanges,
                SellLevels.OrderBy(o => o.Price).ToList());

        public IReadOnlyList<ExchangeVolume> GetBuyVolumes(decimal volume,
            IReadOnlyList<ExternalLimitOrder> activeLimitOrders,
            IReadOnlyList<string> excludedExchanges)
            => GetVolumes(volume, activeLimitOrders, excludedExchanges,
                BuyLevels.OrderByDescending(o => o.Price).ToList());

        private static IReadOnlyList<ExchangeVolume> GetVolumes(
            decimal volume,
            IReadOnlyList<ExternalLimitOrder> activeLimitOrders,
            IReadOnlyList<string> excludedExchanges,
            IEnumerable<AggregatedOrderBookLevel> levels)
        {
            var exchangeVolumes = new Dictionary<string, ExchangeVolume>();

            decimal totalActiveVolume = activeLimitOrders.Sum(o => o.Volume);
            
            Dictionary<string, decimal> activeVolumes = activeLimitOrders
                .GroupBy(o => o.Exchange)
                .ToDictionary(o => o.Key, o => o.Sum(p => p.Volume));
            
            decimal remainingVolume = volume - totalActiveVolume;

            foreach (AggregatedOrderBookLevel level in levels)
            {
                IEnumerable<AggregatedOrderBookVolume> orderBookVolumes =
                    level.ExchangeVolumes.OrderByDescending(o => o.Volume);

                foreach (AggregatedOrderBookVolume orderBookVolume in orderBookVolumes)
                {
                    if(excludedExchanges.Contains(orderBookVolume.Exchange))
                        continue;
                    
                    decimal levelVolume = orderBookVolume.Volume;
                    
                    if (activeVolumes.TryGetValue(orderBookVolume.Exchange, out decimal activeVolume))
                    {
                        if (levelVolume >= activeVolume)
                        {
                            levelVolume -= activeVolume;
                            activeVolumes.Remove(orderBookVolume.Exchange);
                        }
                        else
                        {
                            activeVolumes[orderBookVolume.Exchange] -= levelVolume;
                            levelVolume = 0;
                        }
                    }

                    if(levelVolume <= 0)
                        continue;
                    
                    if (!exchangeVolumes.TryGetValue(orderBookVolume.Exchange, out ExchangeVolume exchangeVolume))
                    {
                        exchangeVolume = new ExchangeVolume {Exchange = orderBookVolume.Exchange};
                        exchangeVolumes[orderBookVolume.Exchange] = exchangeVolume;
                    }
                    
                    exchangeVolume.Price = orderBookVolume.Price;

                    if (remainingVolume > levelVolume)
                    {
                        exchangeVolume.Volume += levelVolume;
                        remainingVolume -= levelVolume;
                    }
                    else
                    {
                        exchangeVolume.Volume += remainingVolume;
                        remainingVolume = 0;
                    }

                    if (remainingVolume <= 0)
                        break;
                }

                if (remainingVolume <= 0)
                    break;
            }

            return exchangeVolumes.Values.ToList();
        }
    }
}
