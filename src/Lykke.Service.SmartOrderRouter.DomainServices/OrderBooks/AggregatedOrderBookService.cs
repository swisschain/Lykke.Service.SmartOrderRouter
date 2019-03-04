using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Lykke.Service.MarketInstruments.Client.Models.AssetPairs;
using Lykke.Service.MarketInstruments.Client.Services;
using Lykke.Service.SmartOrderRouter.Domain.Entities.OrderBooks;
using Lykke.Service.SmartOrderRouter.Domain.Exceptions;
using Lykke.Service.SmartOrderRouter.Domain.Services;

namespace Lykke.Service.SmartOrderRouter.DomainServices.OrderBooks
{
    public class AggregatedOrderBookService : IAggregatedOrderBookService
    {
        private readonly IExternalOrderBookService _externalOrderBookService;
        private readonly IMarketInstrumentService _marketInstrumentService;
        private readonly ISettingsService _settingsService;

        private readonly ConcurrentDictionary<string, AggregatedOrderBook> _orderBooks =
            new ConcurrentDictionary<string, AggregatedOrderBook>();

        public AggregatedOrderBookService(
            IExternalOrderBookService externalOrderBookService,
            IMarketInstrumentService marketInstrumentService,
            ISettingsService settingsService)
        {
            _externalOrderBookService = externalOrderBookService;
            _marketInstrumentService = marketInstrumentService;
            _settingsService = settingsService;
        }

        public IReadOnlyList<AggregatedOrderBook> GetAll()
        {
            return _orderBooks.Values.ToList();
        }

        public AggregatedOrderBook GetByAssetPair(string assetPair)
        {
            _orderBooks.TryGetValue(assetPair, out AggregatedOrderBook aggregatedOrderBook);

            return aggregatedOrderBook;
        }

        public Task UpdateAsync(string assetPair)
        {
            string exchangeName = _settingsService.GetExchangeName();

            AssetPairModel assetPairSettings = _marketInstrumentService.GetAssetPair(assetPair, exchangeName);

            if (assetPairSettings == null)
                throw new FailedOperationException("Asset pair not found");

            IReadOnlyList<ExternalOrderBook> externalOrderBooks = _externalOrderBookService.GetByAssetPair(assetPair);

            var sellLevels = new Dictionary<decimal, Dictionary<string, decimal>>();

            foreach (ExternalOrderBook externalOrderBook in externalOrderBooks)
            {
                foreach (ExternalOrderBookLevel priceLevel in externalOrderBook.SellLevels)
                {
                    decimal price = priceLevel.Price.TruncateDecimalPlaces(assetPairSettings.PriceAccuracy, true);

                    if (!sellLevels.ContainsKey(price))
                        sellLevels[price] = new Dictionary<string, decimal>();

                    sellLevels[price][externalOrderBook.Exchange] = priceLevel.Volume;
                }
            }

            var buyLevels = new Dictionary<decimal, Dictionary<string, decimal>>();

            foreach (ExternalOrderBook externalOrderBook in externalOrderBooks)
            {
                foreach (ExternalOrderBookLevel priceLevel in externalOrderBook.BuyLevels)
                {
                    decimal price = priceLevel.Price.TruncateDecimalPlaces(assetPairSettings.PriceAccuracy);

                    if (!buyLevels.ContainsKey(price))
                        buyLevels[price] = new Dictionary<string, decimal>();

                    buyLevels[price][externalOrderBook.Exchange] = priceLevel.Volume;
                }
            }

            var aggregatedOrderBook = new AggregatedOrderBook
            {
                AssetPair = assetPair,
                Timestamp = DateTime.UtcNow,
                SellLevels = sellLevels
                    .Select(priceLevel => new AggregatedOrderBookLevel
                    {
                        Price = priceLevel.Key,
                        Volume = priceLevel.Value.Sum(exchangeVolume => exchangeVolume.Value),
                        ExchangeVolumes = priceLevel.Value
                    })
                    .OrderBy(o => o.Price)
                    .ToList(),
                BuyLevels = buyLevels
                    .Select(priceLevel => new AggregatedOrderBookLevel
                    {
                        Price = priceLevel.Key,
                        Volume = priceLevel.Value.Sum(exchangeVolume => exchangeVolume.Value),
                        ExchangeVolumes = priceLevel.Value
                    })
                    .OrderByDescending(o => o.Price)
                    .ToList()
            };

            _orderBooks.AddOrUpdate(assetPair, aggregatedOrderBook, (key, value) => aggregatedOrderBook);

            return Task.CompletedTask;
        }

        public Task ResetAsync()
        {
            _orderBooks.Clear();

            return Task.CompletedTask;
        }
    }
}
