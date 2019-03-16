using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.SmartOrderRouter.Domain.Entities.Exchanges;

namespace Lykke.Service.SmartOrderRouter.Domain.Repositories
{
    public interface IExchangeSettingsRepository
    {
        Task<IReadOnlyList<ExchangeSettings>> GetAllAsync();
        
        Task UpdateAsync(ExchangeSettings exchangeSettings);
    }
}
