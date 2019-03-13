using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Lykke.Service.SmartOrderRouter.Client.Api;
using Lykke.Service.SmartOrderRouter.Client.Models.Balances;
using Lykke.Service.SmartOrderRouter.Domain.Entities.Balances;
using Lykke.Service.SmartOrderRouter.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.SmartOrderRouter.Controllers
{
    [Route("/api/[controller]")]
    public class BalancesController : Controller, IBalancesApi
    {
        private readonly IBalanceService _balanceService;

        public BalancesController(IBalanceService balanceService)
        {
            _balanceService = balanceService;
        }

        /// <inheritdoc/>
        /// <response code="200">A collection of balances.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<BalanceModel>), (int) HttpStatusCode.OK)]
        public Task<IReadOnlyList<BalanceModel>> GetAsync(string exchange = null)
        {
            IReadOnlyList<Balance> balances;

            if (string.IsNullOrEmpty(exchange))
                balances = _balanceService.GetAll();
            else
                balances = _balanceService.GetByExchange(exchange);

            return Task.FromResult(Mapper.Map<IReadOnlyList<BalanceModel>>(balances));
        }
    }
}
