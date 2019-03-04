using JetBrains.Annotations;

namespace Lykke.Service.SmartOrderRouter.Client.Models.Exchanges
{
    /// <summary>
    /// Specifies an exchange status.
    /// </summary>
    [PublicAPI]
    public enum ExchangeStatus
    {
        /// <summary>
        /// Unspecified status.
        /// </summary>
        None,
        
        /// <summary>
        /// Indicates that the exchange is active and can be used to execute orders.
        /// </summary>
        Active,
        
        /// <summary>
        /// Indicates that the exchange is stopped and not used to calculate order books.
        /// </summary>
        Stopped
    }
}
