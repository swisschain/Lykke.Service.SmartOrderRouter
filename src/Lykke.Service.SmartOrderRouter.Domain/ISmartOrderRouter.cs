using System.Threading.Tasks;

namespace Lykke.Service.SmartOrderRouter.Domain
{
    public interface ISmartOrderRouter
    {
        Task ExecuteMarketOrdersAsync();
    }
}
