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

            RuleFor(_ => (int)_.CountNo).InclusiveBetween(1, 3);

            // Validation rules for properties if disputant pleaded guilty
            RuleFor(_ => _.RequestTimeToPay)
                .NotNull()
                .When(PleaGuilty)
                .WithMessage("'Request Time To Pay' selection is required to be set since disputant pleaded guilty");

            RuleFor(_ => _.RequestReduction)
                .NotNull()
                .When(PleaGuilty)
                .WithMessage("'Request Reduction' selection is required to be set since disputant pleaded guilty");

            RuleFor(_ => _.RequestReduction)
                .Must(BeYesOrNo)
                .When(PleaGuilty)
                .WithMessage("'Request Reduction' selection is must be Y or N since disputant pleaded guilty");

            RuleFor(_ => _.RequestCourtAppearance)
                .NotNull()
                .When(PleaGuilty)
                .WithMessage("'Request Court Appearance' selection is required to be set since disputant pleaded guilty");

            RuleFor(_ => _.RequestCourtAppearance)
                .Must(BeYesOrNo)
                .When(PleaGuilty)
                .WithMessage("'Request Court Appearance' selection is must be Y or N since disputant pleaded guilty");

            // Validation rules for properties if disputant pleaded not guilty
            RuleFor(_ => _.RequestTimeToPay)
                .Must(BeNoOrNull)
                .When(PleaNotGuilty)
                .WithMessage("'Request Time To Pay' must not be true since disputant pleaded not guilty");

            RuleFor(_ => _.RequestReduction)
                .Must(BeNoOrNull)
                .When(PleaNotGuilty)
                .WithMessage("'Request Reduction' must not be true since disputant pleaded not guilty");

            RuleFor(_ => _.RequestCourtAppearance)
                .Must(BeYesOrNull)
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

        private bool BeYesOrNull(DisputeCountRequestCourtAppearance? value)
        {
            return value is null || value.Value == DisputeCountRequestCourtAppearance.Y;
        }

        private bool BeNoOrNull(DisputeCountRequestTimeToPay? value)
        {
            return value is null || value.Value == DisputeCountRequestTimeToPay.N;
        }

        private bool BeNoOrNull(DisputeCountRequestReduction? value)
        {
            return value is null || value.Value == DisputeCountRequestReduction.N;
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
