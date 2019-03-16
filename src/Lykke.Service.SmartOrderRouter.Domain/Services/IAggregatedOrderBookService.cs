using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.SmartOrderRouter.Domain.Entities.OrderBooks;

namespace Lykke.Service.SmartOrderRouter.Domain.Services
{
    public interface IAggregatedOrderBookService
    {
        IReadOnlyList<AggregatedOrderBook> GetAll();
        
        AggregatedOrderBook GetByAssetPair(string assetPair);
        
        Task UpdateAsync(string assetPair);

        Task ResetAsync();
    }
}
