using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.SmartOrderRouter.Client.Models.MarketOrders;
using Refit;

namespace Lykke.Service.SmartOrderRouter.Client.Api
{
    /// <summary>
    /// Provides methods for working with market orders.
    /// </summary>
    [PublicAPI]
    public interface IMarketOrdersApi
    {
        /// <summary>
        /// Returns a collection of market orders.
        /// </summary>
        /// <param name="clientId">The identifier of the client.</param>
        /// <returns>A collection of market orders.</returns>
        [Get("/api/MarketOrders")]
        Task<IReadOnlyList<MarketOrderModel>> GetAsync(string clientId = null);

        /// <summary>
        /// Returns the market order by identifier.
        /// </summary>
        /// <param name="marketOrderId">The identifier of the market order.</param>
        /// <returns>The market order.</returns>
        [Get("/api/MarketOrders/{marketOrderId}")]
        Task<MarketOrderModel> GetByIdAsync(string marketOrderId);

        /// <summary>
        /// Creates new market order.
        /// </summary>
        /// <param name="model">The model that describes market order creation information.</param>
        [Post("/api/MarketOrders")]
        Task StartAsync([Body] MarketOrderRequestModel model);
    }
}
