using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using AzureStorage;
using Lykke.Service.SmartOrderRouter.Domain.Entities.Exchanges;
using Lykke.Service.SmartOrderRouter.Domain.Repositories;

namespace Lykke.Service.SmartOrderRouter.AzureRepositories.Exchanges
{
    public class ExchangeSettingsRepository : IExchangeSettingsRepository
    {
        private readonly INoSQLTableStorage<ExchangeSettingsEntity> _storage;

        public ExchangeSettingsRepository(INoSQLTableStorage<ExchangeSettingsEntity> storage)
        {
            _storage = storage;
        }

        public async Task<IReadOnlyList<ExchangeSettings>> GetAllAsync()
        {
            IEnumerable<ExchangeSettingsEntity> entities = await _storage.GetDataAsync(GetPartitionKey());

            return Mapper.Map<List<ExchangeSettings>>(entities);
        }

        public async Task UpdateAsync(ExchangeSettings exchangeSettings)
        {
            var entity = new ExchangeSettingsEntity(GetPartitionKey(), GetRowKey(exchangeSettings.Name));

            Mapper.Map(exchangeSettings, entity);

            await _storage.InsertOrReplaceAsync(entity);
        }

        private static string GetPartitionKey()
            => "ExchangeSettings";

        private static string GetRowKey(string exchangeName)
            => exchangeName.ToUpper();
    }
}
