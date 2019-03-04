using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Sdk;
using Lykke.Service.SmartOrderRouter.Rabbit.Subscribers;

namespace Lykke.Service.SmartOrderRouter.Managers
{
    [UsedImplicitly]
    public class ShutdownManager : IShutdownManager
    {
        private readonly OrderBookSubscriber[] _orderBookSubscribers;

        public ShutdownManager(
            OrderBookSubscriber[] orderBookSubscribers)
        {
            _orderBookSubscribers = orderBookSubscribers;
        }

        public Task StopAsync()
        {
            foreach (OrderBookSubscriber orderBookSubscriber in _orderBookSubscribers)
                orderBookSubscriber.Stop();

            return Task.CompletedTask;
        }
    }
}
