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
