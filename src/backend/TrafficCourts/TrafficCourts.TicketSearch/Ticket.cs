namespace TrafficCourts.TicketSearch;

public record Ticket
{
    /// <summary>
    /// The ticket number.
    /// </summary>
    public string Number { get; set; }

    public Ticket(string number)
    {
        Number = number;
    }

    public DateTime? Issued { get; set; }

    public string Surname { get; set; } = string.Empty;
    public string FirstGivenName { get; set; } = string.Empty;
    public string SecondGivenName { get; set; } = string.Empty;

    public List<Count> Counts { get; set; } = [];
}


public record Count
{
    public const string MotorVehicleAct = "MVA";
    public const string MotorVehicleActRegulations = "MVAR";
    public const string MotorVehicleRegulations = "MVR";

    /// <summary>
    /// The count number (1-3).
    /// </summary>
    public int Number { get; set; }

    /// <summary>
    /// The ticketed amount
    /// </summary>
    public decimal TicketedAmount { get; set; }

    public decimal AmountDue { get; set; }

    public decimal? DiscountAmount { get; set; }

    public bool IsAct => Act == MotorVehicleAct;
    public bool IsRegulation => Act == MotorVehicleActRegulations || Act == MotorVehicleRegulations;

    public string Act { get; set; } = string.Empty;
    public string Section { get; set; } = string.Empty;
    public string Subsection { get; set; } = string.Empty;
    public string Paragraph { get; set; } = string.Empty;
    public string Subparagraph { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
    public string FormNumber { get; set; } = string.Empty;
}
