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
    public class InternalOrderBookService : IInternalOrderBookService
    {
        private readonly IExternalOrderBookService _externalOrderBookService;
        private readonly IMarketInstrumentService _marketInstrumentService;
        private readonly ISettingsService _settingsService;

        private readonly ConcurrentDictionary<string, InternalOrderBook> _orderBooks =
            new ConcurrentDictionary<string, InternalOrderBook>();

        public InternalOrderBookService(
            IExternalOrderBookService externalOrderBookService,
            IMarketInstrumentService marketInstrumentService,
            ISettingsService settingsService)
        {
            _externalOrderBookService = externalOrderBookService;
            _marketInstrumentService = marketInstrumentService;
            _settingsService = settingsService;
        }

        public IReadOnlyList<InternalOrderBook> GetAll()
        {
            return _orderBooks.Values.ToList();
        }

        public InternalOrderBook GetByAssetPair(string assetPair)
        {
            _orderBooks.TryGetValue(assetPair, out InternalOrderBook internalOrderBook);

            return internalOrderBook;
        }

        public Task UpdateAsync(string assetPair)
        {
            string exchangeName = _settingsService.GetExchangeName();

            AssetPairModel assetPairSettings = _marketInstrumentService.GetAssetPair(assetPair, exchangeName);

            if (assetPairSettings == null)
                throw new FailedOperationException("Asset pair not found");

            IReadOnlyList<ExternalOrderBook> externalOrderBooks = _externalOrderBookService.GetByAssetPair(assetPair);

            var sellLevels = new Dictionary<decimal, decimal>();

            decimal ask = decimal.MaxValue;

            foreach (ExternalOrderBook externalOrderBook in externalOrderBooks)
            {
                foreach (ExternalOrderBookLevel priceLevel in externalOrderBook.SellLevels)
                {
                    decimal price = priceLevel.Price.TruncateDecimalPlaces(assetPairSettings.PriceAccuracy, true);

                    if (!sellLevels.ContainsKey(price))
                        sellLevels[price] = 0;

                    sellLevels[price] += priceLevel.Volume;

                    if (ask > price)
                        ask = price;
                }
            }

            var buyLevels = new Dictionary<decimal, decimal>();

            decimal bid = decimal.MinValue;

            foreach (ExternalOrderBook externalOrderBook in externalOrderBooks)
            {
                foreach (ExternalOrderBookLevel priceLevel in externalOrderBook.BuyLevels)
                {
                    decimal price = priceLevel.Price.TruncateDecimalPlaces(assetPairSettings.PriceAccuracy);

                    if (!buyLevels.ContainsKey(price))
                        buyLevels[price] = 0;

                    buyLevels[price] += priceLevel.Volume;

                    if (bid < price)
                        bid = price;
                }
            }

            if (ask <= bid)
            {
                decimal mid = Math.Round((ask + bid) / 2, assetPairSettings.PriceAccuracy);

                var basisPoint = (decimal) Math.Pow(.1, assetPairSettings.PriceAccuracy);

                ask = mid + basisPoint;

                bid = mid - basisPoint;
            }

            var sellOrderBookLevels = new List<OrderBookLevel>();

            var bestSellLevel = new OrderBookLevel {Price = ask};

            foreach (var (price, volume) in sellLevels)
            {
                if (price <= ask)
                    bestSellLevel.Volume += volume;
                else
                    sellOrderBookLevels.Add(new OrderBookLevel {Price = price, Volume = volume});
            }

            if (bestSellLevel.Volume > 0)
                sellOrderBookLevels.Add(bestSellLevel);

            var buyOrderBookLevels = new List<OrderBookLevel>();

            var bestBuyLevel = new OrderBookLevel {Price = bid};

            foreach (var (price, volume) in buyLevels)
            {
                if (price >= bid)
                    bestBuyLevel.Volume += volume;
                else
                    buyOrderBookLevels.Add(new OrderBookLevel {Price = price, Volume = volume});
            }

            if (bestBuyLevel.Volume > 0)
                buyOrderBookLevels.Add(bestBuyLevel);

            var internalOrderBook = new InternalOrderBook
            {
                AssetPair = assetPairSettings.Name,
                Timestamp = DateTime.UtcNow,
                SellLevels = sellOrderBookLevels
                    .OrderBy(o => o.Price)
                    .ToList(),
                BuyLevels = buyOrderBookLevels
                    .OrderByDescending(o => o.Price)
                    .ToList()
            };

            _orderBooks.AddOrUpdate(assetPair, internalOrderBook, (key, value) => internalOrderBook);

            return Task.CompletedTask;
        }

        public Task ResetAsync()
        {
            _orderBooks.Clear();

            return Task.CompletedTask;
        }
    }
}
