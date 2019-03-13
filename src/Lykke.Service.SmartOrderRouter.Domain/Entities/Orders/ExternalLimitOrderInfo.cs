namespace Lykke.Service.SmartOrderRouter.Domain.Entities.Orders
{
    /// <summary>
    /// Represent a common limit order details that received from external exchanges.
    /// </summary>
    public class ExternalLimitOrderInfo
    {
        /// <summary>
        /// The unique identifier of order.
        /// </summary>
        public string Id { get; set; }
        
        /// <summary>
        /// The name of asset pair.
        /// </summary>
        public string AssetPair { get; set; }

        /// <summary>
        /// The original price of order.
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// The original volume of order.
        /// </summary>
        public decimal Volume { get; set; }

        /// <summary>
        /// Indicates a type of the order.
        /// </summary>
        public OrderType Type { get; set; }

        /// <summary>
        /// The volume that was executed on exchange.
        /// </summary>
        public decimal ExecutedVolume { get; set; }

        /// <summary>
        /// The average price that was used to execute order. 
        /// </summary>
        public decimal ExecutedPrice { get; set; }
        
        /// <summary>
        /// Indicated a status of the order.
        /// </summary>
        public ExternalLimitOrderStatus Status { get; set; }
    }
}
