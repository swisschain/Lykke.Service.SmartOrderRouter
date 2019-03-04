using System;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Lykke.Service.SmartOrderRouter.Client.Models.MarketTrades
{
    /// <summary>
    /// Represents a market trade.
    /// </summary>
    [PublicAPI]
    public class MarketTradeModel
    {
        /// <summary>
        /// The identifier of the trade.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The identifier of the market order which executed while trade.
        /// </summary>
        public string MarketOrderId { get; set; }

        /// <summary>
        /// The name of the exchange.
        /// </summary>
        public string Exchange { get; set; }

        /// <summary>
        /// The asset pair.
        /// </summary>
        public string AssetPairId { get; set; }

        /// <summary>
        /// The type of the trade.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public TradeType Type { get; set; }

        /// <summary>
        /// The time of the trade.
        /// </summary>
        public DateTime Time { get; set; }

        /// <summary>
        /// The price of the trade.
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// The volume of the trade.
        /// </summary>
        public decimal Volume { get; set; }
    }
}
