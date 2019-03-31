using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Sdk;
using Lykke.Service.SmartOrderRouter.Rabbit.Subscribers;
using Lykke.Service.SmartOrderRouter.Timers;

namespace Lykke.Service.SmartOrderRouter.Managers
{
    [UsedImplicitly]
    public class ShutdownManager : IShutdownManager
    {
        private readonly BalancesTimer _balancesTimer;
        private readonly SmartOrderRouterTimer _smartOrderRouterTimer;
        private readonly OrderBookSubscriber[] _orderBookSubscribers;
        private readonly QuoteSubscriber[] _quoteSubscribers;

        public ShutdownManager(
            BalancesTimer balancesTimer,
            SmartOrderRouterTimer smartOrderRouterTimer,
            OrderBookSubscriber[] orderBookSubscribers,
            QuoteSubscriber[] quoteSubscribers)
        {
            _balancesTimer = balancesTimer;
            _smartOrderRouterTimer = smartOrderRouterTimer;
            _orderBookSubscribers = orderBookSubscribers;
            _quoteSubscribers = quoteSubscribers;
        }

        public Task StopAsync()
        {
            _smartOrderRouterTimer.Stop();

            foreach (OrderBookSubscriber orderBookSubscriber in _orderBookSubscribers)
                orderBookSubscriber.Stop();

            foreach (QuoteSubscriber quoteSubscriber in _quoteSubscribers)
                quoteSubscriber.Stop();

            _balancesTimer.Stop();

            return Task.CompletedTask;
        }
    }
}
