using System;

namespace Gov.TicketWorker.Features.Emails
{
    public interface IEmailFilter
    {
        /// <summary>
        /// Determines if the system is allowed to send to the target email address
        /// </summary>
        /// <param name="email">The email address to check</param>
        /// <returns><true/> if sending email is allowed, otherwise <false/></returns>
        /// <exception cref="ArgumentNullException"><paramref name="email"/> is null</exception>
        /// <exception cref="ArgumentException"><paramref name="email"/> is empty string or white space</exception>
        /// <exception cref="FormatException"><paramref name="email"/> is not a valid email address</exception>
        bool IsAllowed(string email);
    }
}
