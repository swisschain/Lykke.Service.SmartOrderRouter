namespace Lykke.Service.SmartOrderRouter.Domain.Entities.OrderBooks
{
    /// <summary>
    /// Represents a price level in the external order book.
    /// </summary>
    public class ExternalOrderBookLevel : OrderBookLevel
    {
        /// <summary>
        /// The total markup that was applied to the price (market fees).
        /// </summary>
        public decimal Markup { get; set; }

        /// <summary>
        /// The original price of external order book level.
        /// </summary>
        public decimal OriginalPrice { get; set; }
    }
}
