using FluentValidation;
using System.Text.RegularExpressions;
using TrafficCourts.Citizen.Service.Models.Dispute;

namespace TrafficCourts.Citizen.Service.Validators
{
    /// <summary>
    /// Fluent Validator for Notice Of Dispute Model
    /// </summary>
    public class NoticeOfDisputeValidator : AbstractValidator<NoticeOfDispute>
    {
        public NoticeOfDisputeValidator()
        {
            RuleFor(_ => _.TicketNumber).NotEmpty().MaximumLength(12);
            RuleFor(_ => _.IssuedDate).NotEmpty();
            RuleFor(_ => _.Surname).NotEmpty();
            RuleFor(_ => _.GivenNames).NotEmpty();
            RuleFor(_ => _.Birthdate).NotEmpty();
            RuleFor(_ => _.DriversLicenceNumber).NotEmpty().MaximumLength(20);
            RuleFor(_ => _.DriversLicenceProvince).NotEmpty().MaximumLength(30);
            RuleFor(_ => _.Address).NotEmpty();
            RuleFor(_ => _.City).NotEmpty();
            RuleFor(_ => _.Province).NotEmpty().MaximumLength(30);
            RuleFor(_ => _.PostalCode).NotEmpty().Length(6);
            RuleFor(_ => _.HomePhoneNumber).NotEmpty()
                .Matches(new Regex(@"\(?\d{3}\)?-? *\d{3}-? *-?\d{4}"));
            RuleFor(_ => _.WorkPhoneNumber)
                .Matches(new Regex(@"\(?\d{3}\)?-? *\d{3}-? *-?\d{4}"));
            RuleFor(_ => _.EmailAddress).NotEmpty().EmailAddress();
            RuleFor(_ => _.TicketId).NotEmpty();
            RuleFor(_ => _.NumberOfWitness).InclusiveBetween(0,99);
            RuleFor(_ => _.DisputantOcrIssuesDescription).NotEmpty()
                .When(_ => _.DisputantDetectedOcrIssues)
                .WithMessage("'Disputant Ocr Issues Description' is required since disputant detected ocr issues");

            // Validation rules for Legal Representation
            RuleFor(_ => _.LegalRepresentation).NotNull()
                .When(_ => _.RepresentedByLawyer)
                .WithMessage("'Legal Representation' details are required since disputant selected to be represted by lawyer");
            RuleFor(_ => _.LegalRepresentation).InjectValidator();

            // Validation rules for Disputed Counts
            RuleFor(_ => _.DisputedCounts)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .Must(_ => _.Count > 0 && _.Count < 4)
                .WithMessage("At least 1 and maximum 3 Disputed Counts required");

            RuleForEach(_ => _.DisputedCounts).SetValidator(new DisputedCountValidator());

            RuleFor(_ => _.TimeToPayReason).NotEmpty()
                .When(_ => _.DisputedCounts != null && _.DisputedCounts.Any(count => count.RequestTimeToPay == true))
                .WithMessage("'Time To Pay Reason' cannot be null since 'Request Time To Pay' is selected for at least one of the counts");

            RuleFor(_ => _.FineReductionReason).NotEmpty()
                .When(_ => _.DisputedCounts != null && _.DisputedCounts.Any(count => count.RequestReduction == true))
                .WithMessage("'Fine Reduction Reason' cannot be null since 'Request Reduction' is selected for at least one of the counts");
        }
    }
}
