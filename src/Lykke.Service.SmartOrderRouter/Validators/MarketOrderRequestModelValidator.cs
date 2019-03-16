using FluentValidation;
using JetBrains.Annotations;
using Lykke.Service.SmartOrderRouter.Client.Models;
using Lykke.Service.SmartOrderRouter.Client.Models.MarketOrders;

namespace Lykke.Service.SmartOrderRouter.Validators
{
    [UsedImplicitly]
    public class MarketOrderRequestModelValidator : AbstractValidator<MarketOrderRequestModel>
    {
        public MarketOrderRequestModelValidator()
        {
            RuleFor(o => o.ClientId)
                .NotEmpty()
                .WithMessage("Client id required");

            RuleFor(o => o.AssetPair)
                .NotEmpty()
                .WithMessage("Asset pair required");

            RuleFor(o => o.Volume)
                .GreaterThan(0)
                .WithMessage("Volume should be greater then zero");

            RuleFor(o => o.Type)
                .NotEqual(OrderType.None)
                .WithMessage("Order type required");
        }
    }
}
