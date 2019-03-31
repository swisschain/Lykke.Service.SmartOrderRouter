using System;

namespace Lykke.Service.SmartOrderRouter.Domain.Entities
{
    /// <summary>
    /// Represents a quote of the asset pair.
    /// </summary>
    public class Quote
    {
        /// <summary>
        /// Initializes a new instance of <see cref="Quote"/> with parameters.
        /// </summary>
        /// <param name="assetPair">The name of asset pair.</param>
        /// <param name="time">The time of price.</param>
        /// <param name="ask">The best sell price in order book.</param>
        /// <param name="bid">The best buy price in order book.</param>
        /// <param name="exchange">The name of exchange</param>
        public Quote(string assetPair, DateTime time, decimal ask, decimal bid, string exchange)
        {
            AssetPair = assetPair;
            Time = time;
            Ask = ask;
            Bid = bid;
            Mid = (ask + bid) / 2m;
            Spread = Ask - Bid;
            Exchange = exchange;
        }

        /// <summary>
        /// The identifier of the asset pair.
        /// </summary>
        public string AssetPair { get; }

        /// <summary>
        /// The time of price.
        /// </summary>
        public DateTime Time { get; }

        /// <summary>
        /// The best sell price in order book.
        /// </summary>
        public decimal Ask { get; }

        /// <summary>
        /// The best buy price in order book.
        /// </summary>
        public decimal Bid { get; }

        /// <summary>
        /// The mid price in order book.
        /// </summary>
        public decimal Mid { get; }

        /// <summary>
        /// The spread of order book.
        /// </summary>
        public decimal Spread { get; }

        /// <summary>
        /// The name of exchange.
        /// </summary>
        public string Exchange { get; }
    }
}
