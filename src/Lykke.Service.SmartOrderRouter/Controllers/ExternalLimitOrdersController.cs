using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Lykke.Service.SmartOrderRouter.Client.Api;
using Lykke.Service.SmartOrderRouter.Client.Models.ExternalLimitOrders;
using Lykke.Service.SmartOrderRouter.Domain.Entities.Orders;
using Lykke.Service.SmartOrderRouter.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.SmartOrderRouter.Controllers
{
    [Route("/api/[controller]")]
    public class ExternalLimitOrdersController : Controller, IExternalLimitOrdersApi
    {
        private readonly IExternalLimitOrderService _externalLimitOrderService;

        public ExternalLimitOrdersController(IExternalLimitOrderService externalLimitOrderService)
        {
            _externalLimitOrderService = externalLimitOrderService;
        }

        /// <inheritdoc/>
        /// <response code="200">A collection of external limit orders.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<ExternalLimitOrderModel>), (int) HttpStatusCode.OK)]
        public async Task<IReadOnlyList<ExternalLimitOrderModel>> GetByMarketOrderIdAsync(string marketOrderId)
        {
            IReadOnlyList<ExternalLimitOrder> externalLimitOrders =
                await _externalLimitOrderService.GetByParentIdAsync(marketOrderId);

            return Mapper.Map<IReadOnlyList<ExternalLimitOrderModel>>(externalLimitOrders);
        }
    }
}
