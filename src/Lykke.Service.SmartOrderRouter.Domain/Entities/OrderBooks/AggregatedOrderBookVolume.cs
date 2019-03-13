namespace Lykke.Service.SmartOrderRouter.Domain.Entities.OrderBooks
{
    /// <summary>
    /// Represent an exchange order book volume in aggregated price level.
    /// </summary>
    public class AggregatedOrderBookVolume
    {
        public AggregatedOrderBookVolume()
        {
        }
        
        public AggregatedOrderBookVolume(string exchange, decimal price, decimal volume)
        {
            Exchange = exchange;
            Price = price;
            Volume = volume;
        }

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
