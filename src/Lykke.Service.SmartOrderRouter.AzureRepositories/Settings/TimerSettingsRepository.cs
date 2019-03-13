using System.Threading.Tasks;
using AutoMapper;
using AzureStorage;
using Lykke.Service.SmartOrderRouter.Domain.Entities.Settings;
using Lykke.Service.SmartOrderRouter.Domain.Repositories;

namespace Lykke.Service.SmartOrderRouter.AzureRepositories.Settings
{
    public class TimerSettingsRepository : ITimerSettingsRepository
    {
        private readonly INoSQLTableStorage<TimerSettingsEntity> _storage;

        public TimerSettingsRepository(INoSQLTableStorage<TimerSettingsEntity> storage)
        {
            _storage = storage;
        }

        public async Task<TimerSettings> GetAsync()
        {
            TimerSettingsEntity entity = await _storage.GetDataAsync(GetPartitionKey(), GetRowKey());

            return Mapper.Map<TimerSettings>(entity);
        }

        public async Task UpdateAsync(TimerSettings timerSettings)
        {
            var entity = new TimerSettingsEntity(GetPartitionKey(), GetRowKey());

            Mapper.Map(timerSettings, entity);

            await _storage.InsertOrReplaceAsync(entity);
        }

        private static string GetPartitionKey()
            => "Timers";

        private static string GetRowKey()
            => "Timers";
    }
}
