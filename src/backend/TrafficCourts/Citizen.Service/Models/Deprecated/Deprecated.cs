using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using TrafficCourts.Citizen.Service.Models.Search;

#pragma warning disable CS8618 // these types are deprecated, ignore nullable warnings

namespace TrafficCourts.Citizen.Service.Models
{
    public static class DeprecationExtensions
    {
        /// <summary>
        /// Creates deprecated object for backward compatibility.
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public static Deprecated.TicketDispute CreateDeprecated(this TicketSearchResult result)
        {
            Deprecated.TicketDispute dispute = new();

            dispute.ViolationTicketNumber = result.ViolationTicketNumber;
            dispute.ViolationDate = result.ViolationDate.ToString("yyyy-MM-dd");
            dispute.ViolationTime = result.ViolationTime;
            dispute.Offences = new List<Deprecated.Offence>();

            foreach (var offence in result.Offences)
            {
                dispute.Offences.Add(new Deprecated.Offence
                {
                    AmountDue = offence.AmountDue,
                    OffenceDescription = offence.OffenceDescription,
                    VehicleDescription = offence.VehicleDescription!,
                    OffenceNumber = offence.OffenceNumber,
                });
            }

            return dispute;
        }
    }

    namespace Deprecated
    {
        public class TicketDispute
        {
            public string ViolationTicketNumber { get; set; }
            public string ViolationTime { get; set; }
            public string ViolationDate { get; set; }
            public Disputant Disputant { get; set; }
            public Additional Additional { get; set; }
            public List<Offence> Offences { get; set; }
            public string DiscountDueDate { get; set; }//null or has valid or invalid value
            public decimal DiscountAmount { get; set; }//25 always
        }

        [ExcludeFromCodeCoverage(Justification = "Poco")]
        public class Additional
        {
            public bool LawyerPresent { get; set; }
            public bool InterpreterRequired { get; set; }
            public string InterpreterLanguage { get; set; }
            public bool WitnessPresent { get; set; }
            public int? NumberOfWitnesses { get; set; }
            public bool RequestReduction { get; set; }
            public bool RequestMoreTime { get; set; }
            public string ReductionReason { get; set; }
            public string MoreTimeReason { get; set; }
        }

        [ExcludeFromCodeCoverage(Justification = "Poco")]
        public class Disputant
        {
            public string LastName { get; set; }
            public string GivenNames { get; set; }
            public string MailingAddress { get; set; }
            public string City { get; set; }
            public string Province { get; set; }
            public string PostalCode { get; set; }
            public string Birthdate { get; set; }
            public string EmailAddress { get; set; }
            public string DriverLicenseNumber { get; set; }
            public string DriverLicenseProvince { get; set; }
            public string PhoneNumber { get; set; }

        }

        [ExcludeFromCodeCoverage(Justification = "Poco")]
        public class Offence
        {
            public int OffenceNumber { get; set; }
            public decimal TicketedAmount { get; set; }//total
            public decimal AmountDue { get; set; } //shell ticket: the same as total. But for rsi ticket : not change, just from RSI data. actual meaning: total-paid-discount
            public string ViolationDateTime { get; set; }
            public string OffenceDescription { get; set; }
            public string VehicleDescription { get; set; }
            public decimal DiscountAmount { get; set; }//discount, always 25
            public string DiscountDueDate { get; set; }
            public string InvoiceType { get; set; }
            public string OffenceAgreementStatus { get; set; }
            public bool RequestReduction { get; set; }
            public bool RequestMoreTime { get; set; }
            public bool? ReductionAppearInCourt { get; set; }
            public string ReductionReason { get; set; }
            public string MoreTimeReason { get; set; }
            public DisputeStatus Status { get; set; }
        }

        public enum DisputeStatus
        {
            New,
            Submitted,
            InProgress,//ticket already verified
            Complete,
            Rejected,
        }


        public static class ApiResponse
        {
            public static ApiResultResponse<T> Result<T>(T result)
            {
                return new ApiResultResponse<T>(result);
            }

            public static ApiMessageResponse Message(string message)
            {
                return new ApiMessageResponse(message);
            }

            public static ApiBadRequestResponse BadRequest(ModelStateDictionary modelState)
            {
                return new ApiBadRequestResponse(modelState);
            }
        }

        public class ApiResultResponse<T>
        {
            public T Result { get; }

            public ApiResultResponse(T result)
            {
                Result = result;
            }
        }

        public class ApiMessageResponse
        {
            public string Message { get; }

            public ApiMessageResponse(string message)
            {
                Message = message;
            }
        }

        public class ApiBadRequestResponse
        {
            public IEnumerable<string> Errors { get; }

            public ApiBadRequestResponse(ModelStateDictionary modelState)
            {
                if (modelState == null || modelState.IsValid)
                {
                    throw new ArgumentException("ModelState must have errors", nameof(modelState));
                }

                Errors = modelState
                    .SelectMany(x => x.Value.Errors)
                    .Select(x => x.ErrorMessage);
            }
        }

        public class CourtLocation
        {
            public string Code { get; set; }
            public string Name { get; set; }
        }

        public class Language
        {
            public string Code { get; set; }
            public string Name { get; set; }
        }

        public class Status
        {
            public int Code { get; set; }
            public string Name { get; set; }
        }

        public class Country
        {
            public string Code { get; set; }
            public string Name { get; set; }
        }

        public class Province
        {
            public string Code { get; set; }
            public string CountryCode { get; set; }
            public string Name { get; set; }
        }

        public class Statute
        {
            public decimal Code { get; set; }
            public string Name { get; set; }
        }

        public class PoliceLocation
        {
            public string Code { get; set; }
            public string Name { get; set; }
        }

        public class LookupsAll
        {
            public List<CourtLocation> CourtLocations { get; set; } = new List<CourtLocation>();
            public List<Language> Languages { get; set; } = new List<Language>();
            public List<Status> Statuses { get; set; } = new List<Status>();
            public List<Country> Countries { get; set; } = new List<Country>();
            public List<Province> Provinces { get; set; } = new List<Province>();
            public List<Statute> Statutes { get; set; } = new List<Statute>();
            public List<PoliceLocation> PoliceLocations { get; set; } = new List<PoliceLocation>();
        }

        public class AddressAutocompleteFindResponse
        {
            public string Id { get; set; } // The Id to use as the SearchTerm with the Find method.
            public string Text { get; set; } // The found item.
            public string Highlight { get; set; } // A series of number pairs that indicates which characters in the Text property have matched the SearchTerm.
            public string Cursor { get; set; } // A zero-based position in the Text response indicating the suggested position of the cursor if this item is selected. A -1 response indicates no suggestion is available.
            public string Description { get; set; } // Descriptive information about the found item, typically if it's a container.
            public string Next { get; set; } // The next step of the search process. (Find, Retrieve)
        }

        public class AddressAutocompleteRetrieveResponse
        {
            public string Id { get; set; }
            public string DomesticId { get; set; }
            public string Language { get; set; }
            public string LanguageAlternatives { get; set; }
            public string Department { get; set; }
            public string Company { get; set; }
            public string SubBuilding { get; set; }
            public string BuildingNumber { get; set; }
            public string BuildingName { get; set; }
            public string SecondaryStreet { get; set; }
            public string Street { get; set; }
            public string Block { get; set; }
            public string Neighbourhood { get; set; }
            public string District { get; set; }
            public string City { get; set; }
            public string Line1 { get; set; }
            public string Line2 { get; set; }
            public string Line3 { get; set; }
            public string Line4 { get; set; }
            public string Line5 { get; set; }
            public string AdminAreaName { get; set; }
            public string AdminAreaCode { get; set; }
            public string Province { get; set; }
            public string ProvinceName { get; set; }
            public string ProvinceCode { get; set; }
            public string PostalCode { get; set; }
            public string CountryName { get; set; }
            public string CountryIso2 { get; set; }
            public string CountryIso3 { get; set; }
            public int CountryIsoNumber { get; set; }
            public string SortingNumber1 { get; set; }
            public string SortingNumber2 { get; set; }
            public string Barcode { get; set; }
            public string PoBoxNumber { get; set; }
            public string Label { get; set; }
            public string DataLevel { get; set; }
        }

        public class Dispute
        {
            [Key]
            [Required]
            public int Id { get; set; }
            public string ViolationTicketNumber { get; set; }
            public string DisputantLastName { get; set; }
            public string DisputantFirstName { get; set; }
            public string DisputantMailingAddress { get; set; }
            public string DisputantMailingCity { get; set; }
            public string DisputantMailingProvince { get; set; }
            public string DisputantMailingPostalCode { get; set; }
            public string DisputantBirthDate { get; set; }
            public string DisputantEmailAddress { get; set; }
            public string DriverLicense { get; set; }
            public string DriverLicenseProvince { get; set; }
            public string PhoneNumber { get; set; }
            public bool LawyerPresent { get; set; }
            public bool InterpreterRequired { get; set; }
            public bool WitnessPresent { get; set; }
            public int? NumberOfWitnesses { get; set; }
            public string InterpreterLanguage { get; set; }
            public bool RequestReduction { get; set; }
            public bool RequestMoreTime { get; set; }
            public string ReductionReason { get; set; }
            public string MoreTimeReason { get; set; }
            public string ConfirmationNumber { get; set; }

            public ICollection<OffenceDisputeDetail> OffenceDisputeDetails { get; set; }
        }

        public class OffenceDispute
        {
            public string ViolationTicketNumber { get; set; }
            public int OffenceNumber { get; set; }
            public string ViolationTime { get; set; }
            public string ViolationDate { get; set; }
            public Additional Additional { get; set; }
            public OffenceDisputeDetail OffenceDisputeDetail { get; set; }
        }

        public class OffenceDisputeDetail2
        {
            [Key]
            public int Id { get; set; }
            public int OffenceNumber { get; set; }
            public bool RequestReduction { get; set; }
            public bool? ReductionAppearInCourt { get; set; }
            public bool RequestMoreTime { get; set; }
            public string ReductionReason { get; set; }
            public string MoreTimeReason { get; set; }
            public DisputeStatus Status { get; set; }
            public string OffenceAgreementStatus { get; set; }

            //[ForeignKey("Dispute")]
            public int DisputeId { get; set; }
            public Dispute Dispute { get; set; }
        }

        public class OffenceDisputeDetail
        {
            public string OffenceAgreementStatus { get; set; }
            public bool RequestReduction { get; set; }
            public bool RequestMoreTime { get; set; }
            public bool? ReductionAppearInCourt { get; set; }
            public string ReductionReason { get; set; }
            public string MoreTimeReason { get; set; }
            public DisputeStatus Status { get; set; }
        }


        public class CreateOffenceDisputeResponse
        {
            public int Id { get; set; }
        }

        public class CreateDisputeCommand : TicketDispute, IRequest<CreateDisputeResponse>
        {

        }

        public class CreateDisputeResponse
        {
            public int Id { get; set; }
        }

        public class CreateOffenceDisputeCommand : OffenceDispute, IRequest<CreateOffenceDisputeResponse>
        {
        }

        public class ShellTicketImageCommand : ShellTicketImage, IRequest<ShellTicketImageResponse>
        {
        }

        public class ShellTicketImageResponse
        {
        }

        public class ShellTicketImage
        {
            public string ViolationTicketNumber { get; set; }
            public string ViolationTime { get; set; }
            public IFormFile Image { get; set; }
        }

        public class CreateShellTicketCommand : ShellTicket, IRequest<CreateShellTicketResponse>
        {
        }
        public class CreateShellTicketResponse
        {
            public int Id { get; set; }
        }

        public class ShellTicket
        {
            public string ViolationTicketNumber { get; set; }
            public string ViolationTime { get; set; }
            public string ViolationDate { get; set; }
            public string LastName { get; set; }
            public string GivenNames { get; set; }
            public string DriverLicenseNumber { get; set; }
            public string Birthdate { get; set; } //2012-09-18
            public string Gender { get; set; } //M,F,O
            public string CourtHearingLocation { get; set; }//code
            public string DetachmentLocation { get; set; } //code
            public decimal? Count1Charge { get; set; } // charge code: 19023
            public decimal? Count1FineAmount { get; set; } //140.90
            public decimal? Count2Charge { get; set; } // charge code: 19023
            public decimal? Count2FineAmount { get; set; } //140.90
            public decimal? Count3Charge { get; set; }// charge code: 19023
            public decimal? Count3FineAmount { get; set; } //140.90
            public string Photo { get; set; } //base64 encoded image data.
            public string Address { get; set; }
            public string City { get; set; }
            public string Province { get; set; }
            public string PostalCode { get; set; }
            public string DriverLicenseProvince { get; set; }
        }

        public class TicketPaymentConfirmCommand : IRequest<TicketPaymentConfirmResponse>
        {
            [FromQuery(Name = "id")]
            [Required]
            public string Guid { get; set; }

            [FromQuery(Name = "status")]
            [Required]
            public string Status { get; set; }

            [FromQuery(Name = "amount")]
            public string Amount { get; set; }

            [FromQuery(Name = "confNo")]
            public string ConfirmationNumber { get; set; }

            [FromQuery(Name = "transId")]
            public string TransactionId { get; set; }

        }

        public class TicketPaymentConfirmResponse
        {
            public string TicketNumber { get; set; }
            public string Time { get; set; }
        }

        public class TicketPaymentCommand : IRequest<TicketPaymentResponse>
        {
            [FromQuery(Name = "ticketNumber")]
            [Required]
            [RegularExpression("^[A-Z]{2}[0-9]{6,}$", ErrorMessage = "ticketNumber must start with two upper case letters and 6 or more numbers")]
            public string TicketNumber { get; set; }

            [FromQuery(Name = "time")]
            [Required]
            [RegularExpression("^(2[0-3]|[01]?[0-9]):([0-5]?[0-9])$", ErrorMessage = "time must be properly formatted 24 hour clock")]
            public string Time { get; set; }

            [FromQuery(Name = "counts")]
            [Required]
            [RegularExpression("^[1-3]+(,[1-3]+)*$", ErrorMessage = "counts must be properly formatted, user , as seperatoer")]
            public string Counts { get; set; }

            [FromQuery(Name = "amount")]
            [Required]
            [RegularExpression(@"^\d*\.?\d*$", ErrorMessage = "amount needs to be a valid decimal")]
            public string Amount { get; set; }
        }

        public class TicketPaymentResponse : RedirectPay
        {
        }

        public class RedirectPay
        {
            public string ViolationTicketNumber { get; set; }
            public string ViolationTime { get; set; }
            public string Counts { get; set; }
            public string CallbackUrl { get; set; }
            public string RedirectUrl { get; set; }
        }

    }



}
