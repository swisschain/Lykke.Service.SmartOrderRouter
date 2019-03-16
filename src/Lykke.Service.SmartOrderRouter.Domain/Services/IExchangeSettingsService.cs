using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.SmartOrderRouter.Domain.Entities.Exchanges;

namespace Lykke.Service.SmartOrderRouter.Domain.Services
{
    public interface IExchangeSettingsService
    {
        Task<IReadOnlyList<ExchangeSettings>> GetAllAsync();

        Task<ExchangeSettings> GetByNameAsync(string exchangeName);

        Task UpdateAsync(ExchangeSettings exchangeSettings, string userId);

        Task StartAsync(string exchangeName, string userId);

        Task StopAsync(string exchangeName, string userId);
    }
}
