using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrafficCourts.Messaging.MessageContracts;

/// <summary>
/// Interface message contract for sending validation emails. An email would go out to an end user with a validation link so they can confirm the address is correct.
/// </summary>
[EndpointConvention("email-send-validation")]
public class EmailSendValidation : IMessage
{
    public EmailSendValidation(Guid emailValidationToken)
    {
        EmailValidationToken = emailValidationToken;
    }

    public Guid EmailValidationToken { get; set; }
}
