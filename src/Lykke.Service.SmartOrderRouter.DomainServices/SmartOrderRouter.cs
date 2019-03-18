using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Service.MarketInstruments.Client.Models.AssetPairs;
using Lykke.Service.MarketInstruments.Client.Services;
using Lykke.Service.SmartOrderRouter.Domain;
using Lykke.Service.SmartOrderRouter.Domain.Entities.Exchanges;
using Lykke.Service.SmartOrderRouter.Domain.Entities.OrderBooks;
using Lykke.Service.SmartOrderRouter.Domain.Entities.Orders;
using Lykke.Service.SmartOrderRouter.Domain.Exceptions;
using Lykke.Service.SmartOrderRouter.Domain.Services;
using Lykke.Service.SmartOrderRouter.DomainServices.Extensions;

namespace Lykke.Service.SmartOrderRouter.DomainServices
{
    public class SmartOrderRouter : ISmartOrderRouter
    {
        private readonly IMarketOrderService _marketOrderService;
        private readonly IExternalLimitOrderService _externalLimitOrderService;
        private readonly IAggregatedOrderBookService _aggregatedOrderBookService;
        private readonly IExchangeSettingsService _exchangeSettingsService;
        private readonly IMarketInstrumentService _marketInstrumentService;
        private readonly ISettingsService _settingsService;
        private readonly IReadOnlyDictionary<string, IExchange> _exchanges;
        private readonly ILog _log;

        public SmartOrderRouter(
            IMarketOrderService marketOrderService,
            IExternalLimitOrderService externalLimitOrderService,
            IAggregatedOrderBookService aggregatedOrderBookService,
            IExchangeSettingsService exchangeSettingsService,
            IMarketInstrumentService marketInstrumentService,
            ISettingsService settingsService,
            IEnumerable<IExchange> exchanges,
            ILogFactory logFactory)
        {
            _marketOrderService = marketOrderService;
            _externalLimitOrderService = externalLimitOrderService;
            _aggregatedOrderBookService = aggregatedOrderBookService;
            _exchangeSettingsService = exchangeSettingsService;
            _marketInstrumentService = marketInstrumentService;
            _settingsService = settingsService;
            _exchanges = exchanges.ToDictionary(exchange => exchange.Name, exchange => exchange);
            _log = logFactory.CreateLog(this);
        }

        public async Task ExecuteMarketOrdersAsync()
        {
            try
            {
                MarketOrder activeMarketOrder = await GetActiveMarketOrderAsync();

                if (activeMarketOrder != null)
                {
                    await RegisterTradesAsync(activeMarketOrder.Id);

                    if (await CanCompleteMarketOrderAsync(activeMarketOrder))
                    {
                        activeMarketOrder.Status = MarketOrderStatus.Filled;

                        await _marketOrderService.UpdateAsync(activeMarketOrder);

                        _log.InfoWithDetails("Market order executed", activeMarketOrder);
                    }
                }

                MarketOrder nextMarketOrder = await GetNextMarketOrderAsync();

                if (nextMarketOrder != null)
                    await ExecuteActiveMarketOrderAsync(nextMarketOrder);
            }
            catch (Exception exception)
            {
                _log.ErrorWithDetails(exception, "An error occurred while executing market orders");
            }
        }

        private async Task<MarketOrder> GetActiveMarketOrderAsync()
        {
            IReadOnlyList<MarketOrder> marketOrders =
                await _marketOrderService.GetByStatusAsync(MarketOrderStatus.Active);

            return marketOrders
                .OrderBy(o => o.CreatedDate)
                .FirstOrDefault();
        }

        private async Task<MarketOrder> GetNextMarketOrderAsync()
        {
            MarketOrder marketOrder = await GetActiveMarketOrderAsync();

            if (marketOrder != null)
                return marketOrder;

            IReadOnlyList<MarketOrder> marketOrders = await _marketOrderService.GetByStatusAsync(MarketOrderStatus.New);

            marketOrder = marketOrders
                .OrderBy(o => o.CreatedDate)
                .FirstOrDefault();

            if (marketOrder == null)
                return null;

            marketOrder.Status = MarketOrderStatus.Active;

            await _marketOrderService.UpdateAsync(marketOrder);

            return marketOrder;
        }

        private async Task RegisterTradesAsync(string marketOrderId)
        {
            IReadOnlyList<ExternalLimitOrder> externalLimitOrders =
                await _externalLimitOrderService.GetByParentIdAsync(marketOrderId);

            IEnumerable<ExternalLimitOrder> activeExternalLimitOrders =
                externalLimitOrders.Where(o => o.Status == ExternalLimitOrderStatus.Active);

            foreach (ExternalLimitOrder externalLimitOrder in activeExternalLimitOrders)
            {
                if (_exchanges.TryGetValue(externalLimitOrder.Exchange, out IExchange exchange))
                {
                    try
                    {
                        await exchange.CancelLimitOrderAsync(externalLimitOrder.Id);

                        ExternalLimitOrderInfo externalLimitOrderInfo =
                            await exchange.GetLimitOrderInfoAsync(externalLimitOrder.Id);

                        switch (externalLimitOrderInfo.Status)
                        {
                            case ExternalLimitOrderStatus.Filled:
                            case ExternalLimitOrderStatus.PartiallyFilled:
                                externalLimitOrder.Execute(externalLimitOrderInfo);
                                await _externalLimitOrderService.UpdateAsync(externalLimitOrder);
                                break;
                            case ExternalLimitOrderStatus.Cancelled:
                                externalLimitOrder.Cancel();
                                await _externalLimitOrderService.UpdateAsync(externalLimitOrder);
                                break;
                            case ExternalLimitOrderStatus.Active:
                                _log.WarningWithDetails("External limit order not cancelled", externalLimitOrder);
                                break;
                        }
                    }
                    catch (Exception exception)
                    {
                        _log.ErrorWithDetails(exception, "An error occurred while processing external limit order",
                            externalLimitOrder);
                    }
                }
                else
                {
                    _log.WarningWithDetails("Can not process external limit order. Exchange not found.",
                        externalLimitOrder);
                }
            }
        }

        private async Task<bool> CanCompleteMarketOrderAsync(MarketOrder marketOrder)
        {
            IReadOnlyList<ExternalLimitOrder> externalLimitOrders =
                await _externalLimitOrderService.GetByParentIdAsync(marketOrder.Id);

            if (externalLimitOrders.Any(o => o.Status == ExternalLimitOrderStatus.Active))
                return false;

            decimal executedVolume = externalLimitOrders.Sum(o => o.ExecutedVolume ?? 0);

            string exchange = _settingsService.GetExchangeName();

            AssetPairModel assetPairModel = _marketInstrumentService.GetAssetPair(marketOrder.AssetPair, exchange);

            decimal remainingVolume = Math.Round(marketOrder.Volume - executedVolume, assetPairModel.VolumeAccuracy);

            return remainingVolume == 0 || remainingVolume < assetPairModel.MinVolume;
        }

        private async Task ExecuteActiveMarketOrderAsync(MarketOrder marketOrder)
        {
            IReadOnlyList<ExternalLimitOrder> externalLimitOrders =
                await _externalLimitOrderService.GetByParentIdAsync(marketOrder.Id);

            decimal activeVolume = externalLimitOrders
                .Where(o => o.Status == ExternalLimitOrderStatus.Active)
                .Sum(o => o.Volume);

            decimal executedVolume = externalLimitOrders
                .Where(o => o.Status == ExternalLimitOrderStatus.Filled ||
                            o.Status == ExternalLimitOrderStatus.PartiallyFilled)
                .Sum(o => o.Volume);

            decimal remainingVolume = marketOrder.Volume - activeVolume - executedVolume;

            AggregatedOrderBook aggregatedOrderBook = _aggregatedOrderBookService.GetByAssetPair(marketOrder.AssetPair);

            if (aggregatedOrderBook == null)
            {
                _log.WarningWithDetails("Can not execute market order the aggregated order book does not exist",
                    marketOrder);
                return;
            }

            IReadOnlyList<ExchangeVolume> volumes;

            if (marketOrder.Type == OrderType.Sell)
                volumes = aggregatedOrderBook.GetBuyVolumes(remainingVolume);
            else
                volumes = aggregatedOrderBook.GetSellVolumes(remainingVolume);

            if (volumes.Count == 0)
            {
                _log.WarningWithDetails("Can not execute market order no liquidity in aggregated order book",
                    marketOrder);
                return;
            }

            foreach (ExchangeVolume exchangeVolume in volumes)
            {
                ExchangeSettings exchangeSettings =
                    await _exchangeSettingsService.GetByNameAsync(exchangeVolume.Exchange);

                decimal price;

                if (marketOrder.Type == OrderType.Sell)
                    price = exchangeVolume.Price * (1 - exchangeSettings.SlippageMarkup);
                else
                    price = exchangeVolume.Price * (1 + exchangeSettings.SlippageMarkup);

                try
                {
                    await CreateExternalLimitOrderAsync(marketOrder.Id, exchangeVolume.Exchange, marketOrder.AssetPair,
                        price, exchangeVolume.Volume, marketOrder.Type);
                }
                catch (FailedOperationException exception)
                {
                    _log.WarningWithDetails("Can create external limit order. Asset pair settings does not exist.",
                        exception,
                        new
                        {
                            MarketOrderId = marketOrder.Id,
                            exchangeVolume.Exchange,
                            marketOrder.AssetPair,
                            Price = price,
                            exchangeVolume.Volume,
                            OrderType = marketOrder.Type.ToString()
                        });
                }
                catch (Exception exception)
                {
                    _log.ErrorWithDetails(exception, "An error occurred while creating external limit order",
                        new
                        {
                            MarketOrderId = marketOrder.Id,
                            exchangeVolume.Exchange,
                            marketOrder.AssetPair,
                            Price = price,
                            exchangeVolume.Volume,
                            OrderType = marketOrder.Type.ToString()
                        });
                }
            }
        }

        private async Task CreateExternalLimitOrderAsync(string marketOrderId, string exchangeName, string assetPair,
            decimal price, decimal volume, OrderType orderType)
        {
            if (_exchanges.TryGetValue(exchangeName, out IExchange exchange))
            {
                AssetPairModel assetPairModel = _marketInstrumentService.GetAssetPair(assetPair, exchange.Name);

                if (assetPairModel == null)
                    throw new FailedOperationException("Asset pair settings does not exist");

                price = price.TruncateDecimalPlaces(assetPairModel.PriceAccuracy, orderType == OrderType.Sell);

                volume = Math.Round(volume, assetPairModel.VolumeAccuracy);

                if (volume < assetPairModel.MinVolume)
                    throw new FailedOperationException("The volume is less than min volume");

                string orderId = await exchange.CreateLimitOrderAsync(assetPair, price, volume, orderType);

                _log.InfoWithDetails("External order created", new
                {
                    Id = orderId,
                    Exchange = exchangeName,
                    AssetPair = assetPair,
                    Price = price,
                    Volume = volume
                });

                var externalLimitOrder = new ExternalLimitOrder(orderId, exchange.Name, assetPair, price, volume,
                    orderType);

                await _externalLimitOrderService.AddAsync(marketOrderId, externalLimitOrder);
            }
            else
            {
                throw new FailedOperationException("Exchange not found");
            }
        }
    }
}
