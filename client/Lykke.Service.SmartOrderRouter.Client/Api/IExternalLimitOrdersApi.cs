using System;
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
        /// Returns a collection of external limit orders.
        /// </summary>
        /// <param name="marketOrderId">The identifier of the market order.</param>
        /// <param name="status">The status of the external limit order.</param>
        /// <param name="startDate">The start date of period.</param>
        /// <param name="endDate">The end date of period.</param>
        /// <param name="limit">The number of market orders.</param>
        /// <returns>A collection of external limit orders.</returns>
        [Get("/api/ExternalLimitOrders")]
        Task<IReadOnlyList<ExternalLimitOrderModel>> GetAsync(string marketOrderId,
            ExternalLimitOrderStatus? status, DateTime? startDate, DateTime? endDate, int? limit);
    }
}
