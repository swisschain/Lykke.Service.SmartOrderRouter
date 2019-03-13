namespace Lykke.Service.SmartOrderRouter.Domain.Entities.Orders
{
    /// <summary>
    /// Specifies an order type.
    /// </summary>
    public enum OrderType
    {
        /// <summary>
        /// Unspecified order type.
        /// </summary>
        None,

        /// <summary>
        /// Buy order type.
        /// </summary>
        Buy,

        /// <summary>
        /// Sell order type.
        /// </summary>
        Sell
    }
}
