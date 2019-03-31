using System.Collections.Generic;
using Lykke.Service.SmartOrderRouter.Domain.Entities;
using Lykke.Service.SmartOrderRouter.Domain.Services;
using Lykke.Service.SmartOrderRouter.DomainServices.Cache;

namespace Lykke.Service.SmartOrderRouter.DomainServices.Quotes
{
    public class QuoteService : IQuoteService
    {
        private readonly InMemoryCache<Quote> _cache;

        public QuoteService()
        {
            _cache = new InMemoryCache<Quote>(GetKey, true);
        }

        public IReadOnlyList<Quote> GetAll()
        {
            return _cache.GetAll();
        }

        public Quote GetByAssetPair(string exchange, string assetPair)
        {
            return _cache.Get(GetKey(exchange, assetPair));
        }

        public void Update(Quote quote)
        {
            _cache.Set(quote);
        }

        private static string GetKey(Quote quote)
            => GetKey(quote.Exchange, quote.AssetPair);

        private static string GetKey(string exchange, string assetPair)
            => $"{exchange}-{assetPair}";
    }
}
