namespace Gov.TicketWorker.Features.Emails
{
    /// <summary>
    /// Allows any valid email address
    /// </summary>
    public class NotFilteredEmailFilter : EmailFilter
    {
        protected override bool IsAllowedCore(string email) => true;
    }
}
