using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Service.SmartOrderRouter.Domain.Entities.Exchanges;
using Lykke.Service.SmartOrderRouter.Domain.Entities.OrderBooks;
using Lykke.Service.SmartOrderRouter.Domain.Services;
using Lykke.Service.SmartOrderRouter.DomainServices.Extensions;

namespace Lykke.Service.SmartOrderRouter.DomainServices.OrderBooks
{
    public class OrderBookService : IOrderBookService
    {
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        private readonly IAggregatedOrderBookService _aggregatedOrderBookService;
        private readonly IInternalOrderBookService _internalOrderBookService;
        private readonly IExternalOrderBookService _externalOrderBookService;
        private readonly IExchangeSettingsService _exchangeSettingsService;

        private readonly ILog _log;

        public OrderBookService(
            IAggregatedOrderBookService aggregatedOrderBookService,
            IInternalOrderBookService internalOrderBookService,
            IExternalOrderBookService externalOrderBookService,
            IExchangeSettingsService exchangeSettingsService,
            ILogFactory logFactory)
        {
            _aggregatedOrderBookService = aggregatedOrderBookService;
            _internalOrderBookService = internalOrderBookService;
            _externalOrderBookService = externalOrderBookService;
            _exchangeSettingsService = exchangeSettingsService;

            _log = logFactory.CreateLog(this);
        }

        public async Task UpdateAsync(string exchange, string assetPair, DateTime timestamp,
            IReadOnlyList<OrderBookLevel> sellLevels, IReadOnlyList<OrderBookLevel> buyLevels)
        {
            await _semaphore.WaitAsync();

            try
            {
                ExchangeSettings exchangeSettings = await _exchangeSettingsService.GetByNameAsync(exchange);

                if (exchangeSettings == null || exchangeSettings.Status != ExchangeStatus.Active)
                    return;

                if (!exchangeSettings.Instruments.Contains(assetPair))
                    return;

                await _externalOrderBookService.UpdateAsync(exchange, assetPair, timestamp, sellLevels, buyLevels);

                await Task.WhenAll(
                    _aggregatedOrderBookService.UpdateAsync(assetPair),
                    _internalOrderBookService.UpdateAsync(assetPair));
            }
            catch (Exception exception)
            {
                _log.WarningWithDetails("An error occurred while calculating order books", exception,
                    new {exchange, assetPair, timestamp, sellLevels, buyLevels});
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task RemoveByExchangeAsync(string exchange)
        {
            await _semaphore.WaitAsync();

            try
            {
                _externalOrderBookService.Remove(exchange);

                await Task.WhenAll(
                    _aggregatedOrderBookService.ResetAsync(),
                    _internalOrderBookService.ResetAsync());
            }
            catch (Exception exception)
            {
                _log.WarningWithDetails("An error occurred while resetting order books", exception, new {exchange});
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}
