using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.SmartOrderRouter.Domain.Entities;

namespace Lykke.Service.SmartOrderRouter.Domain.Services
{
    public interface IQuoteService
    {
        IReadOnlyList<Quote> GetAll();

        Quote GetByAssetPair(string source, string assetPair);

        void Update(Quote quote);
    }
}
