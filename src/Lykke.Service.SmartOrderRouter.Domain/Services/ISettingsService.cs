using System.Threading.Tasks;
using Lykke.Service.SmartOrderRouter.Domain.Entities.Settings;

namespace Lykke.Service.SmartOrderRouter.Domain.Services
{
    public interface ISettingsService
    {
        string GetExchangeName();

        Task<TimerSettings> GetTimerSettingsAsync();

        Task UpdateTimerSettingsAsync(TimerSettings timerSettings);
    }
}
