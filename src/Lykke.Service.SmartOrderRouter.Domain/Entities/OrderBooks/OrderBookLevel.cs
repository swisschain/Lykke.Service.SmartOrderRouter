namespace Lykke.Service.SmartOrderRouter.Domain.Entities.OrderBooks
{
    /// <summary>
    /// Represents a price level in the order book.
    /// </summary>
    public class OrderBookLevel
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
