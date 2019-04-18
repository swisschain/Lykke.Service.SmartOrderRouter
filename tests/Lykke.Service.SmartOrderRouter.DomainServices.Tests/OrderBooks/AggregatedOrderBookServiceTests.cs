using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Service.MarketInstruments.Client.Models.AssetPairs;
using Lykke.Service.MarketInstruments.Client.Services;
using Lykke.Service.SmartOrderRouter.Domain.Entities.Balances;
using Lykke.Service.SmartOrderRouter.Domain.Entities.OrderBooks;
using Lykke.Service.SmartOrderRouter.Domain.Services;
using Lykke.Service.SmartOrderRouter.DomainServices.OrderBooks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Lykke.Service.SmartOrderRouter.DomainServices.Tests.OrderBooks
{
    [TestClass]
    public class AggregatedOrderBookServiceTests
    {
        private const string Exchange = "e";
        private const string Exchange1 = "e1";
        private const string Exchange2 = "e2";
        private const string Exchange3 = "e3";
        private const string AssetPair = "BTCUSD";
        private const string BaseAsset = "BTC";
        private const string QuoteAsset = "USD";

        private readonly Mock<IExternalOrderBookService> _externalOrderBookServiceMock =
            new Mock<IExternalOrderBookService>();

        private readonly Mock<IMarketInstrumentService> _marketInstrumentsServiceMock =
            new Mock<IMarketInstrumentService>();

        private readonly Mock<IBalanceService> _balanceServiceMock =
            new Mock<IBalanceService>();

        private readonly Mock<ISettingsService> _settingsServiceMock =
            new Mock<ISettingsService>();

        private readonly List<AssetPairModel> _assetPairs = new List<AssetPairModel>
        {
            new AssetPairModel
            {
                Name = AssetPair,
                PriceAccuracy = 2,
                VolumeAccuracy = 8,
                BaseAsset = BaseAsset,
                QuoteAsset = QuoteAsset,
                Exchange = Exchange
            },
            new AssetPairModel
            {
                Name = AssetPair,
                PriceAccuracy = 3,
                VolumeAccuracy = 8,
                BaseAsset = BaseAsset,
                QuoteAsset = QuoteAsset,
                Exchange = Exchange1
            },
            new AssetPairModel
            {
                Name = AssetPair,
                PriceAccuracy = 4,
                VolumeAccuracy = 8,
                BaseAsset = BaseAsset,
                QuoteAsset = QuoteAsset,
                Exchange = Exchange2
            },
            new AssetPairModel
            {
                Name = AssetPair,
                PriceAccuracy = 5,
                VolumeAccuracy = 8,
                BaseAsset = BaseAsset,
                QuoteAsset = QuoteAsset,
                Exchange = Exchange3
            }
        };

        private readonly List<ExternalOrderBook> _externalOrderBooks = new List<ExternalOrderBook>
        {
            new ExternalOrderBook
            {
                Exchange = Exchange1,
                AssetPair = AssetPair,
                Timestamp = DateTime.UtcNow,
                SellLevels = new List<ExternalOrderBookLevel>
                {
                    new ExternalOrderBookLevel {Price = 109.500m, Volume = 3},
                    new ExternalOrderBookLevel {Price = 105.800m, Volume = 2},
                    new ExternalOrderBookLevel {Price = 100.000m, Volume = 1}
                },
                BuyLevels = new List<ExternalOrderBookLevel>
                {
                    new ExternalOrderBookLevel {Price = 95.000m, Volume = 1},
                    new ExternalOrderBookLevel {Price = 93.000m, Volume = 2},
                    new ExternalOrderBookLevel {Price = 87.562m, Volume = 3}
                }
            },
            new ExternalOrderBook
            {
                Exchange = Exchange2,
                AssetPair = AssetPair,
                Timestamp = DateTime.UtcNow,
                SellLevels = new List<ExternalOrderBookLevel>
                {
                    new ExternalOrderBookLevel {Price = 107.8770m, Volume = 3},
                    new ExternalOrderBookLevel {Price = 105.8000m, Volume = 2},
                    new ExternalOrderBookLevel {Price = 100.0000m, Volume = 1}
                },
                BuyLevels = new List<ExternalOrderBookLevel>
                {
                    new ExternalOrderBookLevel {Price = 99.5760m, Volume = 1},
                    new ExternalOrderBookLevel {Price = 96.0000m, Volume = 2},
                    new ExternalOrderBookLevel {Price = 93.0000m, Volume = 3}
                }
            },
            new ExternalOrderBook
            {
                Exchange = Exchange3,
                AssetPair = AssetPair,
                Timestamp = DateTime.UtcNow,
                SellLevels = new List<ExternalOrderBookLevel>
                {
                    new ExternalOrderBookLevel {Price = 101.40001m, Volume = 3},
                    new ExternalOrderBookLevel {Price = 100.00000m, Volume = 2},
                    new ExternalOrderBookLevel {Price = 98.00000m, Volume = 1}
                },
                BuyLevels = new List<ExternalOrderBookLevel>
                {
                    new ExternalOrderBookLevel {Price = 99.50000m, Volume = 1},
                    new ExternalOrderBookLevel {Price = 96.00005m, Volume = 2},
                    new ExternalOrderBookLevel {Price = 93.00000m, Volume = 3}
                }
            }
        };

        private readonly List<Balance> _balances = new List<Balance>();

        private AggregatedOrderBookService _service;

        [TestInitialize]
        public void TestInitialize()
        {
            _externalOrderBookServiceMock.Setup(o => o.GetByAssetPair(It.IsAny<string>()))
                .Returns((string assetPair) => _externalOrderBooks.Where(o => o.AssetPair == assetPair).ToList());

            _marketInstrumentsServiceMock.Setup(o => o.GetAssetPair(It.IsAny<string>(), It.IsAny<string>()))
                .Returns((string name, string exchange) =>
                    _assetPairs.FirstOrDefault(o => o.Name == name && o.Exchange == exchange));

            _balanceServiceMock.Setup(o => o.Get(It.IsAny<string>(), It.IsAny<string>()))
                .Returns((string exchange, string asset) =>
                    _balances.FirstOrDefault(o => o.Exchange == exchange && o.Asset == asset) ??
                    new Balance(exchange, asset, decimal.Zero, decimal.Zero));

            _settingsServiceMock.Setup(o => o.GetExchangeName())
                .Returns(Exchange);

            _service = new AggregatedOrderBookService(
                _externalOrderBookServiceMock.Object,
                _marketInstrumentsServiceMock.Object,
                _balanceServiceMock.Object,
                _settingsServiceMock.Object);
        }

        [TestMethod]
        public async Task Create_Order_Book_With_Unlimited_Balance()
        {
            // arrange

            _balances.Add(new Balance(Exchange1, BaseAsset, int.MaxValue, decimal.Zero));
            _balances.Add(new Balance(Exchange1, QuoteAsset, int.MaxValue, decimal.Zero));
            _balances.Add(new Balance(Exchange2, BaseAsset, int.MaxValue, decimal.Zero));
            _balances.Add(new Balance(Exchange2, QuoteAsset, int.MaxValue, decimal.Zero));
            _balances.Add(new Balance(Exchange3, BaseAsset, int.MaxValue, decimal.Zero));
            _balances.Add(new Balance(Exchange3, QuoteAsset, int.MaxValue, decimal.Zero));

            var expectedAggregatedOrderBook = new AggregatedOrderBook
            {
                AssetPair = AssetPair,
                Timestamp = DateTime.UtcNow,
                SellLevels = new List<AggregatedOrderBookLevel>
                {
                    new AggregatedOrderBookLevel
                    {
                        Price = 98.00m, Volume = 1, ExchangeVolumes = new List<AggregatedOrderBookVolume>
                        {
                            new AggregatedOrderBookVolume {Exchange = Exchange3, Volume = 1}
                        }
                    },
                    new AggregatedOrderBookLevel
                    {
                        Price = 100.00m, Volume = 4, ExchangeVolumes = new List<AggregatedOrderBookVolume>
                        {
                            new AggregatedOrderBookVolume {Exchange = Exchange1, Volume = 1},
                            new AggregatedOrderBookVolume {Exchange = Exchange2, Volume = 1},
                            new AggregatedOrderBookVolume {Exchange = Exchange3, Volume = 2}
                        }
                    },
                    new AggregatedOrderBookLevel
                    {
                        Price = 101.41m, Volume = 3, ExchangeVolumes = new List<AggregatedOrderBookVolume>
                        {
                            new AggregatedOrderBookVolume {Exchange = Exchange3, Volume = 3}
                        }
                    },
                    new AggregatedOrderBookLevel
                    {
                        Price = 105.8m, Volume = 4, ExchangeVolumes = new List<AggregatedOrderBookVolume>
                        {
                            new AggregatedOrderBookVolume {Exchange = Exchange1, Volume = 2},
                            new AggregatedOrderBookVolume {Exchange = Exchange2, Volume = 2}
                        }
                    },
                    new AggregatedOrderBookLevel
                    {
                        Price = 107.88m, Volume = 3, ExchangeVolumes = new List<AggregatedOrderBookVolume>
                        {
                            new AggregatedOrderBookVolume {Exchange = Exchange2, Volume = 3}
                        }
                    },
                    new AggregatedOrderBookLevel
                    {
                        Price = 109.5m, Volume = 3, ExchangeVolumes = new List<AggregatedOrderBookVolume>
                        {
                            new AggregatedOrderBookVolume {Exchange = Exchange1, Volume = 3}
                        }
                    }
                },
                BuyLevels = new List<AggregatedOrderBookLevel>
                {
                    new AggregatedOrderBookLevel
                    {
                        Price = 99.57m, Volume = 1, ExchangeVolumes = new List<AggregatedOrderBookVolume>
                        {
                            new AggregatedOrderBookVolume {Exchange = Exchange2, Volume = 1}
                        }
                    },
                    new AggregatedOrderBookLevel
                    {
                        Price = 99.50m, Volume = 1, ExchangeVolumes = new List<AggregatedOrderBookVolume>
                        {
                            new AggregatedOrderBookVolume {Exchange = Exchange3, Volume = 1}
                        }
                    },
                    new AggregatedOrderBookLevel
                    {
                        Price = 96.00m, Volume = 4, ExchangeVolumes = new List<AggregatedOrderBookVolume>
                        {
                            new AggregatedOrderBookVolume {Exchange = Exchange2, Volume = 2},
                            new AggregatedOrderBookVolume {Exchange = Exchange3, Volume = 2}
                        }
                    },
                    new AggregatedOrderBookLevel
                    {
                        Price = 95.00m, Volume = 1, ExchangeVolumes = new List<AggregatedOrderBookVolume>
                        {
                            new AggregatedOrderBookVolume {Exchange = Exchange1, Volume = 1}
                        }
                    },
                    new AggregatedOrderBookLevel
                    {
                        Price = 93.00m, Volume = 8, ExchangeVolumes = new List<AggregatedOrderBookVolume>
                        {
                            new AggregatedOrderBookVolume {Exchange = Exchange1, Volume = 2},
                            new AggregatedOrderBookVolume {Exchange = Exchange2, Volume = 3},
                            new AggregatedOrderBookVolume {Exchange = Exchange3, Volume = 3}
                        }
                    },
                    new AggregatedOrderBookLevel
                    {
                        Price = 87.56m, Volume = 3, ExchangeVolumes = new List<AggregatedOrderBookVolume>
                        {
                            new AggregatedOrderBookVolume {Exchange = Exchange1, Volume = 3}
                        }
                    }
                }
            };

            // act

            await _service.UpdateAsync(AssetPair);

            var actualAggregatedOrderBook = _service.GetByAssetPair(AssetPair);

            // assert

            Assert.IsTrue(AreEqual(expectedAggregatedOrderBook, actualAggregatedOrderBook));
        }

                [TestMethod]
        public async Task Create_Order_Book_With_Limited_Balance()
        {
            // arrange

            _balances.Add(new Balance(Exchange1, BaseAsset, 4, decimal.Zero));
            _balances.Add(new Balance(Exchange1, QuoteAsset, 368.562m, decimal.Zero));
            _balances.Add(new Balance(Exchange2, BaseAsset, 5, decimal.Zero));
            _balances.Add(new Balance(Exchange2, QuoteAsset, 477.576m, decimal.Zero));
            _balances.Add(new Balance(Exchange3, BaseAsset, 2, decimal.Zero));
            _balances.Add(new Balance(Exchange3, QuoteAsset, 195.50005m, decimal.Zero));

            var expectedAggregatedOrderBook = new AggregatedOrderBook
            {
                AssetPair = AssetPair,
                Timestamp = DateTime.UtcNow,
                SellLevels = new List<AggregatedOrderBookLevel>
                {
                    new AggregatedOrderBookLevel
                    {
                        Price = 98.00m, Volume = 1, ExchangeVolumes = new List<AggregatedOrderBookVolume>
                        {
                            new AggregatedOrderBookVolume {Exchange = Exchange3, Volume = 1}
                        }
                    },
                    new AggregatedOrderBookLevel
                    {
                        Price = 100.00m, Volume = 3, ExchangeVolumes = new List<AggregatedOrderBookVolume>
                        {
                            new AggregatedOrderBookVolume {Exchange = Exchange1, Volume = 1},
                            new AggregatedOrderBookVolume {Exchange = Exchange2, Volume = 1},
                            new AggregatedOrderBookVolume {Exchange = Exchange3, Volume = 1}
                        }
                    },
                    new AggregatedOrderBookLevel
                    {
                        Price = 105.8m, Volume = 4, ExchangeVolumes = new List<AggregatedOrderBookVolume>
                        {
                            new AggregatedOrderBookVolume {Exchange = Exchange1, Volume = 2},
                            new AggregatedOrderBookVolume {Exchange = Exchange2, Volume = 2}
                        }
                    },
                    new AggregatedOrderBookLevel
                    {
                        Price = 107.88m, Volume = 2, ExchangeVolumes = new List<AggregatedOrderBookVolume>
                        {
                            new AggregatedOrderBookVolume {Exchange = Exchange2, Volume = 2}
                        }
                    },
                    new AggregatedOrderBookLevel
                    {
                        Price = 109.5m, Volume = 1, ExchangeVolumes = new List<AggregatedOrderBookVolume>
                        {
                            new AggregatedOrderBookVolume {Exchange = Exchange1, Volume = 1}
                        }
                    }
                },
                BuyLevels = new List<AggregatedOrderBookLevel>
                {
                    new AggregatedOrderBookLevel
                    {
                        Price = 99.57m, Volume = 1, ExchangeVolumes = new List<AggregatedOrderBookVolume>
                        {
                            new AggregatedOrderBookVolume {Exchange = Exchange2, Volume = 1}
                        }
                    },
                    new AggregatedOrderBookLevel
                    {
                        Price = 99.50m, Volume = 1, ExchangeVolumes = new List<AggregatedOrderBookVolume>
                        {
                            new AggregatedOrderBookVolume {Exchange = Exchange3, Volume = 1}
                        }
                    },
                    new AggregatedOrderBookLevel
                    {
                        Price = 96.00m, Volume = 3, ExchangeVolumes = new List<AggregatedOrderBookVolume>
                        {
                            new AggregatedOrderBookVolume {Exchange = Exchange2, Volume = 2},
                            new AggregatedOrderBookVolume {Exchange = Exchange3, Volume = 1}
                        }
                    },
                    new AggregatedOrderBookLevel
                    {
                        Price = 95.00m, Volume = 1, ExchangeVolumes = new List<AggregatedOrderBookVolume>
                        {
                            new AggregatedOrderBookVolume {Exchange = Exchange1, Volume = 1}
                        }
                    },
                    new AggregatedOrderBookLevel
                    {
                        Price = 93.00m, Volume = 4, ExchangeVolumes = new List<AggregatedOrderBookVolume>
                        {
                            new AggregatedOrderBookVolume {Exchange = Exchange1, Volume = 2},
                            new AggregatedOrderBookVolume {Exchange = Exchange2, Volume = 2}
                        }
                    },
                    new AggregatedOrderBookLevel
                    {
                        Price = 87.56m, Volume = 1, ExchangeVolumes = new List<AggregatedOrderBookVolume>
                        {
                            new AggregatedOrderBookVolume {Exchange = Exchange1, Volume = 1}
                        }
                    }
                }
            };

            // act

            await _service.UpdateAsync(AssetPair);

            var actualAggregatedOrderBook = _service.GetByAssetPair(AssetPair);

            // assert

            Assert.IsTrue(AreEqual(expectedAggregatedOrderBook, actualAggregatedOrderBook));
        }
        
        [TestMethod]
        public async Task Create_Order_Book_With_Zero_Balance_Except_Exchange3()
        {
            // arrange

            _balances.Add(new Balance(Exchange3, BaseAsset, int.MaxValue, decimal.Zero));
            _balances.Add(new Balance(Exchange3, QuoteAsset, int.MaxValue, decimal.Zero));

            var expectedAggregatedOrderBook = new AggregatedOrderBook
            {
                AssetPair = AssetPair,
                Timestamp = DateTime.UtcNow,
                SellLevels = new List<AggregatedOrderBookLevel>
                {
                    new AggregatedOrderBookLevel
                    {
                        Price = 98.00m, Volume = 1, ExchangeVolumes = new List<AggregatedOrderBookVolume>
                        {
                            new AggregatedOrderBookVolume {Exchange = Exchange3, Volume = 1}
                        }
                    },
                    new AggregatedOrderBookLevel
                    {
                        Price = 100.00m, Volume = 2, ExchangeVolumes = new List<AggregatedOrderBookVolume>
                        {
                            new AggregatedOrderBookVolume {Exchange = Exchange3, Volume = 2}
                        }
                    },
                    new AggregatedOrderBookLevel
                    {
                        Price = 101.41m, Volume = 3, ExchangeVolumes = new List<AggregatedOrderBookVolume>
                        {
                            new AggregatedOrderBookVolume {Exchange = Exchange3, Volume = 3}
                        }
                    }
                },
                BuyLevels = new List<AggregatedOrderBookLevel>
                {
                    new AggregatedOrderBookLevel
                    {
                        Price = 99.50m, Volume = 1, ExchangeVolumes = new List<AggregatedOrderBookVolume>
                        {
                            new AggregatedOrderBookVolume {Exchange = Exchange3, Volume = 1}
                        }
                    },
                    new AggregatedOrderBookLevel
                    {
                        Price = 96.00m, Volume = 2, ExchangeVolumes = new List<AggregatedOrderBookVolume>
                        {
                            new AggregatedOrderBookVolume {Exchange = Exchange3, Volume = 2}
                        }
                    },
                    new AggregatedOrderBookLevel
                    {
                        Price = 93.00m, Volume = 3, ExchangeVolumes = new List<AggregatedOrderBookVolume>
                        {
                            new AggregatedOrderBookVolume {Exchange = Exchange3, Volume = 3}
                        }
                    }
                }
            };

            // act

            await _service.UpdateAsync(AssetPair);

            var actualAggregatedOrderBook = _service.GetByAssetPair(AssetPair);

            // assert

            Assert.IsTrue(AreEqual(expectedAggregatedOrderBook, actualAggregatedOrderBook));
        }
        
        private static bool AreEqual(AggregatedOrderBook a, AggregatedOrderBook b)
        {
            if (a == null && b == null)
                return true;

            if (a == null || b == null)
                return false;

            if (a.AssetPair != b.AssetPair)
                return false;

            return AreEqual(a.SellLevels, b.SellLevels) && AreEqual(a.BuyLevels, b.BuyLevels);
        }

        private static bool AreEqual(IReadOnlyCollection<AggregatedOrderBookLevel> a,
            IReadOnlyCollection<AggregatedOrderBookLevel> b)
            => a.Count == b.Count && a.All(o => b.Any(p => AreEqual(o, p)));

        private static bool AreEqual(AggregatedOrderBookLevel a, AggregatedOrderBookLevel b)
        {
            return a.Price == b.Price &&
                   a.Volume == b.Volume &&
                   a.ExchangeVolumes.Count == b.ExchangeVolumes.Count &&
                   a.ExchangeVolumes.All(o => b.ExchangeVolumes.Any(p => AreEqual(o, p)));
        }

        private static bool AreEqual(AggregatedOrderBookVolume a, AggregatedOrderBookVolume b)
            => a.Exchange == b.Exchange && a.Price == b.Price && a.Volume == b.Volume;
    }
}
