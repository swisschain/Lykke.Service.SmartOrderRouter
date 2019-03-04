using JetBrains.Annotations;
using Lykke.Service.SmartOrderRouter.Client.Api;

namespace Lykke.Service.SmartOrderRouter.Client
{
    /// <summary>
    /// Smart order router service client interface.
    /// </summary>
    [PublicAPI]
    public interface ISmartOrderRouterClient
    {
        /// <summary>
        /// Balances API.
        /// </summary>
        IBalancesApi Balances { get; set; }

        /// <summary>
        /// Exchanges API.
        /// </summary>
        IExchangesApi Exchanges { get; set; }

        /// <summary>
        /// Market orders API.
        /// </summary>
        IMarketOrdersApi MarketOrders { get; set; }

        /// <summary>
        /// Market trades API.
        /// </summary>
        IMarketTradesApi MarketTrades { get; set; }

        /// <summary>
        /// Order books API.
        /// </summary>
        IOrderBooksApi OrderBooks { get; set; }

        /// <summary>
        /// Quotes API.
        /// </summary>
        IQuotesApi Quotes { get; set; }
    }
}
