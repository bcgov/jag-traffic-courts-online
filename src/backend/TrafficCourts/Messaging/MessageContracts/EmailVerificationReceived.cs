using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrafficCourts.Messaging.MessageContracts;

/// <summary>
/// Interface message contract for verifying an email. A user would click a link in an email they received to confirm the email address is valid.
/// </summary>
public class EmailVerificationReceived
{
    public EmailVerificationReceived(Guid emailVerificationToken)
    {
        EmailVerificationToken = emailVerificationToken;
    }

    public Guid EmailVerificationToken { get; set; }

}
