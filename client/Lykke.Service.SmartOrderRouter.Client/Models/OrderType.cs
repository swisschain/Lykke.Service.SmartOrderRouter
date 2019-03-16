using JetBrains.Annotations;

namespace Lykke.Service.SmartOrderRouter.Client.Models
{
    /// <summary>
    /// Specifies an order type.
    /// </summary>
    [PublicAPI]
    public enum OrderType
    {
        /// <summary>
        /// Unspecified order type.
        /// </summary>
        None,

        /// <summary>
        /// Indicates buy order.
        /// </summary>
        Buy,

        /// <summary>
        /// Indicates sell order.
        /// </summary>
        Sell
    }
}
