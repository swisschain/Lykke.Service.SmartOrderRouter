using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.SmartOrderRouter.Client.Models.OrderBooks;
using Refit;

namespace Lykke.Service.SmartOrderRouter.Client.Api
{
    /// <summary>
    /// Provides methods for working with order books.
    /// </summary>
    [PublicAPI]
    public interface IOrderBooksApi
    {
        /// <summary>
        /// Returns a collection of aggregated order books. 
        /// </summary>
        /// <returns>A collection of aggregated order books.</returns>
        [Get("/api/OrderBooks/aggregated")]
        Task<IReadOnlyList<AggregatedOrderBookModel>> GetAggregatedAsync();

        /// <summary>
        /// Returns an aggregated order book by asset pair. 
        /// </summary>
        /// <param name="assetPair">The name of the asset pair.</param>
        /// <returns>The aggregated order book.</returns>
        [Get("/api/OrderBooks/aggregated/{assetPair}")]
        Task<AggregatedOrderBookModel> GetAggregatedAsync(string assetPair);

        /// <summary>
        /// Returns a collection of internal order books. 
        /// </summary>
        /// <returns>A collection of internal order books.</returns>
        [Get("/api/OrderBooks/internal")]
        Task<IReadOnlyList<InternalOrderBookModel>> GetInternalAsync();
        
        /// <summary>
        /// Returns an internal order book by asset pair. 
        /// </summary>
        /// <param name="assetPair">The name of the asset pair.</param>
        /// <returns>The internal order book.</returns>
        [Get("/api/OrderBooks/internal/{assetPair}")]
        Task<InternalOrderBookModel> GetInternalAsync(string assetPair);

        /// <summary>
        /// Returns a collection of external order books filtered by exchange and asset pair. 
        /// </summary>
        /// <param name="exchange">The name of the exchange.</param>
        /// <param name="assetPair">The name of the asset pair.</param>
        /// <returns>A collection of external order books.</returns>
        [Get("/api/OrderBooks/external")]
        Task<IReadOnlyList<ExternalOrderBookModel>> GetExternalAsync(string exchange = null, string assetPair = null);
    }
}
