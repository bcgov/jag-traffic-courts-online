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
    public string IsElectronicTicket { get; set; }

    [JsonProperty("hasNoticeOfHearing")]
    public string HasNoticeOfHearing { get; set; }

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
    public string Number { get; set; }

    [JsonProperty("surname")]
    public string Surname { get; set; }

    [JsonProperty("givenNames")]
    public string GivenNames { get; set; }

    [JsonProperty("address")]
    public string Address { get; set; }

    [JsonProperty("issuedDate")]
    public DateTime IssuedDate { get; set; }

    [JsonProperty("submittedDate")]
    public DateTime SubmittedDate { get; set; }

    [JsonProperty("courtHouse")]
    public string CourtHouse { get; set; }

    [JsonProperty("icbcReceivedDate")]
    public DateTime IcbcReceivedDate { get; set; }

    [JsonProperty("policeDetachment")]
    public string PoliceDetachment { get; set; }

    [JsonProperty("offenceLocation")]
    public string OffenceLocation { get; set; }

    [JsonProperty("courtAgenyId")]
    public string CourtAgenyId { get; set; }
}

public class ContactInformation
{
    [JsonProperty("surname")]
    public string Surname { get; set; }
    [JsonProperty("givenNames")]
    public string GivenNames { get; set; }
    [JsonProperty("address")]
    public string Address { get; set; }
    [JsonProperty("driversLicence")]
    public DriversLicence DriversLicence { get; set; } = new DriversLicence();
    [JsonProperty("email")]
    public string Email { get; set; }
}

public class DriversLicence
{
    [JsonProperty("province")]
    public string Province { get; set; }

    [JsonProperty("number")]
    public string Number { get; set; }
}


public class Appearance
{
    /// <summary>
    /// The appearance date and time
    /// </summary>
    [JsonProperty("date")]
    public DateTime Date { get; set; }

    [JsonProperty("reason")]
    public string Reason { get; set; }

    /// <summary>
    /// The appearance room
    /// </summary>
    [JsonProperty("room")]
    public string Room { get; set; }

    /// <summary>
    /// </summary>
    [JsonProperty("app")]
    public string App { get; set; }

    /// <summary>
    /// No appearance date
    /// </summary>
    [JsonProperty("noApp")]
    public DateTime? NoApp { get; set; }

    /// <summary>
    /// Y or N
    /// </summary>
    [JsonProperty("clerk")]
    public string Clerk { get; set; }

    /// <summary>
    /// </summary>
    [JsonProperty("defenseCouncil")]
    public string DefenseCouncil { get; set; }

    [JsonProperty("defenseAtt")]
    public string DefenseAtt { get; set; }

    [JsonProperty("crown")]
    public string Crown { get; set; }

    [JsonProperty("seized")]
    public string Seized { get; set; }

    [JsonProperty("judicialJustice")]
    public string JudicialJustice { get; set; }

    [JsonProperty("comments")]
    public string Comments { get; set; }
}

public class CourtOptions
{
    [JsonProperty("counsel")]
    public LegalCounsel LegalCounsel { get; set; }

    [JsonProperty("witnessCount")]
    public int WitnessCount { get; set; }
    
    [JsonProperty("interpreter")]
    public string InterpreterLanguage { get; set; }

    [JsonProperty("attendanceType")]
    public string DisputantAttendanceType { get; set; }
}

public class LegalCounsel
{
    [JsonProperty("name")]
    public string FullName { get; set; }

    [JsonProperty("firm")]
    public string FirmName { get; set; }
}

public class WrittenReasons
{
    [JsonProperty("fineReduction")]
    public string FineReduction { get; set; }

    [JsonProperty("timeToPay")]
    public string TimeToPay { get; set; }
}

public class OffenseCount
{
    [JsonProperty("")]
    public int Count { get; set; }

    [JsonProperty("plea")]
    public string Plea { get; set; }

    [JsonProperty("description")]
    public string Description { get; set; }

    [JsonProperty("due")]
    public DateTime Due { get; set; }

    [JsonProperty("fine")]
    public decimal Fine { get; set; }

    //[JsonProperty("request")]
    //public string Request { get; set; }

    [JsonProperty("requestFineReduction")]
    public string RequestFineReduction { get; set; }

    [JsonProperty("requestTimeToPay")]
    public string RequestTimeToPay { get; set; }
}

public class Document
{
    [JsonProperty("filename")]
    public string FileName { get; set; }
}

public class FileHistoryEvent
{
    /// <summary>
    /// The date and time of the file history event
    /// </summary>
    [JsonProperty("date")]
    public DateTime Date { get; set; }

    [JsonProperty("username")]
    public string Username { get; set; }

    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("description")]
    public string Description { get; set; }
}

public class FileRemark
{
    [JsonProperty("date")]
    public DateTime Date { get; set; }

    [JsonProperty("username")]
    public string Username { get; set; }

    [JsonProperty("note")]
    public string Note { get; set; }
}