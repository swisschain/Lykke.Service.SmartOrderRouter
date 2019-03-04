using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Sdk;
using Lykke.Service.SmartOrderRouter.Rabbit.Subscribers;

namespace Lykke.Service.SmartOrderRouter.Managers
{
    [UsedImplicitly]
    public class StartupManager : IStartupManager
    {
        private readonly OrderBookSubscriber[] _orderBookSubscribers;

        public StartupManager(
            OrderBookSubscriber[] orderBookSubscribers)
        {
            _orderBookSubscribers = orderBookSubscribers;
        }

        public Task StartAsync()
        {
            foreach (OrderBookSubscriber orderBookSubscriber in _orderBookSubscribers)
                orderBookSubscriber.Start();

            return Task.CompletedTask;
        }
    }
}
