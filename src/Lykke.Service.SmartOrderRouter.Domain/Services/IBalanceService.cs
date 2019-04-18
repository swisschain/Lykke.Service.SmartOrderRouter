using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.SmartOrderRouter.Domain.Entities.Balances;

namespace Lykke.Service.SmartOrderRouter.Domain.Services
{
    public interface IBalanceService
    {
        IReadOnlyList<Balance> GetAll();

        Balance Get(string exchange, string asset);

        IReadOnlyList<Balance> GetByExchange(string exchange);

        Task UpdateAsync();
    }
}
