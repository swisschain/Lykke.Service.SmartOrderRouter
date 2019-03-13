using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.SmartOrderRouter.Domain.Entities.Orders;

namespace Lykke.Service.SmartOrderRouter.Domain.Repositories
{
    public interface IMarketOrderRepository
    {
        Task<MarketOrder> GetByIdAsync(string marketOrderId);

        Task<IReadOnlyList<MarketOrder>> GetByStatusAsync(MarketOrderStatus marketOrderStatus);

        Task<IReadOnlyList<MarketOrder>> GetByClientIdAsync(string clientId);

        Task<IReadOnlyList<MarketOrder>> GetAllAsync(DateTime startDate, DateTime endDate, int? limit);

        Task InsertAsync(MarketOrder marketOrder);

        Task UpdateAsync(MarketOrder marketOrder);
    }
}
