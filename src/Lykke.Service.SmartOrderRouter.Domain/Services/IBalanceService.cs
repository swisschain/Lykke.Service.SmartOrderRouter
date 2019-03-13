using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.SmartOrderRouter.Domain.Entities.Balances;

namespace Lykke.Service.SmartOrderRouter.Domain.Services
{
    public interface IBalanceService
    {
        IReadOnlyList<Balance> GetAll();

        IReadOnlyList<Balance> GetByExchange(string exchange);

        Task UpdateAsync();
    }
}
