using System;
using System.Linq;
using System.Threading.Tasks;
using Common.Log;
using JetBrains.Annotations;
using Lykke.Common.ExchangeAdapter.Contracts;
using Lykke.Common.Log;
using Lykke.RabbitMqBroker;
using Lykke.RabbitMqBroker.Subscriber;
using Lykke.Service.MarketInstruments.Client.Models.AssetPairs;
using Lykke.Service.MarketInstruments.Client.Services;
using Lykke.Service.SmartOrderRouter.Domain.Entities.OrderBooks;
using Lykke.Service.SmartOrderRouter.Domain.Services;
using Lykke.Service.SmartOrderRouter.DomainServices.Extensions;
using Lykke.Service.SmartOrderRouter.Settings.ServiceSettings.Rabbit.Subscribers;

namespace Lykke.Service.SmartOrderRouter.Rabbit.Subscribers
{
    [UsedImplicitly]
    public class OrderBookSubscriber : IDisposable
    {
        private readonly SubscriberSettings _settings;
        private readonly IOrderBookService _orderBookService;
        private readonly IMarketInstrumentService _marketInstrumentService;
        private readonly ILogFactory _logFactory;
        private readonly ILog _log;

        private RabbitMqSubscriber<OrderBook> _subscriber;

        public OrderBookSubscriber(
            SubscriberSettings settings,
            IOrderBookService orderBookService,
            IMarketInstrumentService marketInstrumentService,
            ILogFactory logFactory)
        {
            _settings = settings;
            _orderBookService = orderBookService;
            _marketInstrumentService = marketInstrumentService;
            _logFactory = logFactory;
            _log = logFactory.CreateLog(this);
        }

        public void Start()
        {
            var settings = RabbitMqSubscriptionSettings
                .ForSubscriber(_settings.ConnectionString, _settings.Exchange, _settings.QueueSuffix);

            settings.DeadLetterExchangeName = null;

            _subscriber = new RabbitMqSubscriber<OrderBook>(_logFactory, settings,
                    new ResilientErrorHandlingStrategy(_logFactory, settings, TimeSpan.FromSeconds(10)))
                .SetMessageDeserializer(new JsonMessageDeserializer<OrderBook>())
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

        private async Task ProcessMessageAsync(OrderBook orderBook)
        {
            try
            {
                AssetPairModel assetPairSettings = _marketInstrumentService.GetAssetPairs()
                    .FirstOrDefault(o => o.Exchange == orderBook.Source && o.Id == orderBook.Asset);

                if (assetPairSettings == null)
                    return;

                await _orderBookService.UpdateAsync(assetPairSettings.Exchange, assetPairSettings.Name,
                    orderBook.Timestamp,
                    orderBook.Asks.Select(orderBookItem => new OrderBookLevel
                    {
                        Price = orderBookItem.Price,
                        Volume = orderBookItem.Volume
                    }).ToList(),
                    orderBook.Bids.Select(orderBookItem => new OrderBookLevel
                    {
                        Price = orderBookItem.Price,
                        Volume = orderBookItem.Volume
                    }).ToList());
            }
            catch (Exception exception)
            {
                _log.ErrorWithDetails(exception, "An unexpected error occurred while processing external order book",
                    orderBook);
            }
        }
    }
}
