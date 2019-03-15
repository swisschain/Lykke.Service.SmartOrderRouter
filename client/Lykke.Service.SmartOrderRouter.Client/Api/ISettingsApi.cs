using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.SmartOrderRouter.Client.Models.Settings;
using Refit;

namespace Lykke.Service.SmartOrderRouter.Client.Api
{
    /// <summary>
    /// Provides methods for working with service settings.
    /// </summary>
    [PublicAPI]
    public interface ISettingsApi
    {
        /// <summary>
        /// Returns internal timers settings.
        /// </summary>
        /// <returns>The model that represent the timer settings.</returns>
        [Get("/api/Settings/timers")]
        Task<TimerSettingsModel> GetTimerSettingsAsync();

        /// <summary>
        /// Updates internal timers settings.
        /// </summary>
        /// <param name="model">The model that represent the timer settings.</param>
        [Put("/api/Settings/timers")]
        Task UpdateTimerSettingsAsync([Body] TimerSettingsModel model);
    }
}
