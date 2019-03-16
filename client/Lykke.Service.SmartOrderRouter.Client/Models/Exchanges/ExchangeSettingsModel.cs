using System.Collections.Generic;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Lykke.Service.SmartOrderRouter.Client.Models.Exchanges
{
    /// <summary>
    /// Represents an exchange settings.
    /// </summary>
    [PublicAPI]
    public class ExchangeSettingsModel
    {
        /// <summary>
        /// The name of exchange.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Indicates the status of exchange. 
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public ExchangeStatus Status { get; set; }

        /// <summary>
        /// The collection of instruments that can be used on exchange to calculate order books and execute orders.
        /// </summary>
        public IReadOnlyList<string> Instruments { get; set; }

        /// <summary>
        /// The fee in bps of cash-in/out operation on exchange.
        /// </summary>
        public decimal MarketFee { get; set; }

        /// <summary>
        /// The fee in bps of transaction on exchange.
        /// </summary>
        public decimal TransactionFee { get; set; }

        /// <summary>
        /// The markup for reducing order slippage while trading.  
        /// </summary>
        public decimal SlippageMarkup { get; set; }
    }
}
