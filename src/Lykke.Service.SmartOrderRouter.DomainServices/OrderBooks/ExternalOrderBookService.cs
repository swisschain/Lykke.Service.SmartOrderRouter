using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Lykke.Service.MarketInstruments.Client.Models.AssetPairs;
using Lykke.Service.MarketInstruments.Client.Services;
using Lykke.Service.SmartOrderRouter.Domain.Entities.Exchanges;
using Lykke.Service.SmartOrderRouter.Domain.Entities.OrderBooks;
using Lykke.Service.SmartOrderRouter.Domain.Exceptions;
using Lykke.Service.SmartOrderRouter.Domain.Services;

namespace Lykke.Service.SmartOrderRouter.DomainServices.OrderBooks
{
    public class ExternalOrderBookService : IExternalOrderBookService
    {
        private readonly object _sync = new object();

        private readonly IExchangeSettingsService _exchangeSettingsService;
        private readonly IMarketInstrumentService _marketInstrumentService;

        private readonly Dictionary<string, Dictionary<string, ExternalOrderBook>> _orderBooks =
            new Dictionary<string, Dictionary<string, ExternalOrderBook>>();

        public ExternalOrderBookService(
            IExchangeSettingsService exchangeSettingsService,
            IMarketInstrumentService marketInstrumentService)
        {
            _exchangeSettingsService = exchangeSettingsService;
            _marketInstrumentService = marketInstrumentService;
        }

        public IReadOnlyList<ExternalOrderBook> Filter(string exchange, string assetPair)
        {
            var externalOrderBooks = new List<ExternalOrderBook>();

            lock (_sync)
            {
                foreach (Dictionary<string, ExternalOrderBook> orderBooks in _orderBooks.Values)
                {
                    IEnumerable<ExternalOrderBook> list = orderBooks.Values;

                    if (!string.IsNullOrEmpty(exchange))
                        list = list.Where(o => o.Exchange == exchange);

                    if (!string.IsNullOrEmpty(assetPair))
                        list = list.Where(o => o.AssetPair == assetPair);

                    externalOrderBooks.AddRange(list);
                }
            }

            return externalOrderBooks;
        }

        public IReadOnlyList<ExternalOrderBook> GetByAssetPair(string assetPair)
        {
            var externalOrderBooks = new List<ExternalOrderBook>();

            lock (_sync)
            {
                foreach (Dictionary<string, ExternalOrderBook> orderBooks in _orderBooks.Values)
                {
                    if (orderBooks.TryGetValue(assetPair, out ExternalOrderBook externalOrderBook))
                        externalOrderBooks.Add(externalOrderBook);
                }
            }

            return externalOrderBooks;
        }

        public async Task UpdateAsync(string exchange, string assetPair, DateTime timestamp,
            IReadOnlyList<OrderBookLevel> sellLevels, IReadOnlyList<OrderBookLevel> buyLevels)
        {
            ExchangeSettings exchangeSettings = await _exchangeSettingsService.GetByNameAsync(exchange);

            if (exchangeSettings == null)
                throw new FailedOperationException("Exchange setting not found");

            AssetPairModel assetPairSettings = _marketInstrumentService.GetAssetPair(assetPair, exchange);

            if (assetPairSettings == null)
                throw new FailedOperationException("Asset pair not found");

            var externalOrderBook = new ExternalOrderBook
            {
                Exchange = exchange,
                AssetPair = assetPair,
                Timestamp = timestamp,
                SellLevels = sellLevels.Select(o => new ExternalOrderBookLevel
                    {
                        Price = (o.Price * (1 + exchangeSettings.MarketFee + exchangeSettings.TransactionFee))
                            .TruncateDecimalPlaces(assetPairSettings.PriceAccuracy, true),
                        Volume = o.Volume,
                        Markup = exchangeSettings.MarketFee + exchangeSettings.TransactionFee,
                        OriginalPrice = o.Price
                    })
                    .OrderBy(o => o.Price)
                    .ToList(),
                BuyLevels = buyLevels.Select(o => new ExternalOrderBookLevel
                    {
                        Price = (o.Price * (1 - exchangeSettings.MarketFee - exchangeSettings.TransactionFee))
                            .TruncateDecimalPlaces(assetPairSettings.PriceAccuracy),
                        Volume = o.Volume,
                        Markup = exchangeSettings.MarketFee + exchangeSettings.TransactionFee,
                        OriginalPrice = o.Price
                    })
                    .OrderByDescending(o => o.Price)
                    .ToList()
            };

            lock (_sync)
            {
                if (!_orderBooks.ContainsKey(exchange))
                    _orderBooks[exchange] = new Dictionary<string, ExternalOrderBook>();

                _orderBooks[exchange][assetPair] = externalOrderBook;
            }
        }

        public void Remove(string exchange)
        {
            lock (_sync)
                _orderBooks.Remove(exchange);
        }
    }
}
