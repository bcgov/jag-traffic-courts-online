using Newtonsoft.Json;

namespace TrafficCourts.Staff.Service.Models.DigitalCaseFiles.Print;

public class DigitalCaseFile
{
    /// <summary>
    /// Indicates that this digial case file is a hearing as opposed
    /// a written reasons case.
    /// </summary>
    [JsonProperty("hearing")]
    public bool Hearing { get; set; }

    [JsonProperty("ticket")]
    public TicketSummary Ticket { get; set; } = new TicketSummary();

    [JsonProperty("contact")]
    public ContactInformation Contact { get; set; } = new ContactInformation();

    [JsonProperty("courtOptions")]
    public CourtOptions CourtOptions { get; set; } = new CourtOptions();

    [JsonProperty("appearance")]
    public Appearance Appearance { get; set; } = new Appearance();

    [JsonProperty("pastAppearances")]
    public List<Appearance> AppearanceHistory { get; set; } = new List<Appearance>();

    [JsonProperty("writtenReasons")]
    public WrittenReasons WrittenReasons { get; set; } = new WrittenReasons();

    [JsonProperty("counts")]
    public List<OffenseCount> Counts { get; set; } = new List<OffenseCount>();

    [JsonProperty("isElectronicTicket")]
    public string IsElectronicTicket { get; set; } = string.Empty;

    [JsonProperty("hasNoticeOfHearing")]
    public string HasNoticeOfHearing { get; set; } = string.Empty;

    [JsonProperty("documents")]
    public List<Document> Documents { get; set; } = new List<Document>();

    [JsonProperty("history")]
    public List<FileHistoryEvent> History { get; set; } = new List<FileHistoryEvent>();

    [JsonProperty("remarks")]
    public List<FileRemark> FileRemarks { get; set; } = new List<FileRemark>();
}

public class TicketSummary
{
    /// <summary>
    /// The ticket number
    /// </summary>
    [JsonProperty("number")]
    public string Number { get; set; } = string.Empty;

    [JsonProperty("surname")]
    public string Surname { get; set; } = string.Empty;

    [JsonProperty("givenNames")]
    public string GivenNames { get; set; } = string.Empty;

    [JsonProperty("address")]
    public string Address { get; set; } = string.Empty;

    [JsonProperty("issuedDate")]
    public DateTime IssuedDate { get; set; }

    [JsonProperty("submittedDate")]
    public DateTime SubmittedDate { get; set; }

    [JsonProperty("courtHouse")]
    public string CourtHouse { get; set; } = string.Empty;

    [JsonProperty("icbcReceivedDate")]
    public DateTime IcbcReceivedDate { get; set; }

    [JsonProperty("policeDetachment")]
    public string PoliceDetachment { get; set; } = string.Empty;

    [JsonProperty("offenceLocation")]
    public string OffenceLocation { get; set; } = string.Empty;

    [JsonProperty("courtAgenyId")]
    public string CourtAgenyId { get; set; } = string.Empty;
}

public class ContactInformation
{
    [JsonProperty("surname")]
    public string Surname { get; set; } = string.Empty;
    [JsonProperty("givenNames")]
    public string GivenNames { get; set; } = string.Empty;
    [JsonProperty("address")]
    public string Address { get; set; } = string.Empty;
    [JsonProperty("driversLicence")]
    public DriversLicence DriversLicence { get; set; } = new DriversLicence();
    [JsonProperty("email")]
    public string Email { get; set; } = string.Empty;
}

public class DriversLicence
{
    [JsonProperty("province")]
    public string Province { get; set; } = string.Empty;

    [JsonProperty("number")]
    public string Number { get; set; } = string.Empty;
}


public class Appearance
{
    /// <summary>
    /// The appearance date and time
    /// </summary>
    [JsonProperty("date")]
    public DateTime Date { get; set; }

    [JsonProperty("reason")]
    public string Reason { get; set; } = string.Empty;

    /// <summary>
    /// The appearance room
    /// </summary>
    [JsonProperty("room")]
    public string Room { get; set; } = string.Empty;

    /// <summary>
    /// </summary>
    [JsonProperty("app")]
    public string App { get; set; } = string.Empty;

    /// <summary>
    /// No appearance date
    /// </summary>
    [JsonProperty("noApp")]
    public DateTime? NoApp { get; set; }

    /// <summary>
    /// Y or N
    /// </summary>
    [JsonProperty("clerk")]
    public string Clerk { get; set; } = string.Empty;

    /// <summary>
    /// </summary>
    [JsonProperty("defenseCouncil")]
    public string DefenseCouncil { get; set; } = string.Empty;

    [JsonProperty("defenseAtt")]
    public string DefenseAtt { get; set; } = string.Empty;

    [JsonProperty("crown")]
    public string Crown { get; set; } = string.Empty;

    [JsonProperty("seized")]
    public string Seized { get; set; } = string.Empty;

    [JsonProperty("judicialJustice")]
    public string JudicialJustice { get; set; } = string.Empty;

    [JsonProperty("comments")]
    public string Comments { get; set; } = string.Empty;
}

public class CourtOptions
{
    [JsonProperty("counsel")]
    public LegalCounsel LegalCounsel { get; set; } = new LegalCounsel();

    [JsonProperty("witnessCount")]
    public int WitnessCount { get; set; }
    
    [JsonProperty("interpreter")]
    public string InterpreterLanguage { get; set; } = string.Empty;

    [JsonProperty("attendanceType")]
    public string DisputantAttendanceType { get; set; } = string.Empty;
}

public class LegalCounsel
{
    [JsonProperty("name")]
    public string FullName { get; set; } = string.Empty;

    [JsonProperty("firm")]
    public string FirmName { get; set; } = string.Empty;
}

public class WrittenReasons
{
    [JsonProperty("fineReduction")]
    public string FineReduction { get; set; } = string.Empty;

    [JsonProperty("timeToPay")]
    public string TimeToPay { get; set; } = string.Empty;
}

public class OffenseCount
{
    [JsonProperty("")]
    public int Count { get; set; }

    [JsonProperty("plea")]
    public string Plea { get; set; } = string.Empty;

    [JsonProperty("description")]
    public string Description { get; set; } = string.Empty;

    [JsonProperty("due")]
    public DateTime Due { get; set; }

    [JsonProperty("fine")]
    public decimal Fine { get; set; }

    [JsonProperty("requestFineReduction")]
    public string RequestFineReduction { get; set; } = string.Empty;

    [JsonProperty("requestTimeToPay")]
    public string RequestTimeToPay { get; set; } = string.Empty;
}

public class Document
{
    [JsonProperty("filename")]
    public string FileName { get; set; } = string.Empty;
}

public class FileHistoryEvent
{
    /// <summary>
    /// The date and time of the file history event
    /// </summary>
    [JsonProperty("date")]
    public DateTime Date { get; set; }

    [JsonProperty("username")]
    public string Username { get; set; } = string.Empty;

    [JsonProperty("type")]
    public string Type { get; set; } = string.Empty;

    [JsonProperty("description")]
    public string Description { get; set; } = string.Empty;
}

public class FileRemark
{
    [JsonProperty("date")]
    public DateTime Date { get; set; }

    [JsonProperty("username")]
    public string Username { get; set; } = string.Empty;

    [JsonProperty("note")]
    public string Note { get; set; } = string.Empty;
}