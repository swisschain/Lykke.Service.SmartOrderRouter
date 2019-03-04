using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Lykke.Common.ApiLibrary.Exceptions;
using Lykke.Service.SmartOrderRouter.Client.Api;
using Lykke.Service.SmartOrderRouter.Client.Models.Exchanges;
using Lykke.Service.SmartOrderRouter.Domain.Entities.Exchanges;
using Lykke.Service.SmartOrderRouter.Domain.Exceptions;
using Lykke.Service.SmartOrderRouter.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.SmartOrderRouter.Controllers
{
    [Route("/api/[controller]")]
    public class ExchangesController : Controller, IExchangesApi
    {
        private readonly IExchangeSettingsService _exchangeSettingsService;
        private readonly IOrderBookService _orderBookService;

        public ExchangesController(
            IExchangeSettingsService exchangeSettingsService,
            IOrderBookService orderBookService)
        {
            _exchangeSettingsService = exchangeSettingsService;
            _orderBookService = orderBookService;
        }

        /// <inheritdoc/>
        /// <response code="200">A collection of exchange settings.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<ExchangeSettingsModel>), (int) HttpStatusCode.OK)]
        public async Task<IReadOnlyList<ExchangeSettingsModel>> GetAllAsync()
        {
            IReadOnlyList<ExchangeSettings> settings = await _exchangeSettingsService.GetAllAsync();

            return Mapper.Map<IReadOnlyList<ExchangeSettingsModel>>(settings);
        }

        /// <inheritdoc/>
        /// <response code="200">The exchange settings.</response>
        [HttpGet("{name}")]
        [ProducesResponseType(typeof(ExchangeSettingsModel), (int) HttpStatusCode.OK)]
        public async Task<ExchangeSettingsModel> GetByNameAsync(string name)
        {
            ExchangeSettings settings = await _exchangeSettingsService.GetByNameAsync(name);

            return Mapper.Map<ExchangeSettingsModel>(settings);
        }

        /// <inheritdoc/>
        /// <response code="204">The exchange settings successfully updated.</response>
        [HttpPut]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        public async Task UpdateAsync([FromBody] ExchangeSettingsModel model, string userId)
        {
            var settings = Mapper.Map<ExchangeSettings>(model);

            await _exchangeSettingsService.UpdateAsync(settings, userId);
        }

        /// <inheritdoc/>
        /// <response code="204">The exchange status successfully updated.</response>
        [HttpPost("{name}/start")]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        public async Task StartAsync(string name, string userId)
        {
            try
            {
                await _exchangeSettingsService.StartAsync(name, userId);
            }
            catch (EntityNotFoundException)
            {
                throw new ValidationApiException(HttpStatusCode.NotFound, "Exchange not found");
            }
        }

        /// <inheritdoc/>
        /// <response code="204">The exchange status successfully updated.</response>
        [HttpPost("{name}/stop")]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        public async Task StopAsync(string name, string userId)
        {
            try
            {
                await _exchangeSettingsService.StartAsync(name, userId);

                await _orderBookService.RemoveByExchangeAsync(name);
            }
            catch (EntityNotFoundException)
            {
                throw new ValidationApiException(HttpStatusCode.NotFound, "Exchange not found");
            }
        }
    }
}
