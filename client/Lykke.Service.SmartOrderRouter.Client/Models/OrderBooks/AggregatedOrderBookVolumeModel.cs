using JetBrains.Annotations;

namespace Lykke.Service.SmartOrderRouter.Client.Models.OrderBooks
{
    /// <summary>
    /// Represent an exchange order book volume in aggregated price level.
    /// </summary>
    [PublicAPI]
    public class AggregatedOrderBookVolumeModel
    {
        /// <summary>
        /// The name of exchange.
        /// </summary>
        public string Exchange { get; set; }

        /// <summary>
        /// The price of external order book level.
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// The volume of external order book level.
        /// </summary>
        public decimal Volume { get; set; }
    }
}
