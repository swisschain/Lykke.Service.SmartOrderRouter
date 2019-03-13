using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.SmartOrderRouter.Domain.Entities.Orders;
using Lykke.Service.SmartOrderRouter.Domain.Repositories;
using Lykke.Service.SmartOrderRouter.Domain.Services;

namespace Lykke.Service.SmartOrderRouter.DomainServices.Orders
{
    public class ExternalLimitOrderService : IExternalLimitOrderService
    {
        private readonly IExternalLimitOrderRepository _externalLimitOrderRepository;

        public ExternalLimitOrderService(IExternalLimitOrderRepository externalLimitOrderRepository)
        {
            _externalLimitOrderRepository = externalLimitOrderRepository;
        }
        
        public Task<ExternalLimitOrder> GetByIdAsync(string externalLimitOrderId)
        {
            return _externalLimitOrderRepository.GetByIdAsync(externalLimitOrderId);
        }

        public Task<IReadOnlyList<ExternalLimitOrder>> GetByParentIdAsync(string parentOrderId)
        {
            return _externalLimitOrderRepository.GetByParentIdAsync(parentOrderId);
        }

        public Task AddAsync(string parentId, ExternalLimitOrder externalLimitOrder)
        {
            return _externalLimitOrderRepository.InsertAsync(parentId, externalLimitOrder);
        }

        public Task UpdateAsync(ExternalLimitOrder externalLimitOrder)
        {
            return _externalLimitOrderRepository.UpdateAsync(externalLimitOrder);
        }
    }
}
