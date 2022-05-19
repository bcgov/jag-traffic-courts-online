using FluentValidation;
using System.Text.RegularExpressions;
using TrafficCourts.Citizen.Service.Models.Dispute;

namespace TrafficCourts.Citizen.Service.Validators
{
    /// <summary>
    /// Fluent Validator for Legal Representation Model
    /// </summary>
    public class LegalRepresentationValidator : AbstractValidator<LegalRepresentation>
    {
        public LegalRepresentationValidator()
        {
            RuleFor(_ => _.LawFirmName).NotEmpty();
            RuleFor(_ => _.LawyerFullName).NotEmpty();
            RuleFor(_ => _.LawyerEmail).NotEmpty().EmailAddress();
            RuleFor(_ => _.LawyerPhoneNumber).NotEmpty()
                .Matches(new Regex(@"\(?\d{3}\)?-? *\d{3}-? *-?\d{4}"));
            RuleFor(_ => _.LawyerAddress).NotEmpty();
        }
    }
}
