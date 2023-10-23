using Newtonsoft.Json;
using System.Security.Cryptography.Xml;

namespace TrafficCourts.Staff.Service.Models.DigitalCaseFiles.Print;

public class DigitalCaseFile
{
    /// <summary>
    /// Indicates that this digial case file is a hearing as opposed
    /// a written reasons case.
    /// </summary>
    [JsonProperty("hearing")]
    public bool Hearing { get; set; }
    public TicketSummary Ticket { get; set; } = new TicketSummary();

    public ContactInformation ContactInformation { get; set; } = new ContactInformation();

    public CourtOptions CourtOptions { get; set; } = new CourtOptions();

    public Appearance Appearance { get; set; } = new Appearance();

    public List<Appearance> AppearanceHistory { get; set; } = new List<Appearance>();

    public WrittenReasons WrittenReasons { get; set; } = new WrittenReasons();

    public List<OffenseCount> Counts { get; set; } = new List<OffenseCount>(); 

    public bool IsElectronicTicket { get; set; }
    public bool HasNoticeOfHearing { get; set; }

    public List<Document> Documents { get; set; } = new List<Document>();

    public List<FileHistoryEvent> History { get; set; } = new List<FileHistoryEvent>();
    public List<FileRemark> FileRemarks { get; set; } = new List<FileRemark>();
}

public class TicketSummary
{
    /// <summary>
    /// The ticket number
    /// </summary>
    public string Number { get; set; }

    public string Surname { get; set; }
    public string GivenNames { get; set; }

    public string Address { get; set; }

    public DateTime IssuedDate { get; set; }

    public DateTime SubmittedDate { get; set; }

    public string CourtHouse { get; set; }

    public DateTime IcbcReceivedDate { get; set; }

    public string PoliceDetachment { get; set; }

    public string OffenceLocation { get; set; }

    public string CourtAgenyId { get; set; }
}

public class ContactInformation
{
    public string Surname { get; set; }
    public string GivenNames { get; set; }
    public string Address { get; set; }
    public DriversLicence DriversLicence { get; set; } = new DriversLicence();
    public string Email { get; set; }
}

public class DriversLicence
{
    public string Province { get; set; }
    public string Number { get; set; }
}


public class Appearance
{
    /// <summary>
    /// The appearance date and time
    /// </summary>
    public DateTime DateTime { get; set; }

    /// <summary>
    /// The appearance room
    /// </summary>
    public string Room { get; set; }

    /// <summary>
    /// </summary>
    public string App { get; set; }

    /// <summary>
    /// </summary>
    public string NoApp { get; set; }

    /// <summary>
    /// </summary>
    public string ClerkRec { get; set; }

    /// <summary>
    /// </summary>
    public string DefenseCouncil { get; set; }
    public string DefenseAtt { get; set; }
    public bool Seized { get; set; }

    public string JudicialJustice { get; set; }

    public string Comments { get; set; }
}

public class CourtOptions
{
    public LegalCounsel LegalCounsel { get; set; }
    public int WitnessCount { get; set; }
    public string InterpreterLangurage { get; set; }
    public string DisputantAttendanceType { get; set; }
}

public class LegalCounsel
{
    public string FullName { get; set; }
    public string FirmName { get; set; }
}

public class WrittenReasons
{
    public string FineReductionReason { get; set; }
    public string TimeToPayReason { get; set; }
}

public class OffenseCount
{
    public int Count { get; set; }
    public string Plea { get; set; }
    public string Description { get; set; }
    public DateTime DueDate { get; set; }
    public decimal FineAmount { get; set; }
    public string Request { get; set; }
    public bool RequestFineReduction { get; set; }
    public bool RequestTimeToPay { get; set; }
}

public class Document
{
    public string FileName { get; set; }
}

public class FileHistoryEvent
{
    /// <summary>
    /// The date and time of the file history event
    /// </summary>
    public DateTime DateTime { get; set; }

    public string Username { get; set; }

    public string RecordType { get; set; }
    public string Description { get; set; }
}

public class FileRemark
{
    public DateTime DateTime { get; set; }
    public string Username { get; set; }

    public string Note { get; set; }
}