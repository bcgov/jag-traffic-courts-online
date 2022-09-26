using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrafficCourts.Messaging.MessageContracts;

/// <summary>
/// Interface message contract for sending verifying emails. An email would go out to an end user with a verification link so they can confirm the address is correct.
/// </summary>
[EndpointConvention("email-verification-send")]
public class EmailVerificationSend : IMessage
{
    public EmailVerificationSend(Guid emailVerificationToken)
    {
        EmailVerificationToken = emailVerificationToken;
    }

    public Guid EmailVerificationToken { get; set; }
}
