using System.Collections.Generic;

namespace Lykke.Service.SmartOrderRouter.Domain.Entities.Exchanges
{
    /// <summary>
    /// Represents an exchange settings.
    /// </summary>
    public class ExchangeSettings
    {
        public ExchangeSettings()
        {
        }

        public ExchangeSettings(string name)
        {
            Name = name;
            Status = ExchangeStatus.Stopped;
            Instruments = new List<string>();
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
                Instruments = Instruments,
                MarketFee = MarketFee,
                TransactionFee = TransactionFee,
                SlippageMarkup = SlippageMarkup
            };
        }
    }
}
