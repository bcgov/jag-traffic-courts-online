namespace TrafficCourts.Citizen.Service.Services.Tickets.Search;

[Serializable]
public class DisputeSearchFailedException : Exception
{
    public DisputeSearchFailedException(string ticketNumber, Exception? innerException = null) 
        : base($"Dispute search failed. Dispute search response returned an error for: {ticketNumber}.", innerException)
    {
        TicketNumber = ticketNumber;
        ErrorId = Guid.NewGuid();
    }

    public string TicketNumber { get; set; }

    public Guid ErrorId { get; }
}

