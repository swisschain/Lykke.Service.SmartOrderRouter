using System;
using FluentValidation;
using JetBrains.Annotations;
using Lykke.Service.SmartOrderRouter.Client.Models.Settings;

namespace Lykke.Service.SmartOrderRouter.Validators
{
    [UsedImplicitly]
    public class TimerSettingsModelValidator : AbstractValidator<TimerSettingsModel>
    {
        public TimerSettingsModelValidator()
        {
            RuleFor(o => o.Balances)
                .GreaterThan(TimeSpan.Zero)
                .WithMessage("The balances time interval should be greater than zero");

            RuleFor(o => o.MarketOrders)
                .GreaterThan(TimeSpan.Zero)
                .WithMessage("The market orders time interval should be greater than zero");
        }
    }
}
