using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.SmartOrderRouter.Client.Models.Quotes;
using Refit;

namespace Lykke.Service.SmartOrderRouter.Client.Api
{
    /// <summary>
    /// Provides methods for working with quotes.
    /// </summary>
    [PublicAPI]
    public interface IQuotesApi
    {
        /// <summary>
        /// Returns a collection of quotes filtered by exchange and asset pair. 
        /// </summary>
        /// <param name="exchange">The name of the exchange.</param>
        /// <param name="assetPair">The name of the asset pair.</param>
        /// <returns>A collection of quotes.</returns>
        [Get("/api/Quotes")]
        Task<IReadOnlyList<QuoteModel>> GetAsync(string exchange = null, string assetPair = null);
    }
}
