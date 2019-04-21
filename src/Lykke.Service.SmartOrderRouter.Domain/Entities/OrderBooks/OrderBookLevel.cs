namespace Lykke.Service.SmartOrderRouter.Domain.Entities.OrderBooks
{
    /// <summary>
    /// Represents a price level in the order book.
    /// </summary>
    public class OrderBookLevel
    {
        public OrderBookLevel()
        {
        }

        public OrderBookLevel(decimal price, decimal volume)
        {
            Price = price;
            Volume = volume;
        }
        
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
