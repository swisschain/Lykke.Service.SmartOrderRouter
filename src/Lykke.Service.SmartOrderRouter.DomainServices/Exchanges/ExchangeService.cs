using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Lykke.Service.MarketInstruments.Client.Models.AssetPairs;
using Lykke.Service.MarketInstruments.Client.Services;
using Lykke.Service.SmartOrderRouter.Domain;
using Lykke.Service.SmartOrderRouter.Domain.Entities.Balances;
using Lykke.Service.SmartOrderRouter.Domain.Entities.Orders;
using Lykke.Service.SmartOrderRouter.Domain.Exceptions;
using Lykke.Service.SmartOrderRouter.Domain.Services;

namespace Lykke.Service.SmartOrderRouter.DomainServices.Exchanges
{
    public class ExchangeService : IExchangeService
    {
        private readonly IMarketInstrumentService _marketInstrumentService;
        private readonly IReadOnlyDictionary<string, IExchange> _exchanges;

        public ExchangeService(
            IMarketInstrumentService marketInstrumentService,
            IEnumerable<IExchange> exchanges)
        {
            _marketInstrumentService = marketInstrumentService;
            _exchanges = exchanges.ToDictionary(exchange => exchange.Name, exchange => exchange);
        }

        public IReadOnlyList<string> GetExchanges()
            => _exchanges.Keys.ToList();

        public Task<IReadOnlyList<Balance>> GetBalancesAsync(string exchangeName)
            => GetExchange(exchangeName).GetBalancesAsync();

        public async Task<ExternalLimitOrder> CreateLimitOrderAsync(string exchangeName, string assetPair,
            decimal price, decimal volume, OrderType orderType)
        {
            IExchange exchange = GetExchange(exchangeName);

            AssetPairModel assetPairModel = _marketInstrumentService.GetAssetPair(assetPair, exchange.Name);

            if (assetPairModel == null)
                throw new FailedOperationException("Asset pair settings does not exist");

            price = price.TruncateDecimalPlaces(assetPairModel.PriceAccuracy, orderType == OrderType.Sell);

            volume = Math.Round(volume, assetPairModel.VolumeAccuracy);

            if (volume < assetPairModel.MinVolume)
                throw new FailedOperationException("The volume is too small");

            string orderId = await exchange.CreateLimitOrderAsync(assetPair, price, volume, orderType);

            var externalLimitOrder =
                new ExternalLimitOrder(orderId, exchange.Name, assetPair, price, volume, orderType);

            return externalLimitOrder;
        }

        public async Task<bool> CancelLimitOrderAsync(ExternalLimitOrder externalLimitOrder)
        {
            IExchange exchange = GetExchange(externalLimitOrder.Exchange);

            ExternalLimitOrderInfo externalLimitOrderInfo =
                await exchange.GetLimitOrderInfoAsync(externalLimitOrder.Id);

            switch (externalLimitOrderInfo.Status)
            {
                case ExternalLimitOrderStatus.Filled:
                case ExternalLimitOrderStatus.PartiallyFilled:
                    externalLimitOrder.Execute(externalLimitOrderInfo);
                    return true;
                case ExternalLimitOrderStatus.Cancelled:
                    externalLimitOrder.Cancel();
                    return true;
                case ExternalLimitOrderStatus.Active:
                    return false;
                default:
                    throw new InvalidEnumArgumentException(nameof(externalLimitOrderInfo.Status),
                        (int) externalLimitOrderInfo.Status, typeof(ExternalLimitOrderStatus));
            }
        }

        private IExchange GetExchange(string exchangeName)
        {
            if (!_exchanges.TryGetValue(exchangeName, out IExchange exchange))
                throw new FailedOperationException("Unknown exchange");

            return exchange;
        }
    }
}
