using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<IReadOnlyList<ExternalLimitOrder>> FilterAsync(string marketOrderId,
            ExternalLimitOrderStatus? status, DateTime? startDate, DateTime? endDate, int? limit)
        {
            IEnumerable<ExternalLimitOrder> externalLimitOrders;

            if (string.IsNullOrEmpty(marketOrderId))
                externalLimitOrders = await _externalLimitOrderRepository.GetAllAsync();
            else
                externalLimitOrders = await _externalLimitOrderRepository.GetByParentIdAsync(marketOrderId);

            if (startDate.HasValue)
                externalLimitOrders = externalLimitOrders.Where(o => o.CreatedDate >= startDate);
            
            if (endDate.HasValue)
                externalLimitOrders = externalLimitOrders.Where(o => o.CreatedDate <= endDate);
            
            if (status.HasValue)
                externalLimitOrders = externalLimitOrders.Where(o => o.Status == status.Value);

            externalLimitOrders = externalLimitOrders.OrderByDescending(o => o.CreatedDate);
            
            if(limit.HasValue)
                externalLimitOrders = externalLimitOrders.Take(limit.Value);

            return externalLimitOrders.ToList();
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
