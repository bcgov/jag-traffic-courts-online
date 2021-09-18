using System;
using System.Collections.Generic;
using System.Linq;

namespace Gov.TicketWorker.Features.Emails
{
    /// <summary>
    /// Filters the email to a know list of valid emails. 
    /// Used in non production environments to avoid sending
    /// emails to real users.
    /// </summary>
    public class DefaultEmailFilter : EmailFilter
    {
        private readonly HashSet<string> _allowed;

        public DefaultEmailFilter(IEnumerable<string> allowed)
        {
            if (allowed == null) throw new ArgumentNullException(nameof(allowed));

            // remove any null, empty or whitespace enties from the collection
            // and trim any whitespace from both ends
            var cleaned = allowed
                .Where(_ => !string.IsNullOrWhiteSpace(_))
                .Select(_ => _.Trim());

            _allowed = new HashSet<string>(cleaned, StringComparer.OrdinalIgnoreCase);
        }

        protected override bool IsAllowedCore(string email) => _allowed.Contains(email);
    }
}
