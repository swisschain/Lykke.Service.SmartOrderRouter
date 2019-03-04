using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.SmartOrderRouter.Domain.Entities.OrderBooks;

namespace Lykke.Service.SmartOrderRouter.Domain.Services
{
    public interface IExternalOrderBookService
    {
        IReadOnlyList<ExternalOrderBook> Filter(string exchange, string assetPair);
        
        IReadOnlyList<ExternalOrderBook> GetByAssetPair(string assetPair);
        
        Task UpdateAsync(string exchange, string assetPair, DateTime timestamp,
            IReadOnlyList<OrderBookLevel> sellLevels, IReadOnlyList<OrderBookLevel> buyLevels);

        void Remove(string exchange);
    }
}
