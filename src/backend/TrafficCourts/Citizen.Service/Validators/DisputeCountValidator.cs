using FluentValidation;
using TrafficCourts.Citizen.Service.Models.Dispute;
using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;

namespace TrafficCourts.Citizen.Service.Validators
{
    /// <summary>
    /// Fluent Validator for Disputed Count model
    /// </summary>
    public class DisputeCountValidator : AbstractValidator<TrafficCourts.Citizen.Service.Models.Dispute.DisputeCount>
    {
        public DisputeCountValidator()
        {
            RuleFor(_ => _.PleaCode).IsInEnum();

            // Validation rules for properties if disputant pleaded guilty
            RuleFor(_ => _.RequestTimeToPay).NotNull()
                .When(_ => _.PleaCode.Equals(DisputeCountPleaCode.G))
                .WithMessage("'Request Time To Pay' selection is required to be set since disputant pleaded guilty");

            RuleFor(_ => _.RequestReduction).NotNull()
                .When(_ => _.PleaCode.Equals(DisputeCountPleaCode.G))
                .WithMessage("'Request Reduction' selection is required to be set since disputant pleaded guilty");

            RuleFor(_ => _.RequestCourtAppearance).NotNull()
                .When(_ => _.PleaCode.Equals(DisputeCountPleaCode.G))
                .WithMessage("'Request Court Appearance' selection is required to be set since disputant pleaded guilty");

            // Validation rules for properties if disputant pleaded not guilty
            RuleFor(_ => _.RequestTimeToPay).Must(x => x == DisputeCountRequestTimeToPay.Y || x is null)
                .When(_ => _.PleaCode.Equals(DisputeCountPleaCode.N))
                .WithMessage("'Request Time To Pay' must not be true since disputant pleaded not guilty");

            RuleFor(_ => _.RequestReduction).Must(x => x == DisputeCountRequestReduction.Y || x is null)
                .When(_ => _.PleaCode.Equals(DisputeCountPleaCode.N))
                .WithMessage("'Request Reduction' must not be true since disputant pleaded not guilty");

            RuleFor(_ => _.RequestCourtAppearance).Must(x => x == DisputeCountRequestCourtAppearance.Y || x is null)
                .When(_ => _.PleaCode.Equals(DisputeCountPleaCode.N))
                .WithMessage("'Request Court Appearance' must not be false since disputant pleaded not guilty");
        }
    }
}
