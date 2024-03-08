namespace TrafficCourts.Citizen.Service.Services.Tickets.Search;

[Serializable]
public class InvalidTicketVersionException : Exception
{
    public InvalidTicketVersionException(DateTime violationDate) : base($"Invalid ticket found with violation date: {violationDate}. The version of the Ticket is not VT2 since its violation date is before April 9, 2024")
    {
        ViolationDate = violationDate;
        ErrorId = Guid.NewGuid();
    }

    public DateTime ViolationDate { get; init; }

    public Guid ErrorId { get; }
}

