using AutoMapper;
using JetBrains.Annotations;
using Lykke.Service.SmartOrderRouter.AzureRepositories.Exchanges;
using Lykke.Service.SmartOrderRouter.Domain.Entities.Exchanges;

namespace Lykke.Service.SmartOrderRouter.AzureRepositories
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<ExchangeSettings, ExchangeSettingsEntity>(MemberList.Source);
            CreateMap<ExchangeSettingsEntity, ExchangeSettings>(MemberList.Destination);
        }
    }
}
