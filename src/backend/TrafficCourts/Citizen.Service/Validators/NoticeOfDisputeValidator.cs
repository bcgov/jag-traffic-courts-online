﻿using FluentValidation;
using System.Text.RegularExpressions;
using TrafficCourts.Citizen.Service.Models.Dispute;
using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;

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
            RuleFor(_ => _.DisputantSurname).NotEmpty();
            RuleFor(_ => _.DisputantGivenName1).NotEmpty();
            RuleFor(_ => _.DisputantBirthdate).NotEmpty();
            RuleFor(_ => _.DriversLicenceNumber).MaximumLength(20);
            RuleFor(_ => _.DriversLicenceProvince).MaximumLength(30);
            RuleFor(_ => _.Address).NotEmpty();
            RuleFor(_ => _.AddressCity).NotEmpty();
            RuleFor(_ => _.AddressProvince).MaximumLength(30);
            RuleFor(_ => _.PostalCode).MaximumLength(6);
            RuleFor(_ => _.WorkPhoneNumber)
                .Matches(new Regex(@"\(?\d{3}\)?-? *\d{3}-? *-?\d{4}"));
            RuleFor(_ => _.EmailAddress).NotEmpty().EmailAddress();
            RuleFor(_ => _.TicketId).NotEmpty();
            RuleFor(_ => _.WitnessNo).InclusiveBetween(0,99);
            RuleFor(_ => _.DisputantOcrIssues).NotEmpty()
                .When(_ => _.DisputantDetectedOcrIssues == Common.OpenAPIs.OracleDataApi.v1_0.DisputeDisputantDetectedOcrIssues.Y)
                .WithMessage("'Disputant Ocr Issues Description' is required since disputant detected ocr issues");

            // Validation rules for Legal Representation
            RuleFor(_ => _.LawFirmName).NotNull()
                .When(_ => _.RepresentedByLawyer == Common.OpenAPIs.OracleDataApi.v1_0.DisputeRepresentedByLawyer.Y)
                .WithMessage("'Law Firm Name' is required since disputant selected to be represted by lawyer");
            RuleFor(_ => _.LawyerSurname).NotEmpty()
                .When(_ => _.RepresentedByLawyer == Common.OpenAPIs.OracleDataApi.v1_0.DisputeRepresentedByLawyer.Y)
                .WithMessage("'Lawyer Surname' is required since disputant selected to be represted by lawyer");
            RuleFor(_ => _.LawyerGivenName1).NotEmpty()
                .When(_ => _.RepresentedByLawyer == Common.OpenAPIs.OracleDataApi.v1_0.DisputeRepresentedByLawyer.Y)
                .WithMessage("'Lawyer Given Name' is required since disputant selected to be represted by lawyer");
            RuleFor(_ => _.LawyerEmail).NotEmpty().EmailAddress()
                .When(_ => _.RepresentedByLawyer == Common.OpenAPIs.OracleDataApi.v1_0.DisputeRepresentedByLawyer.Y)
                .WithMessage("'Lawyer Email' is required and must be a proper email address since disputant selected to be represted by lawyer");
            RuleFor(_ => _.LawyerPhoneNumber).NotEmpty()
                .Matches(new Regex(@"\(?\d{3}\)?-? *\d{3}-? *-?\d{4}"))
                .When(_ => _.RepresentedByLawyer == Common.OpenAPIs.OracleDataApi.v1_0.DisputeRepresentedByLawyer.Y)
                .WithMessage("'Lawyer phone number' is required and must be a proper phone number since disputant selected to be represted by lawyer");
            RuleFor(_ => _.LawyerAddress).NotEmpty()
                .When(_ => _.RepresentedByLawyer == Common.OpenAPIs.OracleDataApi.v1_0.DisputeRepresentedByLawyer.Y)
                .WithMessage("'Lawyer Address' is required since disputant selected to be represted by lawyer");

            // Validation rules for Disputed Counts
            RuleFor(_ => _.DisputeCounts)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .Must(_ => _.Count > 0 && _.Count < 4)
                .WithMessage("At least 1 and maximum 3 Disputed Counts required");

            // Legal Representation Validators
            RuleForEach(_ => _.DisputeCounts).SetValidator(new DisputeCountValidator());

            RuleFor(_ => _.TimeToPayReason).NotEmpty()
                .When(_ => _.DisputeCounts != null && _.DisputeCounts.Any(count => count.RequestTimeToPay == DisputeCountRequestTimeToPay.Y))
                .WithMessage("'Time To Pay Reason' cannot be null since 'Request Time To Pay' is selected for at least one of the counts");

            RuleFor(_ => _.FineReductionReason).NotEmpty()
                .When(_ => _.DisputeCounts != null && _.DisputeCounts.Any(count => count.RequestReduction == DisputeCountRequestReduction.Y))
                .WithMessage("'Fine Reduction Reason' cannot be null since 'Request Reduction' is selected for at least one of the counts");
        }
    }
}
