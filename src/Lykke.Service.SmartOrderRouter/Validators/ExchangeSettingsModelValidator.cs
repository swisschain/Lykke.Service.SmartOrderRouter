using FluentValidation;
using JetBrains.Annotations;
using Lykke.Service.SmartOrderRouter.Client.Models.Exchanges;

namespace Lykke.Service.SmartOrderRouter.Validators
{
    [UsedImplicitly]
    public class ExchangeSettingsModelValidator : AbstractValidator<ExchangeSettingsModel>
    {
        public ExchangeSettingsModelValidator()
        {
            RuleFor(o => o.Name)
                .NotEmpty()
                .WithMessage("Name required");

            RuleFor(o => o.Status)
                .NotEqual(ExchangeStatus.None)
                .WithMessage("Status required");

            RuleFor(o => o.MarketFee)
                .InclusiveBetween(0, 1)
                .WithMessage("Market fee should be between 0 and 1");

            RuleFor(o => o.TransactionFee)
                .InclusiveBetween(0, 1)
                .WithMessage("Transaction fee should be between 0 and 1");
        }
    }
}
