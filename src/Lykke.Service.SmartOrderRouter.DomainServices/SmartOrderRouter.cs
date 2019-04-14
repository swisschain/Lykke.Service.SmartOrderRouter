using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Service.MarketInstruments.Client.Models.AssetPairs;
using Lykke.Service.MarketInstruments.Client.Services;
using Lykke.Service.SmartOrderRouter.Domain;
using Lykke.Service.SmartOrderRouter.Domain.Entities.Exchanges;
using Lykke.Service.SmartOrderRouter.Domain.Entities.OrderBooks;
using Lykke.Service.SmartOrderRouter.Domain.Entities.Orders;
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
        private readonly IExchangeService _exchangeService;
        private readonly ILog _log;

        public SmartOrderRouter(
            IMarketOrderService marketOrderService,
            IExternalLimitOrderService externalLimitOrderService,
            IAggregatedOrderBookService aggregatedOrderBookService,
            IExchangeSettingsService exchangeSettingsService,
            IMarketInstrumentService marketInstrumentService,
            ISettingsService settingsService,
            IExchangeService exchangeService,
            ILogFactory logFactory)
        {
            _marketOrderService = marketOrderService;
            _externalLimitOrderService = externalLimitOrderService;
            _aggregatedOrderBookService = aggregatedOrderBookService;
            _exchangeSettingsService = exchangeSettingsService;
            _marketInstrumentService = marketInstrumentService;
            _settingsService = settingsService;
            _exchangeService = exchangeService;
            _log = logFactory.CreateLog(this);
        }

        public async Task ExecuteMarketOrdersAsync()
        {
            try
            {
                MarketOrder activeMarketOrder = await _marketOrderService.GetFirstActiveAsync();

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

                MarketOrder nextActiveMarketOrder = await _marketOrderService.GetNextActiveAsync();

                if (nextActiveMarketOrder != null)
                    await ExecuteMarketOrderAsync(nextActiveMarketOrder);
            }
            catch (Exception exception)
            {
                _log.ErrorWithDetails(exception, "An error occurred while executing market orders");
            }
        }

        private async Task RegisterTradesAsync(string marketOrderId)
        {
            IReadOnlyList<ExternalLimitOrder> externalLimitOrders =
                await _externalLimitOrderService.GetByParentIdAsync(marketOrderId);

            IEnumerable<ExternalLimitOrder> activeExternalLimitOrders =
                externalLimitOrders.Where(o => o.Status == ExternalLimitOrderStatus.Active);

            foreach (ExternalLimitOrder externalLimitOrder in activeExternalLimitOrders)
            {
                try
                {
                    if (await _exchangeService.CancelLimitOrderAsync(externalLimitOrder))
                        await _externalLimitOrderService.UpdateAsync(externalLimitOrder);
                    else
                        _log.WarningWithDetails("External limit order not cancelled", externalLimitOrder);
                }
                catch (Exception exception)
                {
                    _log.ErrorWithDetails(exception, "An error occurred while processing external limit order",
                        externalLimitOrder);
                }
            }
        }

        private async Task ExecuteMarketOrderAsync(MarketOrder marketOrder)
        {
            AggregatedOrderBook aggregatedOrderBook = _aggregatedOrderBookService.GetByAssetPair(marketOrder.AssetPair);

            if (aggregatedOrderBook == null)
            {
                _log.WarningWithDetails("Can not execute market order the aggregated order book does not exist",
                    marketOrder);
                return;
            }

            IReadOnlyList<ExternalLimitOrder> externalLimitOrders =
                await _externalLimitOrderService.GetByParentIdAsync(marketOrder.Id);

            List<ExternalLimitOrder> activeLimitOrders = externalLimitOrders
                .Where(o => o.Status == ExternalLimitOrderStatus.Active)
                .ToList();

            decimal executedVolume = externalLimitOrders
                .Where(o => o.Status == ExternalLimitOrderStatus.Filled ||
                            o.Status == ExternalLimitOrderStatus.PartiallyFilled)
                .Sum(o => o.Volume);

            decimal remainingVolume = marketOrder.Volume - executedVolume;

            bool rerouteFailedLimitOrders;

            var excludedExchanges = new List<string>();

            do
            {
                rerouteFailedLimitOrders = false;

                IReadOnlyList<ExchangeVolume> volumes;

                if (marketOrder.Type == OrderType.Sell)
                    volumes = aggregatedOrderBook.GetBuyVolumes(remainingVolume, activeLimitOrders, excludedExchanges);
                else
                    volumes = aggregatedOrderBook.GetSellVolumes(remainingVolume, activeLimitOrders, excludedExchanges);

                if (volumes.Count == 0)
                {
                    _log.WarningWithDetails("Can not execute market order no liquidity in aggregated order book",
                        new
                        {
                            MarketOrder = marketOrder,
                            RemainingVolume = remainingVolume,
                            ActiveVolume = activeLimitOrders.Sum(o => o.Volume)
                        });
                    break;
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
                        ExternalLimitOrder externalLimitOrder =
                            await _exchangeService.CreateLimitOrderAsync(exchangeVolume.Exchange, marketOrder.AssetPair,
                                price, exchangeVolume.Volume, marketOrder.Type);

                        _log.InfoWithDetails("External limit order created", externalLimitOrder);

                        await _externalLimitOrderService.AddAsync(marketOrder.Id, externalLimitOrder);

                        activeLimitOrders.Add(externalLimitOrder);
                    }
                    catch (Exception exception)
                    {
                        rerouteFailedLimitOrders = true;

                        excludedExchanges.Add(exchangeSettings.Name);

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
            } while (rerouteFailedLimitOrders);
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
    }
}
