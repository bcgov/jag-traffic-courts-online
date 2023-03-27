using FluentValidation;
using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;

namespace TrafficCourts.Citizen.Service.Validators
{
    /// <summary>
    /// Fluent Validator for Disputed Count model
    /// </summary>
    public class DisputeCountValidator : AbstractValidator<Models.Disputes.DisputeCount>
    {
        public DisputeCountValidator()
        {
            RuleFor(_ => _.PleaCode)
                .NotNull()
                .IsInEnum();

            RuleFor(_ => _.RequestReduction)
                .NotNull()
                .IsInEnum();

            RuleFor(_ => _.RequestTimeToPay)
                .NotNull()
                .IsInEnum();

            RuleFor(_ => (int)_.CountNo).InclusiveBetween(1, 3);

            RuleFor(_ => _.RequestReduction)
                .Must(BeYesOrNo)
                .WithMessage("'Request Reduction' must be yes or no.");

            RuleFor(_ => _.RequestTimeToPay)
                .Must(BeYesOrNo)
                .WithMessage("'Request Time To Pay' must be yes or no.");

            RuleFor(_ => _.RequestCourtAppearance)
                .NotNull()
                .WithMessage("'Request Court Appearance' selection is required to be set.");

            // Validation rules for properties if disputant pleaded guilty
            RuleFor(_ => _.RequestCourtAppearance)
                .Must(BeYesOrNo)
                .When(PleaGuilty)
                .WithMessage("'Request Court Appearance' selection must be Y or N since disputant pleaded guilty");

            // Validation rules for properties if disputant pleaded not guilty
            RuleFor(_ => _.RequestCourtAppearance)
                .Must(BeYes)
                .When(PleaNotGuilty)
                .WithMessage("'Request Court Appearance' must not be false since disputant pleaded not guilty");
        }

        private bool PleaGuilty(Models.Disputes.DisputeCount count)
        {
            return count.PleaCode.Equals(DisputeCountPleaCode.G);
        }

        private bool PleaNotGuilty(Models.Disputes.DisputeCount count)
        {
            return count.PleaCode.Equals(DisputeCountPleaCode.N);
        }

        private bool BeYes(DisputeCountRequestCourtAppearance? value)
        {
            return value is not null && value.Value == DisputeCountRequestCourtAppearance.Y;
        }

        private bool BeYesOrNo(DisputeCountRequestTimeToPay? value)
        {
            if (value is null) return false;
            return value.Value == DisputeCountRequestTimeToPay.Y || value.Value == DisputeCountRequestTimeToPay.N;
        }

        private bool BeYesOrNo(DisputeCountRequestReduction? value)
        {
            if (value is null) return false;
            return value.Value == DisputeCountRequestReduction.Y || value.Value == DisputeCountRequestReduction.N;
        }

        private bool BeYesOrNo(DisputeCountRequestCourtAppearance? value)
        {
            if (value is null) return false;
            return value.Value == DisputeCountRequestCourtAppearance.Y || value.Value == DisputeCountRequestCourtAppearance.N;

        }
    }
}
