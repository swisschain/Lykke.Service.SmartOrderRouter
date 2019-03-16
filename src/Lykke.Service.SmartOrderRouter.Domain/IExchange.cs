using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.SmartOrderRouter.Domain.Entities.Balances;
using Lykke.Service.SmartOrderRouter.Domain.Entities.Orders;

namespace Lykke.Service.SmartOrderRouter.Domain
{
    public interface IExchange
    {
        string Name { get; }

        Task<IReadOnlyList<Balance>> GetBalancesAsync();

        Task<ExternalLimitOrderInfo> GetLimitOrderInfoAsync(string limitOrderId);

        Task<string> CreateLimitOrderAsync(string assetPair, decimal price, decimal volume, OrderType orderType);

        Task CancelLimitOrderAsync(string limitOrderId);
    }
}
