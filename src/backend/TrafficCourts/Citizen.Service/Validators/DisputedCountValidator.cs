using FluentValidation;
using TrafficCourts.Citizen.Service.Models.Dispute;

namespace TrafficCourts.Citizen.Service.Validators
{
    /// <summary>
    /// Fluent Validator for Disputed Count model
    /// </summary>
    public class DisputedCountValidator : AbstractValidator<DisputedCount>
    {
        public DisputedCountValidator()
        {
            RuleFor(_ => _.Plea).IsInEnum();
            RuleFor(_ => _.Count).InclusiveBetween(1,3);

            // Validation rules for properties if disputant pleaded guilty
            RuleFor(_ => _.RequestTimeToPay).NotNull()
                .When(_ => _.Plea.Equals(Plea.Guilty))
                .WithMessage("'Request Time To Pay' selection is required to be set since disputant pleaded guilty");

            RuleFor(_ => _.RequestReduction).NotNull()
                .When(_ => _.Plea.Equals(Plea.Guilty))
                .WithMessage("'Request Reduction' selection is required to be set since disputant pleaded guilty");

            RuleFor(_ => _.AppearInCourt).NotNull()
                .When(_ => _.Plea.Equals(Plea.Guilty))
                .WithMessage("'Appear In Court' selection is required to be set since disputant pleaded guilty");

            // Validation rules for properties if disputant pleaded not guilty
            RuleFor(_ => _.RequestTimeToPay).Must(x => x.Value == false || x is null)
                .When(_ => _.Plea.Equals(Plea.NotGuilty))
                .WithMessage("'Request Time To Pay' must not be true since disputant pleaded not guilty");

            RuleFor(_ => _.RequestReduction).Must(x => x.Value == false || x is null)
                .When(_ => _.Plea.Equals(Plea.NotGuilty))
                .WithMessage("'Request Reduction' must not be true since disputant pleaded not guilty");

            RuleFor(_ => _.AppearInCourt).Must(x => x.Value == true || x is null)
                .When(_ => _.Plea.Equals(Plea.NotGuilty))
                .WithMessage("'Appear In Court' must be true since disputant pleaded not guilty");
        }
    }
}
