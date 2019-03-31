using System.Collections.Generic;
using System.Linq;
using Lykke.Service.SmartOrderRouter.Domain.Entities;
using Lykke.Service.SmartOrderRouter.Domain.Entities.Balances;
using Lykke.Service.SmartOrderRouter.Domain.Entities.Reports.Balances;
using Lykke.Service.SmartOrderRouter.Domain.Services;

namespace Lykke.Service.SmartOrderRouter.DomainServices.Reports
{
    public class BalanceReportService : IBalanceReportService
    {
        private readonly IBalanceService _balanceService;
        private readonly IQuoteService _quoteService;

        public BalanceReportService(
            IBalanceService balanceService,
            IQuoteService quoteService)
        {
            _balanceService = balanceService;
            _quoteService = quoteService;
        }

        public BalanceReport Get()
        {
            IReadOnlyList<Balance> balances = _balanceService.GetAll();

            List<string> exchanges = balances
                .Select(o => o.Exchange)
                .Distinct()
                .ToList();

            List<string> assets = balances
                .Select(o => o.Asset)
                .Distinct()
                .OrderBy(o => o)
                .ToList();

            var items = new List<BalanceReportItem>();

            foreach (string asset in assets)
            {
                var prices = new List<decimal>();

                foreach (string exchange in exchanges)
                {
                    Quote quote = _quoteService.GetByAssetPair(exchange, $"{asset}USD");

                    if (quote != null)
                        prices.Add(quote.Mid);
                }

                decimal price = prices.DefaultIfEmpty(0).Average();

                items.AddRange(balances
                    .Where(o => o.Asset == asset)
                    .Select(o => new BalanceReportItem
                    {
                        Asset = asset,
                        Exchange = o.Exchange,
                        Amount = o.Amount,
                        AmountUsd = o.Amount * price
                    }));
            }

            return new BalanceReport {Items = items};
        }
    }
}
