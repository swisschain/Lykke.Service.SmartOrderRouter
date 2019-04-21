namespace Lykke.Service.SmartOrderRouter.Domain.Entities.OrderBooks
{
    /// <summary>
    /// Represents a price level in the external order book.
    /// </summary>
    public class ExternalOrderBookLevel : OrderBookLevel
    {
        public ExternalOrderBookLevel()
        {
        }

        public ExternalOrderBookLevel(decimal price, decimal volume, decimal markup, decimal originalPrice)
            : base(price, volume)
        {
            Markup = markup;
            OriginalPrice = originalPrice;
        }

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
