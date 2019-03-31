using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Service.MarketInstruments.Client.Models.AssetPairs;
using Lykke.Service.MarketInstruments.Client.Services;
using Lykke.Service.SmartOrderRouter.Domain.Entities.Orders;
using Lykke.Service.SmartOrderRouter.Domain.Exceptions;
using Lykke.Service.SmartOrderRouter.Domain.Repositories;
using Lykke.Service.SmartOrderRouter.Domain.Services;
using Lykke.Service.SmartOrderRouter.DomainServices.Extensions;

namespace Lykke.Service.SmartOrderRouter.DomainServices.Orders
{
    public class MarketOrderService : IMarketOrderService
    {
        private readonly IMarketOrderRepository _marketOrderRepository;
        private readonly IMarketInstrumentService _marketInstrumentService;
        private readonly ISettingsService _settingsService;
        private readonly ILog _log;

        public MarketOrderService(
            IMarketOrderRepository marketOrderRepository,
            IMarketInstrumentService marketInstrumentService,
            ISettingsService settingsService,
            ILogFactory logFactory)
        {
            _marketOrderRepository = marketOrderRepository;
            _marketInstrumentService = marketInstrumentService;
            _settingsService = settingsService;
            _log = logFactory.CreateLog(this);
        }

        public Task<MarketOrder> GetByIdAsync(string marketOrderId)
        {
            return _marketOrderRepository.GetByIdAsync(marketOrderId);
        }

        public Task<IReadOnlyList<MarketOrder>> GetByStatusAsync(MarketOrderStatus marketOrderStatus)
        {
            return _marketOrderRepository.GetByStatusAsync(marketOrderStatus);
        }

        public Task<IReadOnlyList<MarketOrder>> GetByClientIdAsync(string clientId)
        {
            return _marketOrderRepository.GetByClientIdAsync(clientId);
        }

        public Task<IReadOnlyList<MarketOrder>> GetAllAsync(DateTime startDate, DateTime endDate, int? limit)
        {
            return _marketOrderRepository.GetAllAsync(startDate, endDate, limit);
        }

        public async Task CreateAsync(string clientId, string assetPair, decimal volume, OrderType orderType)
        {
            string exchangeName = _settingsService.GetExchangeName();

            AssetPairModel assetPairSettings = _marketInstrumentService.GetAssetPair(assetPair, exchangeName);
            
            if(assetPairSettings == null)
                throw new FailedOperationException("Asset pair not supported");

            if(volume < assetPairSettings.MinVolume)
                throw new FailedOperationException("Volume is too small");
            
            var marketOrder = new MarketOrder(clientId, assetPair, orderType, volume);

            await _marketOrderRepository.InsertAsync(marketOrder);
            
            _log.InfoWithDetails("Market order created", marketOrder);
        }

        public Task UpdateAsync(MarketOrder marketOrder)
        {
            return _marketOrderRepository.UpdateAsync(marketOrder);
        }
    }
}
