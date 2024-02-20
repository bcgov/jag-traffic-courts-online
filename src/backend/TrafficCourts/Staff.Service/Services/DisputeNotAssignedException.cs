namespace TrafficCourts.Staff.Service.Services
{
    internal class DisputeNotAssignedException : Exception
    {
        public DisputeNotAssignedException(string ticketNumber) : base($"Ticket {ticketNumber} is not assigned to a JJ")
        {
        }
    }
}