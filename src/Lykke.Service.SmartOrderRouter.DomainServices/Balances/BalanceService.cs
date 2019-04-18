using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Service.SmartOrderRouter.Domain;
using Lykke.Service.SmartOrderRouter.Domain.Entities.Balances;
using Lykke.Service.SmartOrderRouter.Domain.Exceptions;
using Lykke.Service.SmartOrderRouter.Domain.Services;
using Lykke.Service.SmartOrderRouter.DomainServices.Extensions;

namespace Lykke.Service.SmartOrderRouter.DomainServices.Balances
{
    public class BalanceService : IBalanceService
    {
        private readonly IExchange[] _exchanges;
        private readonly ILog _log;

        private readonly ConcurrentDictionary<string, IReadOnlyList<Balance>> _balances =
            new ConcurrentDictionary<string, IReadOnlyList<Balance>>();

        public BalanceService(
            IExchange[] exchanges,
            ILogFactory logFactory)
        {
            _exchanges = exchanges;
            _log = logFactory.CreateLog(this);
        }

        public IReadOnlyList<Balance> GetAll()
            => _balances.Values.SelectMany(o => o).ToList();

        public Balance Get(string exchange, string asset)
        {
            Balance balance = null;

            if (_balances.TryGetValue(exchange, out IReadOnlyList<Balance> balances))
                balance = balances.FirstOrDefault(o => o.Asset == asset);
            
            return balance ?? new Balance(exchange, asset, decimal.Zero, decimal.Zero);
        }

        public IReadOnlyList<Balance> GetByExchange(string exchange)
        {
            if (_balances.TryGetValue(exchange, out IReadOnlyList<Balance> balances))
                return balances;

            return new Balance[0];
        }

        public async Task UpdateAsync()
        {
            foreach (IExchange exchange in _exchanges)
            {
                try
                {
                    IReadOnlyList<Balance> balances = await exchange.GetBalancesAsync();

                    _balances.AddOrUpdate(exchange.Name, balances, (key, value) => balances);
                }
                catch (Exception exception)
                {
                    _log.ErrorWithDetails(exception, "An unexpected error occurred while getting balance",
                        new {Exchange = exchange.Name});
                }
            }
        }
    }
}
