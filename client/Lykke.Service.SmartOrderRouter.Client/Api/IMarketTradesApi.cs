using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.SmartOrderRouter.Client.Models.MarketTrades;
using Refit;

namespace Lykke.Service.SmartOrderRouter.Client.Api
{
    /// <summary>
    /// Provides methods for working with market trades.
    /// </summary>
    [PublicAPI]
    public interface IMarketTradesApi
    {
        /// <summary>
        /// Returns a collection of market trades by period.
        /// </summary>
        /// <param name="startDate">The start date of period.</param>
        /// <param name="endDate">The end date of period.</param>
        /// <param name="limit">The maximum number of trades.</param>
        /// <returns>A collection of market trades.</returns>
        [Get("/api/MarketTrades")]
        Task<IReadOnlyList<MarketTradeModel>> GetAsync(DateTime startDate, DateTime endDate, int? limit = null);

        /// <summary>
        /// Returns a collection of market trades by market order id.
        /// </summary>
        /// <param name="marketOrderId">The identifier of market order.</param>
        /// <returns>A collection of market trades.</returns>
        [Get("/api/MarketTrades/MarketOrder/{marketOrderId}")]
        Task<IReadOnlyList<MarketTradeModel>> GetByMarketOrderIdAsync(string marketOrderId);
    }
}
