using System;
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
        /// <param name="startDate">The start date of period.</param>
        /// <param name="endDate">The end date of period.</param>
        /// <param name="limit">The number of market orders.</param>
        /// <returns>A collection of market orders.</returns>
        [Get("/api/MarketOrders")]
        Task<IReadOnlyList<MarketOrderModel>> GetAsync(DateTime startDate, DateTime endDate, int? limit);

        /// <summary>
        /// Returns a collection of market orders by client.
        /// </summary>
        /// <param name="clientId">The identifier of the client.</param>
        /// <returns>A collection of market orders.</returns>
        [Get("/api/MarketOrders/client/{clientId}")]
        Task<IReadOnlyList<MarketOrderModel>> GetByClientIdAsync(string clientId);
        
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
        Task CreateAsync([Body] MarketOrderRequestModel model);
    }
}
