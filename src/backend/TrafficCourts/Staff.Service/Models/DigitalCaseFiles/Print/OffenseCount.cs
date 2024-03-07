using Newtonsoft.Json;

namespace TrafficCourts.Staff.Service.Models.DigitalCaseFiles.Print;

public class OffenseCount
{
    [JsonProperty("count")]
    public string Count { get; set; } = string.Empty;

    /// <summary>
    /// The offense date. This will be the same as the disute issued date.
    /// Duplicating the field makes it easier for binding to the screen.
    /// </summary>
    [JsonProperty("offense")]
    public FormattedDateOnly Offense { get; set; } = FormattedDateOnly.Empty;

    [JsonProperty("plea")]
    public string Plea { get; set; } = string.Empty;

    [JsonProperty("description")]
    public string Description { get; set; } = string.Empty;

    [JsonProperty("due")]
    public FormattedDateOnly Due { get; set; } = FormattedDateOnly.Empty;

    [JsonProperty("fine")]
    public decimal? Fine { get; set; }

    [JsonProperty("appearInCourt")]
    public string AppearInCourt { get; set; } = string.Empty;

    [JsonProperty("requestFineReduction")]
    public string RequestFineReduction { get; set; } = string.Empty;

    [JsonProperty("requestTimeToPay")]
    public string RequestTimeToPay { get; set; } = string.Empty;

    [JsonProperty("finding")]
    public string Finding { get; set; } = string.Empty;

    [JsonProperty("lesserDescription")]
    public string LesserDescription { get; set; } = string.Empty;

    [JsonProperty("reviseFine")]
    public bool ReviseFine { get; set; }

    [JsonProperty("lesserOrGreaterAmt")]
    public decimal? LesserOrGreaterAmount { get; set; }

    [JsonProperty("roundLesserOrGreaterAmt")]
    public decimal? RoundLesserOrGreaterAmount { get; set; }

    [JsonProperty("includesSurcharge")]
    public string IncludesSurcharge { get; set; } = string.Empty;

    [JsonProperty("totalFineAmount")]
    public decimal? TotalFineAmount { get; set; }

    [JsonProperty("isDueDateRevised")]
    public bool IsDueDateRevised { get; set; }

    [JsonProperty("revisedDue")]
    public FormattedDateOnly RevisedDue { get; set; } = FormattedDateOnly.Empty;

    [JsonProperty("ssProbationDuration")]
    public string SsProbationDuration { get; set; } = string.Empty;

    [JsonProperty("ssProbationConditions")]
    public string SsProbationConditions { get; set; } = string.Empty;

    [JsonProperty("jailDuration")]
    public string JailDuration { get; set; } = string.Empty;

    [JsonProperty("jailIntermittent")]
    public string JailIntermittent { get; set; } = string.Empty;

    [JsonProperty("probationDuration")]
    public string ProbationDuration { get; set; } = string.Empty;

    [JsonProperty("probationConditions")]
    public string ProbationConditions { get; set; } = string.Empty;

    [JsonProperty("drivingProhibitionDuration")]
    public string DrivingProhibitionDuration { get; set; } = string.Empty;

    [JsonProperty("drivingProhibitionMVA")]
    public string DrivingProhibitionMVA { get; set; } = string.Empty;

    [JsonProperty("dismissed")]
    public string Dismissed { get; set; } = string.Empty;

    [JsonProperty("wantOfProsecution")]
    public string WantOfProsecution { get; set; } = string.Empty;

    [JsonProperty("withdrawn")]
    public string Withdrawn { get; set; } = string.Empty;

    [JsonProperty("abatement")]
    public string Abatement { get; set; } = string.Empty;

    [JsonProperty("stayOfProceedingsBy")]
    public string StayOfProceedingsBy { get; set; } = string.Empty;

    [JsonProperty("other")]
    public string Other { get; set; } = string.Empty;

    [JsonProperty("remarks")]
    public string Comments { get; set; } = string.Empty;

    [JsonProperty("finalDue")]
    public FormattedDateOnly FinalDue { get; set; } = FormattedDateOnly.Empty;

    [JsonProperty("surcharge")]
    public decimal? Surcharge { get; set; }
}
