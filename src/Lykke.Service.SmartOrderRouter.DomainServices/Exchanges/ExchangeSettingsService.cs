using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Service.SmartOrderRouter.Domain.Entities.Exchanges;
using Lykke.Service.SmartOrderRouter.Domain.Exceptions;
using Lykke.Service.SmartOrderRouter.Domain.Repositories;
using Lykke.Service.SmartOrderRouter.Domain.Services;
using Lykke.Service.SmartOrderRouter.DomainServices.Cache;
using Lykke.Service.SmartOrderRouter.DomainServices.Extensions;

namespace Lykke.Service.SmartOrderRouter.DomainServices.Exchanges
{
    public class ExchangeSettingsService : IExchangeSettingsService
    {
        private readonly IExchangeSettingsRepository _exchangeSettingsRepository;
        private readonly IReadOnlyList<string> _exchanges;
        private readonly ILog _log;

        private readonly InMemoryCache<ExchangeSettings> _cache;

        public ExchangeSettingsService(
            IExchangeSettingsRepository exchangeSettingsRepository,
            IReadOnlyList<string> exchanges,
            ILogFactory logFactory)
        {
            _exchangeSettingsRepository = exchangeSettingsRepository;
            _exchanges = exchanges;
            _log = logFactory.CreateLog(this);

            _cache = new InMemoryCache<ExchangeSettings>(GetKey, false);
        }

        public async Task<IReadOnlyList<ExchangeSettings>> GetAllAsync()
        {
            IReadOnlyList<ExchangeSettings> settings = _cache.GetAll();

            if (settings == null)
            {
                settings = await _exchangeSettingsRepository.GetAllAsync();

                settings = settings.Union(_exchanges
                        .Where(o => settings.All(p => p.Name != o))
                        .Select(o => new ExchangeSettings(o)))
                    .Where(o => _exchanges.Contains(o.Name))
                    .ToList();

                _cache.Initialize(settings);
            }

            return settings;
        }

        public async Task<ExchangeSettings> GetByNameAsync(string exchangeName)
        {
            IReadOnlyList<ExchangeSettings> settings = await GetAllAsync();

            return settings.SingleOrDefault(o => o.Name == exchangeName);
        }

        public async Task UpdateAsync(ExchangeSettings exchangeSettings, string userId)
        {
            await _exchangeSettingsRepository.UpdateAsync(exchangeSettings);

            _cache.Set(exchangeSettings);
            
            _log.InfoWithDetails("Exchange setting updated", new {exchangeSettings, userId});
        }

        public async Task StartAsync(string exchangeName, string userId)
        {
            await SetStatusAsync(exchangeName, ExchangeStatus.Active);

            _log.InfoWithDetails("Exchange activated", new {exchangeName, userId});
        }

        public async Task StopAsync(string exchangeName, string userId)
        {
            await SetStatusAsync(exchangeName, ExchangeStatus.Stopped);

            _log.InfoWithDetails("Exchange stopped", new {exchangeName, userId});
        }

        private async Task SetStatusAsync(string exchangeName, ExchangeStatus exchangeStatus)
        {
            ExchangeSettings exchangeSettings = await GetByNameAsync(exchangeName);

            if (exchangeSettings == null)
                throw new EntityNotFoundException();

            if (exchangeSettings.Status == exchangeStatus)
                return;

            exchangeSettings = exchangeSettings.Clone();

            exchangeSettings.Status = exchangeStatus;

            await _exchangeSettingsRepository.UpdateAsync(exchangeSettings);

            _cache.Set(exchangeSettings);
        }

        private static string GetKey(ExchangeSettings exchangeSettings)
            => exchangeSettings.Name.ToUpper();
    }
}
