using JetBrains.Annotations;

namespace Lykke.Service.SmartOrderRouter.Client.Models.Quotes
{
    /// <summary>
    /// Represents a quote.
    /// </summary>
    [PublicAPI]
    public class QuoteModel
    {
        /// <summary>
        /// The name of exchange.
        /// </summary>
        public string Exchange { get; set; }

        /// <summary>
        /// The identifier of the asset pair.
        /// </summary>
        public string AssetPair { get; set; }

        /// <summary>
        /// The best sell price in order book.
        /// </summary>
        public decimal Ask { get; set; }

        /// <summary>
        /// The best buy price in order book.
        /// </summary>
        public decimal Bid { get; set; }
    }
}
