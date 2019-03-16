using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Lykke.Service.SmartOrderRouter.Client.Api;
using Lykke.Service.SmartOrderRouter.Client.Models.Settings;
using Lykke.Service.SmartOrderRouter.Domain.Entities.Settings;
using Lykke.Service.SmartOrderRouter.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.SmartOrderRouter.Controllers
{
    [Route("/api/[controller]")]
    public class SettingsController : Controller, ISettingsApi
    {
        private readonly ISettingsService _settingsService;

        public SettingsController(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        /// <inheritdoc/>
        /// <response code="200">The model that represent the timer settings.</response>
        [HttpGet("timers")]
        [ProducesResponseType(typeof(TimerSettingsModel), (int) HttpStatusCode.OK)]
        public async Task<TimerSettingsModel> GetTimerSettingsAsync()
        {
            TimerSettings timerSettings = await _settingsService.GetTimerSettingsAsync();

            return Mapper.Map<TimerSettingsModel>(timerSettings);
        }

        /// <inheritdoc/>
        /// <response code="204">The timer settings successfully updated.</response>
        [HttpPut("timers")]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        public async Task UpdateTimerSettingsAsync([FromBody] TimerSettingsModel model)
        {
            TimerSettings timerSettings = Mapper.Map<TimerSettings>(model);

            await _settingsService.UpdateTimerSettingsAsync(timerSettings);
        }
    }
}
