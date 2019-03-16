using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.SmartOrderRouter.Domain.Entities.OrderBooks;

namespace Lykke.Service.SmartOrderRouter.Domain.Services
{
    public interface IOrderBookService
    {
        Task UpdateAsync(string exchange, string assetPair, DateTime timestamp,
            IReadOnlyList<OrderBookLevel> sellLevels, IReadOnlyList<OrderBookLevel> buyLevels);

        Task RemoveByExchangeAsync(string exchange);
    }
}
