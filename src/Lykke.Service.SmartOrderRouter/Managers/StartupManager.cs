using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Sdk;
using Lykke.Service.SmartOrderRouter.Rabbit.Subscribers;
using Lykke.Service.SmartOrderRouter.Timers;

namespace Lykke.Service.SmartOrderRouter.Managers
{
    [UsedImplicitly]
    public class StartupManager : IStartupManager
    {
        private readonly BalancesTimer _balancesTimer;
        private readonly SmartOrderRouterTimer _smartOrderRouterTimer;
        private readonly OrderBookSubscriber[] _orderBookSubscribers;

        public StartupManager(
            BalancesTimer balancesTimer,
            SmartOrderRouterTimer smartOrderRouterTimer,
            OrderBookSubscriber[] orderBookSubscribers)
        {
            _balancesTimer = balancesTimer;
            _smartOrderRouterTimer = smartOrderRouterTimer;
            _orderBookSubscribers = orderBookSubscribers;
        }

        public Task StartAsync()
        {
            _balancesTimer.Start();
            
            foreach (OrderBookSubscriber orderBookSubscriber in _orderBookSubscribers)
                orderBookSubscriber.Start();

            _smartOrderRouterTimer.Start();
            
            return Task.CompletedTask;
        }
    }
}
