using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrafficCourts.Messaging.MessageContracts;

/// <summary>
/// Interface message contract for validating an email. A user would click a link in an email they received to confirm the email address is valid.
/// </summary>
[EndpointConvention("email-received-validation")]
public class EmailReceivedValidation : IMessage
{
    public EmailReceivedValidation(Guid emailValidationToken)
    {
        EmailValidationToken = emailValidationToken;
    }

    public Guid EmailValidationToken { get; set; }

}
