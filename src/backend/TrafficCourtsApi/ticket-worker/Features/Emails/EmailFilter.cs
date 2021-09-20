using System;

namespace Gov.TicketWorker.Features.Emails
{
    /// <summary>
    /// Base class that handles parameter validations of emails
    /// </summary>
    public abstract class EmailFilter : IEmailFilter
    {
        public bool IsAllowed(string email)
        {
            if (email == null) throw new ArgumentNullException(nameof(email));

            if (string.IsNullOrEmpty(email)) throw new ArgumentException(null, nameof(email));

            if (!Validation.IsValidEmail(email))
            {
                throw new FormatException("Email address is not a valid email address");
            }

            return IsAllowedCore(email);
        }

        /// <summary>
        /// Implemenation specific call to check if email the email is allowed. 
        /// All parameter validation and normalization will have occured before
        /// this call. Implementations can assume the email address is valid.
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        protected abstract bool IsAllowedCore(string email);
    }
}
