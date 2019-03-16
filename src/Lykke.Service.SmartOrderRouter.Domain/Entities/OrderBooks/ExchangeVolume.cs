namespace Lykke.Service.SmartOrderRouter.Domain.Entities.OrderBooks
{
    /// <summary>
    /// Represent a volume that can be realised in external exchange.
    /// </summary>
    public class ExchangeVolume
    {
        /// <summary>
        /// The name of exchange.
        /// </summary>
        public string Exchange { get; set; }

        /// <summary>
        /// The price of volume.
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// The volume in external order book.
        /// </summary>
        public decimal Volume { get; set; }
    }
}
