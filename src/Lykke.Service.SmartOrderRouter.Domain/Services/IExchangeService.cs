using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.SmartOrderRouter.Domain.Entities.Balances;
using Lykke.Service.SmartOrderRouter.Domain.Entities.Orders;

namespace Lykke.Service.SmartOrderRouter.Domain.Services
{
    public interface IExchangeService
    {
        IReadOnlyList<string> GetExchanges();

        Task<IReadOnlyList<Balance>> GetBalancesAsync(string exchangeName);

        Task<ExternalLimitOrder> CreateLimitOrderAsync(string exchangeName, string assetPair, decimal price,
            decimal volume, OrderType orderType);

        Task<bool> CancelLimitOrderAsync(ExternalLimitOrder externalLimitOrder);
    }
}
