using JetBrains.Annotations;

namespace Lykke.Service.SmartOrderRouter.Client.Models.ExternalLimitOrders
{
    /// <summary>
    /// Specifies an external limit order status.
    /// </summary>
    [PublicAPI]
    public enum ExternalLimitOrderStatus
    {
        /// <summary>
        /// Unspecified external limit order status.
        /// </summary>
        None,

        /// <summary>
        /// Indicates that the external limit order is not matched.
        /// </summary>
        Active,

        /// <summary>
        /// Indicates that the external limit order partially matched.
        /// </summary>
        PartiallyFilled,

        /// <summary>
        /// Indicates that the external limit order was matched fully.
        /// </summary>
        Filled,

        /// <summary>
        /// Indicates that the external limit order cancelled.
        /// </summary>
        Cancelled
    }
}
