using System;
using System.Collections.Generic;
using System.Linq;
using Lykke.Service.SmartOrderRouter.Domain.Entities.OrderBooks;
using Lykke.Service.SmartOrderRouter.Domain.Entities.Orders;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lykke.Service.SmartOrderRouter.Domain.Tests.Entities.OrderBooks
{
    [TestClass]
    public class AggregatedOrderBookTests
    {
        private const string Exchange1 = "e1";
        private const string Exchange2 = "e2";
        private const string Exchange3 = "e3";

        private readonly List<ExternalLimitOrder> _activeLimitOrders = new List<ExternalLimitOrder>();

        private readonly List<string> _excludedExchanges = new List<string>();

        private readonly AggregatedOrderBook _defaultOrderBook = new AggregatedOrderBook
        {
            AssetPair = "BTCUSD",
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

        [TestMethod]
        public void Get_Sell_Volume_From_First_Level()
        {
            // arrange

            const decimal volume = 3;

            var expectedVolumes = new[]
            {
                new ExchangeVolume {Exchange = Exchange1, Price = 3100, Volume = 3}
            };

            // act

            IReadOnlyList<ExchangeVolume> actualVolumes =
                _defaultOrderBook.GetSellVolumes(volume, _activeLimitOrders, _excludedExchanges);

            // assert

            Assert.IsTrue(AreEqual(expectedVolumes, actualVolumes));
        }

        [TestMethod]
        public void Get_Sell_Volume_From_Tree_Levels()
        {
            // arrange

            const decimal volume = 20;

            var expectedVolumes = new[]
            {
                new ExchangeVolume {Exchange = Exchange1, Price = 3150, Volume = 13},
                new ExchangeVolume {Exchange = Exchange3, Price = 3200, Volume = 7}
            };

            // act

            IReadOnlyList<ExchangeVolume> actualVolumes =
                _defaultOrderBook.GetSellVolumes(volume, _activeLimitOrders, _excludedExchanges);

            // assert

            Assert.IsTrue(AreEqual(expectedVolumes, actualVolumes));
        }

        [TestMethod]
        public void Get_Sell_Volume_From_Tree_Levels_With_Priority()
        {
            // arrange

            const decimal volume = 44;

            var expectedVolumes = new[]
            {
                new ExchangeVolume {Exchange = Exchange1, Price = 3200, Volume = 23},
                new ExchangeVolume {Exchange = Exchange2, Price = 3200, Volume = 1},
                new ExchangeVolume {Exchange = Exchange3, Price = 3200, Volume = 20}
            };

            // act

            IReadOnlyList<ExchangeVolume> actualVolumes =
                _defaultOrderBook.GetSellVolumes(volume, _activeLimitOrders, _excludedExchanges);

            // assert

            Assert.IsTrue(AreEqual(expectedVolumes, actualVolumes));
        }

        [TestMethod]
        public void Get_Sell_Volume_Excluding_Active_Orders()
        {
            // arrange

            const decimal volume = 67;

            _activeLimitOrders.AddRange(new[]
            {
                new ExternalLimitOrder {Exchange = Exchange1, Volume = 13},
                new ExternalLimitOrder {Exchange = Exchange2, Volume = 4}
            });

            var expectedVolumes = new[]
            {
                new ExchangeVolume {Exchange = Exchange1, Price = 3250, Volume = 20},
                new ExchangeVolume {Exchange = Exchange2, Price = 3250, Volume = 10},
                new ExchangeVolume {Exchange = Exchange3, Price = 3200, Volume = 20}
            };

            // act

            IReadOnlyList<ExchangeVolume> actualVolumes =
                _defaultOrderBook.GetSellVolumes(volume, _activeLimitOrders, _excludedExchanges);

            // assert

            Assert.IsTrue(AreEqual(expectedVolumes, actualVolumes));
        }

        [TestMethod]
        public void Get_Sell_Volume_Excluding_Exchanges()
        {
            // arrange

            const decimal volume = 24;

            _excludedExchanges.AddRange(new[] {Exchange1, Exchange3});

            var expectedVolumes = new[]
            {
                new ExchangeVolume {Exchange = Exchange2, Price = 3300, Volume = 24}
            };

            // act

            IReadOnlyList<ExchangeVolume> actualVolumes =
                _defaultOrderBook.GetSellVolumes(volume, _activeLimitOrders, _excludedExchanges);

            // assert

            Assert.IsTrue(AreEqual(expectedVolumes, actualVolumes));
        }

        [TestMethod]
        public void Get_Sell_Volume_Excluding_Active_Orders_And_Exchanges()
        {
            // arrange

            const decimal volume = 55;

            _activeLimitOrders.AddRange(new[]
            {
                new ExternalLimitOrder {Exchange = Exchange1, Volume = 13},
                new ExternalLimitOrder {Exchange = Exchange2, Volume = 4}
            });

            _excludedExchanges.AddRange(new[] {Exchange3});

            var expectedVolumes = new[]
            {
                new ExchangeVolume {Exchange = Exchange1, Price = 3250, Volume = 20},
                new ExchangeVolume {Exchange = Exchange2, Price = 3300, Volume = 18}
            };

            // act

            IReadOnlyList<ExchangeVolume> actualVolumes =
                _defaultOrderBook.GetSellVolumes(volume, _activeLimitOrders, _excludedExchanges);

            // assert

            Assert.IsTrue(AreEqual(expectedVolumes, actualVolumes));
        }

        [TestMethod]
        public void Get_Buy_Volume_From_First_Level()
        {
            // arrange

            const decimal volume = 3;

            var expectedVolumes = new[]
            {
                new ExchangeVolume {Exchange = Exchange2, Price = 3150, Volume = 3}
            };

            // act

            IReadOnlyList<ExchangeVolume> actualVolumes =
                _defaultOrderBook.GetBuyVolumes(volume, _activeLimitOrders, _excludedExchanges);

            // assert

            Assert.IsTrue(AreEqual(expectedVolumes, actualVolumes));
        }

        [TestMethod]
        public void Get_Buy_Volume_From_Tree_Levels()
        {
            // arrange

            const decimal volume = 38;

            var expectedVolumes = new[]
            {
                new ExchangeVolume {Exchange = Exchange1, Price = 2900, Volume = 3},
                new ExchangeVolume {Exchange = Exchange2, Price = 2900, Volume = 23},
                new ExchangeVolume {Exchange = Exchange3, Price = 2900, Volume = 12}
            };

            // act

            IReadOnlyList<ExchangeVolume> actualVolumes =
                _defaultOrderBook.GetBuyVolumes(volume, _activeLimitOrders, _excludedExchanges);

            // assert

            Assert.IsTrue(AreEqual(expectedVolumes, actualVolumes));
        }

        [TestMethod]
        public void Get_Buy_Volume_From_Tree_Levels_With_Priority()
        {
            // arrange

            const decimal volume = 29;

            var expectedVolumes = new[]
            {
                new ExchangeVolume {Exchange = Exchange2, Price = 2900, Volume = 23},
                new ExchangeVolume {Exchange = Exchange3, Price = 2900, Volume = 6}
            };

            // act

            IReadOnlyList<ExchangeVolume> actualVolumes =
                _defaultOrderBook.GetBuyVolumes(volume, _activeLimitOrders, _excludedExchanges);

            // assert

            Assert.IsTrue(AreEqual(expectedVolumes, actualVolumes));
        }

        [TestMethod]
        public void Get_Buy_Volume_Excluding_Active_Orders()
        {
            // arrange

            const decimal volume = 38;

            _activeLimitOrders.AddRange(new[]
            {
                new ExternalLimitOrder {Exchange = Exchange1, Volume = 3},
                new ExternalLimitOrder {Exchange = Exchange2, Volume = 8}
            });

            var expectedVolumes = new[]
            {
                new ExchangeVolume {Exchange = Exchange2, Price = 2900, Volume = 15},
                new ExchangeVolume {Exchange = Exchange3, Price = 2900, Volume = 12}
            };

            // act

            IReadOnlyList<ExchangeVolume> actualVolumes =
                _defaultOrderBook.GetBuyVolumes(volume, _activeLimitOrders, _excludedExchanges);

            // assert

            Assert.IsTrue(AreEqual(expectedVolumes, actualVolumes));
        }

        [TestMethod]
        public void Get_Buy_Volume_Excluding_Active_Orders_And_Exchanges()
        {
            // arrange

            const decimal volume = 26;

            _activeLimitOrders.AddRange(new[]
            {
                new ExternalLimitOrder {Exchange = Exchange1, Volume = 1},
                new ExternalLimitOrder {Exchange = Exchange2, Volume = 3}
            });

            _excludedExchanges.AddRange(new[] {Exchange3});

            var expectedVolumes = new[]
            {
                new ExchangeVolume {Exchange = Exchange1, Price = 2900, Volume = 2},
                new ExchangeVolume {Exchange = Exchange2, Price = 2900, Volume = 20}
            };

            // act

            IReadOnlyList<ExchangeVolume> actualVolumes =
                _defaultOrderBook.GetBuyVolumes(volume, _activeLimitOrders, _excludedExchanges);

            // assert

            Assert.IsTrue(AreEqual(expectedVolumes, actualVolumes));
        }

        private static bool AreEqual(IReadOnlyCollection<ExchangeVolume> a, IReadOnlyCollection<ExchangeVolume> b)
        {
            if (a == null && b == null)
                return true;

            if (a != null && b == null || a == null || a.Count != b.Count)
                return false;

            return a.All(o => b.Any(p => p.Exchange == o.Exchange && p.Price == o.Price && p.Volume == o.Volume));
        }
    }
}
