using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Common.ExchangeAdapter.Client;
using Lykke.Common.ExchangeAdapter.Contracts;
using Lykke.Common.ExchangeAdapter.SpotController;
using Lykke.Common.ExchangeAdapter.SpotController.Records;
using Lykke.Service.MarketInstruments.Client.Models.AssetPairs;
using Lykke.Service.MarketInstruments.Client.Models.Assets;
using Lykke.Service.MarketInstruments.Client.Services;
using Lykke.Service.SmartOrderRouter.Domain;
using Lykke.Service.SmartOrderRouter.Domain.Entities.Balances;
using Lykke.Service.SmartOrderRouter.Domain.Entities.Orders;
using Lykke.Service.SmartOrderRouter.Domain.Exceptions;

namespace Lykke.Service.SmartOrderRouter.DomainServices.Exchanges
{
    public class Exchange : IExchange
    {
        private readonly IMarketInstrumentService _marketInstrumentService;
        private readonly ISpotController _client;

        public Exchange(
            string name,
            IExchangeAdapterClientFactory factory,
            IMarketInstrumentService marketInstrumentService)
        {
            Name = name;
            _client = factory.GetSpotController(name);
            _marketInstrumentService = marketInstrumentService;
        }

        public string Name { get; }

        public async Task<IReadOnlyList<Balance>> GetBalancesAsync()
        {
            try
            {
                GetWalletsResponse response = await _client.GetWalletBalancesAsync();

                return response.Wallets
                    .GroupBy(o => o.Asset)
                    .Select(o =>
                        new Balance(Name, TryGetAssetName(o.Key), o.Sum(e => e.Balance), o.Sum(e => e.Reserved)))
                    .ToList();
            }
            catch (Exception exception)
            {
                throw new Exception("An error occurred while getting balances", exception);
            }
        }

        public async Task<ExternalLimitOrderInfo> GetLimitOrderInfoAsync(string limitOrderId)
        {
            try
            {
                OrderModel order = await _client.LimitOrderStatusAsync(limitOrderId);

                if (order == null)
                    throw new FailedOperationException("External order not found");

                return new ExternalLimitOrderInfo
                {
                    Id = order.Id,
                    AssetPair = GetAssetPairName(order.AssetPair),
                    Price = order.Price,
                    Volume = order.OriginalVolume,
                    Type = order.TradeType == TradeType.Sell
                        ? OrderType.Sell
                        : OrderType.Buy,
                    Status = GetStatus(order),
                    ExecutedVolume = order.ExecutedVolume,
                    ExecutedPrice = order.AvgExecutionPrice
                };
            }
            catch (FailedOperationException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw new FailedOperationException("An error occurred while cancelling limit order", exception);
            }
        }

        public async Task<string> CreateLimitOrderAsync(string assetPair, decimal price, decimal volume,
            OrderType orderType)
        {
            try
            {
                OrderIdResponse response = await _client.CreateLimitOrderAsync(new LimitOrderRequest
                {
                    Instrument = GetAssetPairId(assetPair),
                    TradeType = orderType == OrderType.Sell
                        ? TradeType.Sell
                        : TradeType.Buy,
                    Price = price,
                    Volume = volume
                });

                return response.OrderId;
            }
            catch (FailedOperationException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw new Exception("An error occurred while creating limit order", exception);
            }
        }

        public async Task CancelLimitOrderAsync(string limitOrderId)
        {
            try
            {
                await _client.CancelLimitOrderAsync(new CancelLimitOrderRequest {OrderId = limitOrderId});
            }
            catch (Exception exception)
            {
                throw new Exception("An error occurred while cancelling limit order", exception);
            }
        }

        private string GetAssetPairId(string assetPair)
        {
            AssetPairModel assetPairModel = _marketInstrumentService.GetAssetPair(assetPair, Name);

            if (assetPairModel == null)
                throw new FailedOperationException("Asset pair does not exist");

            return assetPairModel.Id;
        }

        private string GetAssetPairName(string assetPairId)
        {
            AssetPairModel assetPairModel = _marketInstrumentService.GetAssetPairs()
                .SingleOrDefault(o => o.Exchange == Name && o.Id == assetPairId);

            if (assetPairModel == null)
                throw new FailedOperationException("Asset pair does not exist");

            return assetPairModel.Name;
        }

        private string TryGetAssetName(string assetId)
        {
            AssetModel assetModel = _marketInstrumentService.GetAssets()
                .SingleOrDefault(o => o.Exchange == Name && o.Id == assetId);

            return assetModel?.Name ?? assetId;
        }

        private static ExternalLimitOrderStatus GetStatus(OrderModel order)
        {
            switch (order.ExecutionStatus)
            {
                case OrderStatus.Active:
                    return ExternalLimitOrderStatus.Active;

                case OrderStatus.Fill:
                    return ExternalLimitOrderStatus.Filled;

                case OrderStatus.Canceled:
                    return order.ExecutedVolume > 0
                        ? ExternalLimitOrderStatus.PartiallyFilled
                        : ExternalLimitOrderStatus.Cancelled;

                default:
                    throw new FailedOperationException("Unknown status");
            }
        }
    }
}
