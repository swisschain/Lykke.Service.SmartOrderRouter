using System;

namespace Lykke.Service.SmartOrderRouter.Domain.Entities.Orders
{
    /// <summary>
    /// Represents a market order.
    /// </summary>
    public class MarketOrder
    {
        public MarketOrder()
        {
        }

        public MarketOrder(string clientId, string assetPair, OrderType type, decimal volume)
        {
            Id = Guid.NewGuid().ToString();
            ClientId = clientId;
            AssetPair = assetPair;
            Type = type;
            Volume = volume;
            Status = MarketOrderStatus.New;
            CreatedDate = DateTime.UtcNow;
        }
        
        /// <summary>
        /// The unique identifier of order.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The identifier of the client that created order.
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// The name of asset pair.  
        /// </summary>
        public string AssetPair { get; set; }

        /// <summary>
        /// Indicates the order type.
        /// </summary>
        public OrderType Type { get; set; }

        /// <summary>
        /// The order volume.
        /// </summary>
        public decimal Volume { get; set; }

        /// <summary>
        /// The order executed price.
        /// </summary>
        public decimal ExecutedPrice { get; set; }

        /// <summary>
        /// Indicates the order status.
        /// </summary>
        public MarketOrderStatus Status { get; set; }

        /// <summary>
        /// The order creation date.
        /// </summary>
        public DateTime CreatedDate { get; set; }
    }
}
