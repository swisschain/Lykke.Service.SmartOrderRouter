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
        /// External limit orders API.
        /// </summary>
        IExternalLimitOrdersApi ExternalLimitOrders { get; set; }

        /// <summary>
        /// Market orders API.
        /// </summary>
        IMarketOrdersApi MarketOrders { get; set; }

        /// <summary>
        /// Order books API.
        /// </summary>
        IOrderBooksApi OrderBooks { get; set; }

        /// <summary>
        /// Quotes API.
        /// </summary>
        IQuotesApi Quotes { get; set; }
        
        /// <summary>
        /// Reports API.
        /// </summary>
        IReportsApi Reports { get; set; }

        /// <summary>
        /// Settings API.
        /// </summary>
        ISettingsApi Settings { get; set; }
    }
}
