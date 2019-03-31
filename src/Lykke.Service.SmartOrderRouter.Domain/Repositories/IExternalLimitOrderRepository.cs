using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.SmartOrderRouter.Domain.Entities.Orders;

namespace Lykke.Service.SmartOrderRouter.Domain.Repositories
{
    public interface IExternalLimitOrderRepository
    {
        Task<IReadOnlyList<ExternalLimitOrder>> GetAllAsync();
        
        Task<ExternalLimitOrder> GetByIdAsync(string externalLimitOrderId);

        Task<IReadOnlyList<ExternalLimitOrder>> GetByParentIdAsync(string parentOrderId);

        Task InsertAsync(string parentId, ExternalLimitOrder externalLimitOrder);

        Task UpdateAsync(ExternalLimitOrder externalLimitOrder);
    }
}
