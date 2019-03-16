using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Lykke.Service.MarketInstruments.Client.Models.AssetPairs;
using Lykke.Service.MarketInstruments.Client.Services;
using Lykke.Service.SmartOrderRouter.Domain.Entities.Exchanges;
using Lykke.Service.SmartOrderRouter.Domain.Entities.OrderBooks;
using Lykke.Service.SmartOrderRouter.Domain.Services;
using Lykke.Service.SmartOrderRouter.DomainServices.OrderBooks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Lykke.Service.SmartOrderRouter.DomainServices.Tests.OrderBooks
{
    [TestClass]
    public class ExternalOrderBookServiceTests
    {
        private readonly Mock<IExchangeSettingsService> _exchangeSettingsServiceMock =
            new Mock<IExchangeSettingsService>();

        private readonly Mock<IMarketInstrumentService> _marketInstrumentServiceMock =
            new Mock<IMarketInstrumentService>();

        private readonly List<AssetPairModel> _assetPairs = new List<AssetPairModel>();

        private readonly List<ExchangeSettings> _exchangeSettings = new List<ExchangeSettings>();

        private ExternalOrderBookService _service;

        [TestInitialize]
        public void TestInitialize()
        {
            _exchangeSettingsServiceMock.Setup(o => o.GetByNameAsync(It.IsAny<string>()))
                .Returns((string exchange) =>
                    Task.FromResult(_exchangeSettings.FirstOrDefault(o => o.Name == exchange)));

            _marketInstrumentServiceMock.Setup(o => o.GetAssetPair(It.IsAny<string>(), It.IsAny<string>()))
                .Returns((string name, string exchange) =>
                    _assetPairs.FirstOrDefault(o => o.Name == name && o.Exchange == exchange));
            
            _service = new ExternalOrderBookService(
                _exchangeSettingsServiceMock.Object,
                _marketInstrumentServiceMock.Object);
        }

        [TestMethod]
        public async Task Create_Order_Book()
        {
            // arrange

            string assetPair = "BTCUSD";

            var exchangeSettings = new ExchangeSettings
            {
                Name = "exchange1",
                MarketFee = .1m,
                TransactionFee = .01m
            };

            _exchangeSettings.Add(exchangeSettings);

            var assetPairSettings = new AssetPairModel
            {
                Exchange = exchangeSettings.Name,
                Name = assetPair,
                PriceAccuracy = 3
            };

            _assetPairs.Add(assetPairSettings);


            var sellLevels = new List<OrderBookLevel>
            {
                new OrderBookLevel {Price = 1.732235m, Volume = 3},
                new OrderBookLevel {Price = 0.833000m, Volume = 1}
            };

            var buyLevels = new List<OrderBookLevel>
            {
                new OrderBookLevel {Price = 0.712397m, Volume = 1},
                new OrderBookLevel {Price = 0.111000m, Volume = 5}
            };

            var expectedExternalOrderBook = new ExternalOrderBook
            {
                Exchange = exchangeSettings.Name,
                Timestamp = DateTime.UtcNow,
                AssetPair = assetPair,
                SellLevels = sellLevels
                    .Select(o => new ExternalOrderBookLevel
                    {
                        Price = (o.Price * (1 + exchangeSettings.MarketFee + exchangeSettings.TransactionFee))
                            .TruncateDecimalPlaces(assetPairSettings.PriceAccuracy, true),
                        Volume = o.Volume,
                        Markup = exchangeSettings.MarketFee + exchangeSettings.TransactionFee,
                        OriginalPrice = o.Price
                    })
                    .ToList(),
                BuyLevels = buyLevels
                    .Select(o => new ExternalOrderBookLevel
                    {
                        Price = (o.Price * (1 - exchangeSettings.MarketFee - exchangeSettings.TransactionFee))
                            .TruncateDecimalPlaces(assetPairSettings.PriceAccuracy),
                        Volume = o.Volume,
                        Markup = exchangeSettings.MarketFee + exchangeSettings.TransactionFee,
                        OriginalPrice = o.Price
                    })
                    .ToList()
            };

            // act

            await _service.UpdateAsync(exchangeSettings.Name, assetPair, DateTime.UtcNow, sellLevels, buyLevels);

            ExternalOrderBook actualExternalOrderBook = _service.GetByAssetPair(assetPair).Single();

            // assert

            Assert.IsTrue(AreEqual(expectedExternalOrderBook, actualExternalOrderBook));
        }

        private static bool AreEqual(ExternalOrderBook a, ExternalOrderBook b)
        {
            if (a == null && b == null)
                return true;

            if (a == null || b == null)
                return false;

            if (a.AssetPair != b.AssetPair || a.Exchange != b.Exchange)
                return false;

            return AreEqual(a.SellLevels, b.SellLevels) && AreEqual(a.BuyLevels, b.BuyLevels);
        }

        private static bool AreEqual(IReadOnlyCollection<ExternalOrderBookLevel> a,
            IReadOnlyCollection<ExternalOrderBookLevel> b)
        {
            return a.Count == b.Count && a.All(o => b.Any(p => AreEqual(o, p)));
        }

        private static bool AreEqual(ExternalOrderBookLevel a, ExternalOrderBookLevel b)
        {
            return a.Price == b.Price &&
                   a.Volume == b.Volume &&
                   a.Markup == b.Markup &&
                   a.OriginalPrice == b.OriginalPrice;
        }
    }
}
