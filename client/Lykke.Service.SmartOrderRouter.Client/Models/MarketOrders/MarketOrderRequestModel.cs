using JetBrains.Annotations;

namespace Lykke.Service.SmartOrderRouter.Client.Models.MarketOrders
{
    /// <summary>
    /// Represents a market order creation request.
    /// </summary>
    [PublicAPI]
    public class MarketOrderRequestModel
    {
        /// <summary>
        /// The identifier of the client.
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
    }
}
