using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Logs;
using Lykke.Service.MarketInstruments.Client.Services;
using Lykke.Service.SmartOrderRouter.Domain;
using Lykke.Service.SmartOrderRouter.Domain.Entities.Exchanges;
using Lykke.Service.SmartOrderRouter.Domain.Entities.OrderBooks;
using Lykke.Service.SmartOrderRouter.Domain.Entities.Orders;
using Lykke.Service.SmartOrderRouter.Domain.Exceptions;
using Lykke.Service.SmartOrderRouter.Domain.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Lykke.Service.SmartOrderRouter.DomainServices.Tests
{
    [TestClass]
    public class SmartOrderRouterTests
    {
        private const string Exchange1 = "e1";
        private const string Exchange2 = "e2";
        private const string Exchange3 = "e3";
        private const string AssetPair = "BTCUSD";

        private readonly Mock<IMarketOrderService> _marketOrderServiceMock =
            new Mock<IMarketOrderService>();

        private readonly Mock<IExternalLimitOrderService> _externalLimitOrderServiceMock =
            new Mock<IExternalLimitOrderService>();

        private readonly Mock<IAggregatedOrderBookService> _aggregatedOrderBookServiceMock =
            new Mock<IAggregatedOrderBookService>();

        private readonly Mock<IExchangeSettingsService> _exchangeSettingsServiceMock =
            new Mock<IExchangeSettingsService>();

        private readonly Mock<IMarketInstrumentService> _marketInstrumentServiceMock =
            new Mock<IMarketInstrumentService>();

        private readonly Mock<ISettingsService> _settingsServiceMock =
            new Mock<ISettingsService>();

        private readonly Mock<IExchangeService> _exchangeServiceMock =
            new Mock<IExchangeService>();

        private readonly Dictionary<string, List<ExternalLimitOrder>> _externalLimitOrders =
            new Dictionary<string, List<ExternalLimitOrder>>();

        private readonly AggregatedOrderBook _aggregatedOrderBook = new AggregatedOrderBook
        {
            AssetPair = AssetPair,
            Timestamp = DateTime.UtcNow,
            SellLevels = new[]
            {
                new AggregatedOrderBookLevel
                {
                    Price = 3300, Volume = 18, ExchangeVolumes = new[]
                    {
                        new AggregatedOrderBookVolume(Exchange2, 3300, 10),
                        new AggregatedOrderBookVolume(Exchange3, 3300, 8)
                    }
                },
                new AggregatedOrderBookLevel
                {
                    Price = 3250, Volume = 20, ExchangeVolumes = new[]
                    {
                        new AggregatedOrderBookVolume(Exchange1, 3250, 10),
                        new AggregatedOrderBookVolume(Exchange2, 3250, 10)
                    }
                },
                new AggregatedOrderBookLevel
                {
                    Price = 3200, Volume = 29, ExchangeVolumes = new[]
                    {
                        new AggregatedOrderBookVolume(Exchange1, 3200, 10),
                        new AggregatedOrderBookVolume(Exchange2, 3200, 4),
                        new AggregatedOrderBookVolume(Exchange3, 3200, 15)
                    }
                },
                new AggregatedOrderBookLevel
                {
                    Price = 3150, Volume = 15, ExchangeVolumes = new[]
                    {
                        new AggregatedOrderBookVolume(Exchange1, 3150, 10),
                        new AggregatedOrderBookVolume(Exchange3, 3150, 5)
                    }
                },
                new AggregatedOrderBookLevel
                {
                    Price = 3100, Volume = 3, ExchangeVolumes = new[]
                    {
                        new AggregatedOrderBookVolume(Exchange1, 3100, 3)
                    }
                }
            },
            BuyLevels = new[]
            {
                new AggregatedOrderBookLevel
                {
                    Price = 3150, Volume = 3, ExchangeVolumes = new[]
                    {
                        new AggregatedOrderBookVolume(Exchange2, 3150, 3)
                    }
                },
                new AggregatedOrderBookLevel
                {
                    Price = 3000, Volume = 15, ExchangeVolumes = new[]
                    {
                        new AggregatedOrderBookVolume(Exchange2, 3000, 10),
                        new AggregatedOrderBookVolume(Exchange3, 3000, 5)
                    }
                },
                new AggregatedOrderBookLevel
                {
                    Price = 2900, Volume = 20, ExchangeVolumes = new[]
                    {
                        new AggregatedOrderBookVolume(Exchange1, 2900, 3),
                        new AggregatedOrderBookVolume(Exchange2, 2900, 10),
                        new AggregatedOrderBookVolume(Exchange3, 2900, 7)
                    }
                },
                new AggregatedOrderBookLevel
                {
                    Price = 2800, Volume = 20, ExchangeVolumes = new[]
                    {
                        new AggregatedOrderBookVolume(Exchange1, 2800, 10),
                        new AggregatedOrderBookVolume(Exchange2, 2800, 10)
                    }
                },
                new AggregatedOrderBookLevel
                {
                    Price = 2700, Volume = 10, ExchangeVolumes = new[]
                    {
                        new AggregatedOrderBookVolume(Exchange1, 2700, 10)
                    }
                }
            }
        };

        private readonly Dictionary<string, ExchangeSettings> _exchangeSettings =
            new Dictionary<string, ExchangeSettings>
            {
                {Exchange1, new ExchangeSettings(Exchange1) {Status = ExchangeStatus.Active}},
                {Exchange2, new ExchangeSettings(Exchange2) {Status = ExchangeStatus.Active}},
                {Exchange3, new ExchangeSettings(Exchange3) {Status = ExchangeStatus.Active}}
            };

        private ISmartOrderRouter _service;

        [TestInitialize]
        public void TestInitialize()
        {
            _externalLimitOrderServiceMock.Setup(o => o.GetByParentIdAsync(It.IsAny<string>()))
                .Returns((string parentId) =>
                {
                    _externalLimitOrders.TryGetValue(parentId, out List<ExternalLimitOrder> externalLimitOrders);
                    return Task.FromResult<IReadOnlyList<ExternalLimitOrder>>(
                        externalLimitOrders ?? new List<ExternalLimitOrder>());
                });

            _aggregatedOrderBookServiceMock.Setup(o => o.GetByAssetPair(It.IsAny<string>()))
                .Returns((string assetPair) => _aggregatedOrderBook.AssetPair == assetPair
                    ? _aggregatedOrderBook
                    : null);

            _exchangeSettingsServiceMock.Setup(o => o.GetByNameAsync(It.IsAny<string>()))
                .Returns((string exchangeName) =>
                {
                    _exchangeSettings.TryGetValue(exchangeName, out ExchangeSettings exchangeSettings);
                    return Task.FromResult(exchangeSettings);
                });

            _service = new SmartOrderRouter(
                _marketOrderServiceMock.Object,
                _externalLimitOrderServiceMock.Object,
                _aggregatedOrderBookServiceMock.Object,
                _exchangeSettingsServiceMock.Object,
                _marketInstrumentServiceMock.Object,
                _settingsServiceMock.Object,
                _exchangeServiceMock.Object,
                EmptyLogFactory.Instance);
        }

        [TestMethod]
        public async Task Reroute_One_Failed_Limit_Order()
        {
            // arrange

            var marketOrder = new MarketOrder("tmp", AssetPair, OrderType.Buy, 47) {Status = MarketOrderStatus.Active};

            _marketOrderServiceMock.Setup(o => o.GetNextActiveAsync())
                .Returns(Task.FromResult(marketOrder));

            _exchangeServiceMock.Setup(o => o.CreateLimitOrderAsync(
                    It.Is<string>(exchangeName => exchangeName == Exchange3),
                    It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<decimal>(), It.IsAny<OrderType>()))
                .Returns((string exchangeName, string assetPair, decimal price, decimal volume, OrderType orderType) =>
                    Task.FromResult(new ExternalLimitOrder(Guid.NewGuid().ToString(), exchangeName, assetPair,
                        price, volume, orderType)));

            _exchangeServiceMock.Setup(o => o.CreateLimitOrderAsync(
                    It.Is<string>(exchangeName => exchangeName == Exchange1 || exchangeName == Exchange2),
                    It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<decimal>(), It.IsAny<OrderType>()))
                .Throws(new FailedOperationException("error"));

            var expectedExternalLimitOrders = new List<ExternalLimitOrder>
            {
                new ExternalLimitOrder(Guid.NewGuid().ToString(), Exchange3, AssetPair,
                    3200, 20, OrderType.Buy),
                new ExternalLimitOrder(Guid.NewGuid().ToString(), Exchange3, AssetPair,
                    3300, 8, OrderType.Buy)
            };

            var actualExternalLimitOrders = new List<ExternalLimitOrder>();

            _externalLimitOrderServiceMock.Setup(o => o.AddAsync(It.IsAny<string>(), It.IsAny<ExternalLimitOrder>()))
                .Returns(Task.CompletedTask)
                .Callback((string parentId, ExternalLimitOrder externalLimitOrder) =>
                    actualExternalLimitOrders.Add(externalLimitOrder));

            // act

            await _service.ExecuteMarketOrdersAsync();

            // assert

            Assert.IsTrue(AreEqual(expectedExternalLimitOrders, actualExternalLimitOrders));
        }

        private static bool AreEqual(IReadOnlyCollection<ExternalLimitOrder> a,
            IReadOnlyCollection<ExternalLimitOrder> b)
        {
            if (a == null && b == null)
                return true;

            if (a != null && b == null || a == null || a.Count != b.Count)
                return false;

            return a.All(o => b.Any(p =>
                p.Exchange == o.Exchange && p.Price == o.Price && p.Volume == o.Volume && p.Type == o.Type));
        }
    }
}
