using System.Collections.Generic;
using System.Linq;

namespace Lykke.Service.SmartOrderRouter.Domain.Entities.Exchanges
{
    /// <summary>
    /// Represents an exchange settings.
    /// </summary>
    public class ExchangeSettings
    {
        public ExchangeSettings()
        {
            Instruments = new List<string>();
        }

        public ExchangeSettings(string name)
            : this()
        {
            Name = name;
            Status = ExchangeStatus.Stopped;
        }

        /// <summary>
        /// The name of exchange.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Indicates the status of exchange. 
        /// </summary>
        public ExchangeStatus Status { get; set; }

        /// <summary>
        /// The collection of instruments that can be used on exchange to calculate order books and execute orders.
        /// </summary>
        public IReadOnlyList<string> Instruments { get; set; }

        /// <summary>
        /// The fee in bps of cash-in/out operation on exchange.
        /// </summary>
        public decimal MarketFee { get; set; }

        /// <summary>
        /// The fee in bps of transaction on exchange.
        /// </summary>
        public decimal TransactionFee { get; set; }

        /// <summary>
        /// The markup for reducing order slippage while trading.  
        /// </summary>
        public decimal SlippageMarkup { get; set; }

        public ExchangeSettings Clone()
        {
            return new ExchangeSettings
            {
                Name = Name,
                Status = Status,
                Instruments = (Instruments ?? new string[0]).ToList(),
                MarketFee = MarketFee,
                TransactionFee = TransactionFee,
                SlippageMarkup = SlippageMarkup
            };
        }
    }
}
