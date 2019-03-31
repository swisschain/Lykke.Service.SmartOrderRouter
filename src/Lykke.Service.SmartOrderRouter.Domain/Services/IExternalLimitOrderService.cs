using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.SmartOrderRouter.Domain.Entities.Orders;

namespace Lykke.Service.SmartOrderRouter.Domain.Services
{
    public interface IExternalLimitOrderService
    {
        Task<ExternalLimitOrder> GetByIdAsync(string externalLimitOrderId);

        Task<IReadOnlyList<ExternalLimitOrder>> GetByParentIdAsync(string parentOrderId);

        Task<IReadOnlyList<ExternalLimitOrder>> FilterAsync(string marketOrderId, ExternalLimitOrderStatus? status,
            DateTime? startDate, DateTime? endDate, int? limit);

        Task AddAsync(string parentId, ExternalLimitOrder externalLimitOrder);

        Task UpdateAsync(ExternalLimitOrder externalLimitOrder);
    }
}
