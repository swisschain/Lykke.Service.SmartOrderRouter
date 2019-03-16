using JetBrains.Annotations;

namespace Lykke.Service.SmartOrderRouter.Client.Models
{
    /// <summary>
    /// Specifies a type of a trade.
    /// </summary>
    [PublicAPI]
    public enum TradeType
    {
        /// <summary>
        /// Unspecified type.
        /// </summary>
        None,

        /// <summary>
        /// Indicates that buy order was executed while trade.
        /// </summary>
        Buy,

        /// <summary>
        /// Indicates that sell order was executed while trade.
        /// </summary>
        Sell
    }
}
