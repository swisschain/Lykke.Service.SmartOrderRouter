using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Lykke.Service.SmartOrderRouter.Client.Api;
using Lykke.Service.SmartOrderRouter.Client.Models.Reports;
using Lykke.Service.SmartOrderRouter.Domain.Entities.Reports.Balances;
using Lykke.Service.SmartOrderRouter.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.SmartOrderRouter.Controllers
{
    [Route("/api/[controller]")]
    public class ReportsController : Controller, IReportsApi
    {
        private readonly IBalanceReportService _balanceReportService;

        public ReportsController(IBalanceReportService balanceReportService)
        {
            _balanceReportService = balanceReportService;
        }

        /// <inheritdoc/>
        /// <response code="200">The balance report.</response>
        [HttpGet("balance")]
        [ProducesResponseType(typeof(BalanceReportModel), (int) HttpStatusCode.OK)]
        public Task<BalanceReportModel> GetBalanceReportAsync()
        {
            BalanceReport balanceReport = _balanceReportService.Get();

            return Task.FromResult(Mapper.Map<BalanceReportModel>(balanceReport));
        }
    }
}
