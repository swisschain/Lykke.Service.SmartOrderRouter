using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.SmartOrderRouter.Client.Models.Reports;
using Refit;

namespace Lykke.Service.SmartOrderRouter.Client.Api
{
    /// <summary>
    /// Provides methods for working with reports.
    /// </summary>
    [PublicAPI]
    public interface IReportsApi
    {
        /// <summary>
        /// Returns balance report.
        /// </summary>
        /// <returns>The model that represent the balance report.</returns>
        [Get("/api/Reports/balance")]
        Task<BalanceReportModel> GetBalanceReportAsync();
    }
}
