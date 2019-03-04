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
            MarketOrders = httpClientGenerator.Generate<IMarketOrdersApi>();
            MarketTrades = httpClientGenerator.Generate<IMarketTradesApi>();
            OrderBooks = httpClientGenerator.Generate<IOrderBooksApi>();
            Quotes = httpClientGenerator.Generate<IQuotesApi>();
        }

        /// <inheritdoc/>
        public IBalancesApi Balances { get; set; }

        /// <inheritdoc/>
        public IExchangesApi Exchanges { get; set; }

        /// <inheritdoc/>
        public IMarketOrdersApi MarketOrders { get; set; }

        /// <inheritdoc/>
        public IMarketTradesApi MarketTrades { get; set; }

        /// <inheritdoc/>
        public IOrderBooksApi OrderBooks { get; set; }

        /// <inheritdoc/>
        public IQuotesApi Quotes { get; set; }
    }
}
