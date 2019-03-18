using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Lykke.Common.ApiLibrary.Exceptions;
using Lykke.Service.SmartOrderRouter.Client.Api;
using Lykke.Service.SmartOrderRouter.Client.Models.MarketOrders;
using Lykke.Service.SmartOrderRouter.Domain.Entities.Orders;
using Lykke.Service.SmartOrderRouter.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using OrderType = Lykke.Service.SmartOrderRouter.Client.Models.OrderType;

namespace Lykke.Service.SmartOrderRouter.Controllers
{
    [Route("/api/[controller]")]
    public class MarketOrdersController : Controller, IMarketOrdersApi
    {
        private readonly IMarketOrderService _marketOrderService;

        public MarketOrdersController(IMarketOrderService marketOrderService)
        {
            _marketOrderService = marketOrderService;
        }

        /// <inheritdoc/>
        /// <response code="200">A collection of market orders.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<MarketOrderModel>), (int) HttpStatusCode.OK)]
        public async Task<IReadOnlyList<MarketOrderModel>> GetAsync(DateTime startDate, DateTime endDate, int? limit)
        {
            if (startDate == DateTime.MinValue || endDate == DateTime.MinValue || endDate < startDate)
                throw new ValidationApiException("Invalid requested timer range");

            IReadOnlyList<MarketOrder> marketOrders = await _marketOrderService.GetAllAsync(startDate, endDate, limit);

            return Mapper.Map<IReadOnlyList<MarketOrderModel>>(marketOrders);
        }

        /// <inheritdoc/>
        /// <response code="200">A collection of market orders.</response>
        [HttpGet("client/{clientId}")]
        [ProducesResponseType(typeof(IReadOnlyList<MarketOrderModel>), (int) HttpStatusCode.OK)]
        public async Task<IReadOnlyList<MarketOrderModel>> GetByClientIdAsync(string clientId)
        {
            IReadOnlyList<MarketOrder> marketOrders = await _marketOrderService.GetByClientIdAsync(clientId);

            return Mapper.Map<IReadOnlyList<MarketOrderModel>>(marketOrders);
        }

        /// <inheritdoc/>
        /// <response code="200">A market order.</response>
        [HttpGet("{marketOrderId}")]
        [ProducesResponseType(typeof(MarketOrderModel), (int) HttpStatusCode.OK)]
        public async Task<MarketOrderModel> GetByIdAsync(string marketOrderId)
        {
            MarketOrder marketOrder = await _marketOrderService.GetByIdAsync(marketOrderId);

            if (marketOrder == null)
                throw new ValidationApiException(HttpStatusCode.NotFound, "Market order not found");

            return Mapper.Map<MarketOrderModel>(marketOrder);
        }

        /// <inheritdoc/>
        /// <response code="200">The market order successfully created.</response>
        [HttpPost]
        [ProducesResponseType((int) HttpStatusCode.OK)]
        public async Task CreateAsync([FromBody] MarketOrderRequestModel model)
        {
            await _marketOrderService.CreateAsync(model.ClientId, model.AssetPair, model.Volume,
                model.Type == OrderType.Sell
                    ? Domain.Entities.Orders.OrderType.Sell
                    : Domain.Entities.Orders.OrderType.Buy);
        }
    }
}
