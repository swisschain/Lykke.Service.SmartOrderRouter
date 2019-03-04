using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.SmartOrderRouter.Domain.Entities.OrderBooks;

namespace Lykke.Service.SmartOrderRouter.Domain.Services
{
    public interface IInternalOrderBookService
    {
        IReadOnlyList<InternalOrderBook> GetAll();

        InternalOrderBook GetByAssetPair(string assetPair);

        Task UpdateAsync(string assetPair);

        Task ResetAsync();
    }
}
