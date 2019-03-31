using System;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Common.ExchangeAdapter.Contracts;
using Lykke.Common.Log;
using Lykke.RabbitMqBroker;
using Lykke.RabbitMqBroker.Subscriber;
using Lykke.Service.MarketInstruments.Client.Models.AssetPairs;
using Lykke.Service.MarketInstruments.Client.Services;
using Lykke.Service.SmartOrderRouter.Domain.Entities;
using Lykke.Service.SmartOrderRouter.Domain.Services;
using Lykke.Service.SmartOrderRouter.Settings.ServiceSettings.Rabbit.Subscribers;

namespace Lykke.Service.SmartOrderRouter.Rabbit.Subscribers
{
    public class QuoteSubscriber : IDisposable
    {
        private readonly SubscriberSettings _settings;
        private readonly IQuoteService _quoteService;
        private readonly IMarketInstrumentService _marketInstrumentService;
        private readonly ILogFactory _logFactory;

        private RabbitMqSubscriber<TickPrice> _subscriber;

        public QuoteSubscriber(
            SubscriberSettings settings,
            IQuoteService quoteService,
            IMarketInstrumentService marketInstrumentService,
            ILogFactory logFactory)
        {
            _settings = settings;
            _quoteService = quoteService;
            _marketInstrumentService = marketInstrumentService;
            _logFactory = logFactory;
        }

        public void Start()
        {
            var settings = RabbitMqSubscriptionSettings
                .ForSubscriber(_settings.ConnectionString, _settings.Exchange, _settings.QueueSuffix);

            settings.DeadLetterExchangeName = null;

            _subscriber = new RabbitMqSubscriber<TickPrice>(_logFactory, settings,
                    new ResilientErrorHandlingStrategy(_logFactory, settings, TimeSpan.FromSeconds(10)))
                .SetMessageDeserializer(new JsonMessageDeserializer<TickPrice>())
                .Subscribe(ProcessMessageAsync)
                .CreateDefaultBinding()
                .Start();
        }

        public void Stop()
        {
            _subscriber?.Stop();
        }

        public void Dispose()
        {
            _subscriber?.Dispose();
        }

        private Task ProcessMessageAsync(TickPrice tickPrice)
        {
            AssetPairModel assetPairSettings = _marketInstrumentService.GetAssetPairs()
                .FirstOrDefault(o => o.Exchange == tickPrice.Source && o.Id == tickPrice.Asset);

            if (assetPairSettings != null)
            {
                _quoteService.Update(new Quote(assetPairSettings.Name, tickPrice.Timestamp, tickPrice.Ask,
                    tickPrice.Bid, tickPrice.Source));
            }

            return Task.CompletedTask;
        }
    }
}
