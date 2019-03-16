using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.SmartOrderRouter.Client.Models.Exchanges;
using Refit;

namespace Lykke.Service.SmartOrderRouter.Client.Api
{
    /// <summary>
    /// Provides methods for working with exchanges.
    /// </summary>
    [PublicAPI]
    public interface IExchangesApi
    {
        /// <summary>
        /// Returns a collection of available exchange settings.
        /// </summary>
        /// <returns>A collection of exchange settings.</returns>
        [Get("/api/Exchanges")]
        Task<IReadOnlyList<ExchangeSettingsModel>> GetAllAsync();

        /// <summary>
        /// Returns exchange settings be name.
        /// </summary>
        /// <param name="name">The name of the exchange.</param>
        /// <returns>The exchange settings.</returns>
        [Get("/api/Exchanges/{name}")]
        Task<ExchangeSettingsModel> GetByNameAsync(string name);

        /// <summary>
        /// Updates exchange settings.
        /// </summary>
        /// <param name="model">The model that describes exchange settings.</param> 
        /// <param name="userId">The identifier of user witch performed operation.</param>
        [Put("/api/Exchanges")]
        Task UpdateAsync([Body] ExchangeSettingsModel model, string userId);
        
        /// <summary>
        /// Activates exchange.
        /// </summary>
        /// <param name="name">The name of the exchange.</param>
        /// <param name="userId">The identifier of user witch performed operation.</param>
        [Post("/api/Exchanges/{name}/start")]
        Task StartAsync(string name, string userId);
        
        /// <summary>
        /// Stop an activity of exchange.
        /// </summary>
        /// <param name="name">The name of the exchange.</param>
        /// <param name="userId">The identifier of user witch performed operation.</param>
        [Post("/api/Exchanges/{name}/stop")]
        Task StopAsync(string name, string userId);
    }
}
