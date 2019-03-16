using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.SmartOrderRouter.Client.Models.ExternalLimitOrders;
using Refit;

namespace Lykke.Service.SmartOrderRouter.Client.Api
{
    /// <summary>
    /// Provides methods for working with external limit orders.
    /// </summary>
    [PublicAPI]
    public interface IExternalLimitOrdersApi
    {
        /// <summary>
        /// Returns a collection of external limit orders by market order id.
        /// </summary>
        /// <param name="marketOrderId">The identifier of the market order.</param>
        /// <returns>A collection of external limit orders.</returns>
        [Get("/api/ExternalLimitOrders")]
        Task<IReadOnlyList<ExternalLimitOrderModel>> GetByMarketOrderIdAsync(string marketOrderId);
    }
}
