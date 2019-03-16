using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.SmartOrderRouter.Client.Models.Balances;
using Refit;

namespace Lykke.Service.SmartOrderRouter.Client.Api
{
    /// <summary>
    /// Provides methods for working with balances.
    /// </summary>
    [PublicAPI]
    public interface IBalancesApi
    {
        /// <summary>
        /// Returns a collection of assets balances filtered by exchange.
        /// </summary>
        /// <param name="exchange">The name of the exchange.</param>
        /// <returns>A collection of assets balances.</returns>
        [Get("/api/Balances")]
        Task<IReadOnlyList<BalanceModel>> GetAsync(string exchange = null);
    }
}
