using JetBrains.Annotations;

namespace Lykke.Service.SmartOrderRouter.Client.Models.OrderBooks
{
    /// <summary>
    /// Represents a price level in the order book.
    /// </summary>
    [PublicAPI]
    public class OrderBookLevelModel
    {
        /// <summary>
        /// The price of order book level.
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// The volume of order book level.
        /// </summary>
        public decimal Volume { get; set; }
    }
}
