using System.Threading.Tasks;
using Lykke.Service.SmartOrderRouter.Domain.Entities.Settings;

namespace Lykke.Service.SmartOrderRouter.Domain.Repositories
{
    public interface ITimerSettingsRepository
    {
        Task<TimerSettings> GetAsync();

        Task UpdateAsync(TimerSettings timerSettings);
    }
}
