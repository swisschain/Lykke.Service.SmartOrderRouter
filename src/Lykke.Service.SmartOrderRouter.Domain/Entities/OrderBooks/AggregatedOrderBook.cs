using System;
using System.Collections.Generic;
using System.Linq;

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

        public IReadOnlyList<ExchangeVolume> GetSellVolumes(decimal volume)
            => GetVolumes(volume, SellLevels.OrderBy(o => o.Price).ToList());
        
        public IReadOnlyList<ExchangeVolume> GetBuyVolumes(decimal volume)
            => GetVolumes(volume, BuyLevels.OrderByDescending(o => o.Price).ToList());

        private IReadOnlyList<ExchangeVolume> GetVolumes(decimal volume,
            IReadOnlyList<AggregatedOrderBookLevel> levels)
        {
            var exchangeVolumes = new Dictionary<string,  ExchangeVolume>();

            decimal remainingVolume = volume;
            
            foreach (AggregatedOrderBookLevel level in levels)
            {
                IEnumerable<AggregatedOrderBookVolume> orderBookVolumes =
                    level.ExchangeVolumes.OrderByDescending(o => o.Volume);
                
                foreach (AggregatedOrderBookVolume orderBookVolume in orderBookVolumes)
                {
                    if (!exchangeVolumes.TryGetValue(orderBookVolume.Exchange, out ExchangeVolume exchangeVolume))
                    {
                        exchangeVolume = new ExchangeVolume {Exchange = orderBookVolume.Exchange};
                        exchangeVolumes[orderBookVolume.Exchange] = exchangeVolume;
                    }

                    exchangeVolume.Price = orderBookVolume.Price;

                    if (remainingVolume > orderBookVolume.Volume)
                    {
                        exchangeVolume.Volume += orderBookVolume.Volume;
                        remainingVolume -= orderBookVolume.Volume;
                    }
                    else
                    {
                        exchangeVolume.Volume += remainingVolume;
                        remainingVolume = 0;
                    }
                    
                    if(remainingVolume <= 0)
                        break;
                }
                
                if(remainingVolume <= 0)
                    break;
            }

            return exchangeVolumes.Values.ToList();
        }
    }
}
