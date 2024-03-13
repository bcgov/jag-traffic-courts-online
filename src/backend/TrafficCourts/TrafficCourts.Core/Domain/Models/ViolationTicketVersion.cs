namespace TrafficCourts.Domain.Models;

public enum ViolationTicketVersion
{
    /// <summary>
    /// Violation Ticket that was originally used 2022-04
    /// </summary>
    VT1,

    /// <summary>
    /// Violation Ticket was was introduced in 2023-09. This new Violation Ticket form contains most of the same
    /// fields as the VT1, though there are a few new fields and some have been removed.
    /// </summary>
    VT2
}
