using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Lykke.Service.MarketInstruments.Client.Models.AssetPairs;
using Lykke.Service.MarketInstruments.Client.Services;
using Lykke.Service.SmartOrderRouter.Domain.Entities.Balances;
using Lykke.Service.SmartOrderRouter.Domain.Entities.OrderBooks;
using Lykke.Service.SmartOrderRouter.Domain.Exceptions;
using Lykke.Service.SmartOrderRouter.Domain.Services;

namespace Lykke.Service.SmartOrderRouter.DomainServices.OrderBooks
{
    public class AggregatedOrderBookService : IAggregatedOrderBookService
    {
        private readonly IExternalOrderBookService _externalOrderBookService;
        private readonly IMarketInstrumentService _marketInstrumentService;
        private readonly IBalanceService _balanceService;
        private readonly ISettingsService _settingsService;

        private readonly ConcurrentDictionary<string, AggregatedOrderBook> _orderBooks =
            new ConcurrentDictionary<string, AggregatedOrderBook>();

        public AggregatedOrderBookService(
            IExternalOrderBookService externalOrderBookService,
            IMarketInstrumentService marketInstrumentService,
            IBalanceService balanceService,
            ISettingsService settingsService)
        {
            _externalOrderBookService = externalOrderBookService;
            _marketInstrumentService = marketInstrumentService;
            _balanceService = balanceService;
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

        public async Task UpdateAsync(string assetPair)
        {
            string exchangeName = _settingsService.GetExchangeName();

            AssetPairModel assetPairSettings = _marketInstrumentService.GetAssetPair(assetPair, exchangeName);

            if (assetPairSettings == null)
                throw new FailedOperationException("Asset pair not found");

            IReadOnlyList<ExternalOrderBook> externalOrderBooks = _externalOrderBookService.GetByAssetPair(assetPair);

            Task<Dictionary<decimal, Dictionary<string, ExternalOrderBookLevel>>> sellLevelsTask =
                GetSellLevelsAsync(externalOrderBooks, assetPairSettings);

            Task<Dictionary<decimal, Dictionary<string, ExternalOrderBookLevel>>> buyLevelsTask =
                GetBuyLevelsAsync(externalOrderBooks, assetPairSettings);

            await Task.WhenAll(sellLevelsTask, buyLevelsTask);

            var aggregatedOrderBook = new AggregatedOrderBook
            {
                AssetPair = assetPair,
                Timestamp = DateTime.UtcNow,
                SellLevels = sellLevelsTask.Result
                    .Select(priceLevel => new AggregatedOrderBookLevel
                    {
                        Price = priceLevel.Key,
                        Volume = priceLevel.Value.Sum(exchangeVolume => exchangeVolume.Value.Volume),
                        ExchangeVolumes = priceLevel.Value
                            .Select(o => new AggregatedOrderBookVolume(o.Key, o.Value.OriginalPrice, o.Value.Volume))
                            .ToList()
                    })
                    .OrderBy(o => o.Price)
                    .ToList(),
                BuyLevels = buyLevelsTask.Result
                    .Select(priceLevel => new AggregatedOrderBookLevel
                    {
                        Price = priceLevel.Key,
                        Volume = priceLevel.Value.Sum(exchangeVolume => exchangeVolume.Value.Volume),
                        ExchangeVolumes = priceLevel.Value
                            .Select(o => new AggregatedOrderBookVolume(o.Key, o.Value.OriginalPrice, o.Value.Volume))
                            .ToList()
                    })
                    .OrderByDescending(o => o.Price)
                    .ToList()
            };

            _orderBooks.AddOrUpdate(assetPair, aggregatedOrderBook, (key, value) => aggregatedOrderBook);
        }

        public Task ResetAsync()
        {
            _orderBooks.Clear();

            return Task.CompletedTask;
        }

        private Task<Dictionary<decimal, Dictionary<string, ExternalOrderBookLevel>>> GetSellLevelsAsync(
            IEnumerable<ExternalOrderBook> externalOrderBooks, AssetPairModel assetPairSettings)
        {
            var sellLevels = new Dictionary<decimal, Dictionary<string, ExternalOrderBookLevel>>();

            foreach (ExternalOrderBook externalOrderBook in externalOrderBooks)
            {
                Balance balance = _balanceService.Get(externalOrderBook.Exchange, assetPairSettings.BaseAsset);

                int index = 0;
                decimal amount = balance.Amount;

                List<ExternalOrderBookLevel> orderBookLevels = externalOrderBook.SellLevels
                    .OrderBy(o => o.Price)
                    .ToList();

                while (amount > 0 && index < orderBookLevels.Count)
                {
                    ExternalOrderBookLevel priceLevel = orderBookLevels[index++];

                    decimal volume = priceLevel.Volume;

                    if (amount < volume)
                    {
                        volume = amount;
                        amount = 0;
                    }
                    else
                    {
                        amount -= volume;
                    }

                    decimal price = priceLevel.Price.TruncateDecimalPlaces(assetPairSettings.PriceAccuracy, true);

                    if (!sellLevels.TryGetValue(price, out Dictionary<string, ExternalOrderBookLevel> sellLevel))
                        sellLevels.Add(price, sellLevel = new Dictionary<string, ExternalOrderBookLevel>());

                    sellLevel[externalOrderBook.Exchange] = new ExternalOrderBookLevel(priceLevel.Price,
                        volume, priceLevel.Markup, priceLevel.OriginalPrice);
                }
            }

            return Task.FromResult(sellLevels);
        }

        private Task<Dictionary<decimal, Dictionary<string, ExternalOrderBookLevel>>> GetBuyLevelsAsync(
            IEnumerable<ExternalOrderBook> externalOrderBooks, AssetPairModel assetPairSettings)
        {
            var buyLevels = new Dictionary<decimal, Dictionary<string, ExternalOrderBookLevel>>();

            foreach (ExternalOrderBook externalOrderBook in externalOrderBooks)
            {
                Balance balance = _balanceService.Get(externalOrderBook.Exchange, assetPairSettings.QuoteAsset);

                int index = 0;
                decimal amount = balance.Amount;

                List<ExternalOrderBookLevel> orderBookLevels = externalOrderBook.BuyLevels
                    .OrderByDescending(o => o.Price)
                    .ToList();

                while (amount > 0 && index < orderBookLevels.Count)
                {
                    ExternalOrderBookLevel priceLevel = orderBookLevels[index++];

                    decimal volume = priceLevel.Volume;

                    if (amount < volume * priceLevel.Price)
                    {
                        AssetPairModel exchangeAssetPairSettings =
                            _marketInstrumentService.GetAssetPair(externalOrderBook.AssetPair,
                                externalOrderBook.Exchange);

                        volume = Math.Round(amount / priceLevel.Price, exchangeAssetPairSettings.VolumeAccuracy);
                        amount = 0;
                    }
                    else
                    {
                        amount -= volume * priceLevel.Price;
                    }

                    decimal price = priceLevel.Price.TruncateDecimalPlaces(assetPairSettings.PriceAccuracy);

                    if (!buyLevels.TryGetValue(price, out Dictionary<string, ExternalOrderBookLevel> buyLevel))
                        buyLevels.Add(price, buyLevel = new Dictionary<string, ExternalOrderBookLevel>());

                    buyLevel[externalOrderBook.Exchange] = new ExternalOrderBookLevel(priceLevel.Price,
                        volume, priceLevel.Markup, priceLevel.OriginalPrice);
                }
            }

            return Task.FromResult(buyLevels);
        }
    }
}
