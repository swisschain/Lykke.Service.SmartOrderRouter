using System;

namespace Lykke.Service.SmartOrderRouter.Domain.Entities.Orders
{
    /// <summary>
    /// Represent an external limit order.
    /// </summary>
    public class ExternalLimitOrder
    {
        public ExternalLimitOrder()
        {
        }

        public ExternalLimitOrder(string id, string exchange, string assetPair, decimal price, decimal volume,
            OrderType orderType)
        {
            Id = id;
            Exchange = exchange;
            AssetPair = assetPair;
            Price = price;
            Volume = volume;
            Type = orderType;
            Status = ExternalLimitOrderStatus.Active;
            CreatedDate = DateTime.UtcNow;
        }

        /// <summary>
        /// The unique identifier of order.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The name of exchange.
        /// </summary>
        public string Exchange { get; set; }

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
        public decimal? ExecutedVolume { get; set; }

        /// <summary>
        /// The average price that was used to execute order. 
        /// </summary>
        public decimal? ExecutedPrice { get; set; }

        /// <summary>
        /// Indicated a status of the order.
        /// </summary>
        public ExternalLimitOrderStatus Status { get; set; }

        /// <summary>
        /// The date time of order creation.
        /// </summary>
        public DateTime CreatedDate { get; set; }

        public void Execute(ExternalLimitOrderInfo externalLimitOrderInfo)
        {
            Status = externalLimitOrderInfo.Status;
            ExecutedPrice = externalLimitOrderInfo.ExecutedPrice;
            ExecutedVolume = externalLimitOrderInfo.ExecutedVolume;
        }

        public void Cancel()
        {
            Status = ExternalLimitOrderStatus.Cancelled;
        }
    }
}
