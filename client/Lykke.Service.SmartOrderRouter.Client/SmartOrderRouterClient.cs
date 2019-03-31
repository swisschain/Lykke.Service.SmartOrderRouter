using Lykke.HttpClientGenerator;
using Lykke.Service.SmartOrderRouter.Client.Api;

namespace Lykke.Service.SmartOrderRouter.Client
{
    /// <inheritdoc/>
    public class SmartOrderRouterClient : ISmartOrderRouterClient
    {
        /// <summary>
        /// Initializes a new instance of <see cref="SmartOrderRouterClient"/> with <param name="httpClientGenerator"></param>.
        /// </summary> 
        public SmartOrderRouterClient(IHttpClientGenerator httpClientGenerator)
        {
            Balances = httpClientGenerator.Generate<IBalancesApi>();
            Exchanges = httpClientGenerator.Generate<IExchangesApi>();
            ExternalLimitOrders = httpClientGenerator.Generate<IExternalLimitOrdersApi>();
            MarketOrders = httpClientGenerator.Generate<IMarketOrdersApi>();
            OrderBooks = httpClientGenerator.Generate<IOrderBooksApi>();
            Quotes = httpClientGenerator.Generate<IQuotesApi>();
            Reports = httpClientGenerator.Generate<IReportsApi>();
            Settings = httpClientGenerator.Generate<ISettingsApi>();
        }

        /// <inheritdoc/>
        public IBalancesApi Balances { get; set; }

        /// <inheritdoc/>
        public IExchangesApi Exchanges { get; set; }

        /// <inheritdoc/>
        public IExternalLimitOrdersApi ExternalLimitOrders { get; set; }

        /// <inheritdoc/>
        public IMarketOrdersApi MarketOrders { get; set; }

        /// <inheritdoc/>
        public IOrderBooksApi OrderBooks { get; set; }

        /// <inheritdoc/>
        public IQuotesApi Quotes { get; set; }

        /// <inheritdoc/>
        public IReportsApi Reports { get; set; }

        /// <inheritdoc/>
        public ISettingsApi Settings { get; set; }
    }
}
