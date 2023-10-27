using Bogus;
using System;
using System.Linq;
using TrafficCourts.Staff.Service.Models.DigitalCaseFiles.Print;

namespace TrafficCourts.Staff.Service.Test.Services.PrintDigialCaseFile;

public class DigitalCaseFileFaker : Faker<DigitalCaseFile>
{
    private const string Locale = "en_CA";

    private static readonly Faker<DriversLicence> _driversLicenceFaker = new Faker<DriversLicence>(Locale)
        .StrictMode(true)
        .RuleFor(_ => _.Province, f => f.Address.State())
        .RuleFor(_ => _.Number, f => f.DriversLicenceNumber());

    private static readonly Faker<TicketSummary> _ticketSummaryFaker = new Faker<TicketSummary>(Locale)
        .StrictMode(true)
        .RuleFor(_ => _.Number, f => f.TicketNumber())
        .RuleFor(_ => _.Surname, f => f.Name.LastName())
        .RuleFor(_ => _.GivenNames, f => f.Name.FirstName())
        .RuleFor(_ => _.Address, f => f.Address.FullAddress())
        .RuleFor(_ => _.IssuedDate, f => DateTime.SpecifyKind(f.Date.Past().Date, DateTimeKind.Unspecified))
        .RuleFor(_ => _.SubmittedDate, f => DateTime.SpecifyKind(f.Date.Past().Date, DateTimeKind.Unspecified))
        .RuleFor(_ => _.IcbcReceivedDate, f => DateTime.SpecifyKind(f.Date.Past().Date, DateTimeKind.Unspecified))
        .RuleFor(_ => _.PoliceDetachment, f => f.Name.LastName())
        .RuleFor(_ => _.OffenceLocation, f => f.Address.StreetName() + " at " + f.Address.StreetName())
        .RuleFor(_ => _.CourtAgenyId, f => f.AgencyId())
        .RuleFor(_ => _.CourtHouse, f => f.Address.City())
        ;

    private static readonly Faker<ContactInformation> _contactInformationFaker = new Faker<ContactInformation>(Locale)
        .StrictMode(true)
        .RuleFor(_ => _.Surname, f => f.Name.LastName())
        .RuleFor(_ => _.GivenNames, f => f.Name.FirstName())
        .RuleFor(_ => _.Address, f => f.Address.StreetAddress(true))
        .RuleFor(_ => _.DriversLicence, f => _driversLicenceFaker.Generate())
        .RuleFor(_ => _.Email, (f, d) => f.Internet.Email(d.GivenNames, d.Surname))
        ;

    private static readonly Faker<LegalCounsel> _legalCounselFaker = new Faker<LegalCounsel>(Locale)
        .RuleFor(_ => _.FullName, f => f.Name.FullName())
        .RuleFor(_ => _.FirmName, f => f.LegalCounselFirmName())
        ;

    private static readonly Faker<CourtOptions> _courtOptionsFaker = new Faker<CourtOptions>(Locale)
        .StrictMode(true)
        .RuleFor(_ => _.LegalCounsel, f => _legalCounselFaker)
        .RuleFor(_ => _.WitnessCount, f => f.WitnessCount(5))
        .RuleFor(_ => _.InterpreterLanguage, f => f.InterpreterLanguage())
        .RuleFor(_ => _.DisputantAttendanceType, f => f.AttendanceType())
        //.RuleFor(_ => _.Address, f => f.Address.FullAddress())
        //.RuleFor(_ => _.DriversLicence, f => _driversLicenceFaker.Generate())
        //.RuleFor(_ => _.Email, (f, d) => f.Internet.Email(d.GivenNames, d.Surname))
        ;

    private static readonly Faker<Appearance> _futureAppearanceFaker = new Faker<Appearance>(Locale)
        .StrictMode(true)
        .RuleFor(_ => _.Date, f => f.Date.Soon(7))
        .RuleFor(_ => _.Room, f => f.Room())
        .RuleFor(_ => _.Reason, f => f.HearingReason())
        .RuleFor(_ => _.App, f => f.PresentAbsentOrNotRequired())
        .RuleFor(_ => _.NoApp, f => f.Date.Past())
        .RuleFor(_ => _.Clerk, f => f.YesOrNo())
        .RuleFor(_ => _.DefenseCouncil, f => f.Name.FullName())
        .RuleFor(_ => _.DefenseAtt, f => f.PresentOrAbsent())
        .RuleFor(_ => _.Crown, f => f.PresentOrAbsent())
        .RuleFor(_ => _.Seized, f => f.YesOrNo())
        .RuleFor(_ => _.Comments, f => f.Lorem.Lines(2))
        .RuleFor(_ => _.JudicialJustice, f => f.Name.FullName())
        ;

    private static readonly Faker<Appearance> _pastAppearanceFaker = new Faker<Appearance>(Locale)
        .StrictMode(true)
        .RuleFor(_ => _.Date, f => f.Date.Past())
        .RuleFor(_ => _.Room, f => f.Room())
        .RuleFor(_ => _.Reason, f => f.HistoricalHearingReason())
        .RuleFor(_ => _.App, f => f.PresentAbsentOrNotRequired())
        .RuleFor(_ => _.NoApp, f => f.Date.Past())
        .RuleFor(_ => _.Clerk, f => f.YesOrNo())
        .RuleFor(_ => _.DefenseCouncil, f => f.Name.FullName())
        .RuleFor(_ => _.DefenseAtt, f => f.PresentOrAbsent())
        .RuleFor(_ => _.Crown, f => f.PresentOrAbsent())
        .RuleFor(_ => _.Seized, f => f.YesOrNo())
        .RuleFor(_ => _.Comments, f => f.Lorem.Lines(2))
        .RuleFor(_ => _.JudicialJustice, f => f.Name.FullName())
        ;

    private static readonly Faker<WrittenReasons> _writtenReasonsFaker = new Faker<WrittenReasons>(Locale)
        .StrictMode(true)
        .RuleFor(_ => _.FineReduction, f => f.Lorem.Lines(2))
        .RuleFor(_ => _.TimeToPay, f => f.Lorem.Lines(2))
        ;

    private static readonly Faker<Document> _documentFaker = new Faker<Document>(Locale)
        .StrictMode(true)
        .RuleFor(_ => _.FileName, f => f.Random.String2(8).ToLower() + ".pdf")
        ;

    private static readonly Faker<OffenseCount> _offenseCountFaker = new Faker<OffenseCount>(Locale)
        .StrictMode(true)
        .RuleFor(_ => _.Count, f => f.Random.Int(1, 3))
        .RuleFor(_ => _.Description, f => f.OffenseDescription())
        .RuleFor(_ => _.Due, f =>f.Date.Past().Date)
        .RuleFor(_ => _.Fine, f => f.FineAmount())
        .RuleFor(_ => _.Plea, f => f.PickRandom(new string[] { "Guilty", "Not Guilty" }))
        //.RuleFor(_ => _.Request, f => f.PickRandom(new string[] { "?? dont know ??", "?? what this is ??" }))
        .RuleFor(_ => _.RequestFineReduction, f => f.YesOrNo(word: true))
        .RuleFor(_ => _.RequestTimeToPay, f => f.YesOrNo(word: true))
        ;

    private static readonly Faker<FileHistoryEvent> _fileHistoryEventFaker = new Faker<FileHistoryEvent>(Locale)
        .RuleFor(_ => _.Date, f => f.Date.Past())
        .RuleFor(_ => _.Username, f => $"{f.Name.FirstName()[0]}{f.Name.LastName()}".ToUpper())
        .RuleFor(_ => _.Type, f => f.Lorem.Text())
        .RuleFor(_ => _.Description, f => f.Lorem.Text())
        ;

    private static readonly Faker<FileRemark> _fileRemarkFaker = new Faker<FileRemark>(Locale)
        .RuleFor(_ => _.Date, f => f.Date.Past())
        .RuleFor(_ => _.Username, f => $"{f.Name.FirstName()[0]}{f.Name.LastName()}".ToUpper())
        .RuleFor(_ => _.Note, f => f.Lorem.Lines())
    ;

    public DigitalCaseFileFaker() : base(Locale)
    {
        RuleFor(_ => _.Ticket, f => _ticketSummaryFaker);
        RuleFor(_ => _.Contact, f => _contactInformationFaker);
        RuleFor(_ => _.CourtOptions, f => _courtOptionsFaker);
        RuleFor(_ => _.Appearance, f => _futureAppearanceFaker);
        RuleFor(_ => _.AppearanceHistory, f => _pastAppearanceFaker.Generate(3).OrderByDescending(_ => _.Date).ToList());
        RuleFor(_ => _.WrittenReasons, f => _writtenReasonsFaker);
        RuleFor(_ => _.Counts, f => _offenseCountFaker.Generate(3));
        RuleFor(_ => _.IsElectronicTicket, f => f.YesOrNo());
        RuleFor(_ => _.HasNoticeOfHearing, f => f.YesOrNo());
        RuleFor(_ => _.Documents, f => _documentFaker.Generate(3));
        RuleFor(_ => _.History, f => _fileHistoryEventFaker.Generate(3).OrderByDescending(_ => _.Date).ToList());
        RuleFor(_ => _.FileRemarks, f => _fileRemarkFaker.Generate(3).OrderByDescending(_ => _.Date).ToList());
    }
}

