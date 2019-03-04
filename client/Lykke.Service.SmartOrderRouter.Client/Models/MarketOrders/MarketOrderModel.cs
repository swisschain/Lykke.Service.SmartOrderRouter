using System;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Lykke.Service.SmartOrderRouter.Client.Models.MarketOrders
{
    /// <summary>
    /// Represents a market order.
    /// </summary>
    [PublicAPI]
    public class MarketOrderModel
    {
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
        [JsonConverter(typeof(StringEnumConverter))]
        public MarketOrderStatus Status { get; set; }

        /// <summary>
        /// The order creation date.
        /// </summary>
        public DateTime CreatedDate { get; set; }
    }
}
