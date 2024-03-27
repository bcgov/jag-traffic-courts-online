using TrafficCourts.Common.Features.EmailVerificationToken;
using TrafficCourts.Messaging.Models;
using TrafficCourts.Messaging.MessageContracts;
using TrafficCourts.Workflow.Service.Configuration;

namespace TrafficCourts.Workflow.Service.Services.EmailTemplates;

public class VerificationEmailTemplate : EmailTemplate<SendEmailVerificationEmail>, IVerificationEmailTemplate
{
    private readonly EmailConfiguration _emailConfiguration;
    private readonly IDisputeEmailVerificationTokenEncoder _encoder;
    private readonly ILogger<VerificationEmailTemplate> _logger;

    public VerificationEmailTemplate(EmailConfiguration emailConfiguration, IDisputeEmailVerificationTokenEncoder encoder, ILogger<VerificationEmailTemplate> logger) : base()
    {
        _emailConfiguration = emailConfiguration ?? throw new ArgumentNullException(nameof(emailConfiguration));
        _encoder = encoder ?? throw new ArgumentNullException(nameof(encoder));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public override EmailMessage Create(SendEmailVerificationEmail data)
    {
        string verificationUrl = CreateEmailVerificationUrl(data);

        EmailMessage sendEmail = new()
        {
            From = Sender,
            To = data.EmailAddress,
            Subject = $"Verify your email for traffic violation ticket {data.TicketNumber}",
            TextContent = $@"
In order to confirm submission of your intent to dispute traffic violation ticket {data.TicketNumber} you must click on the following link. If you do not click on the link within 7 business days your dispute may not be registered.

{verificationUrl}

If you need more help, contact the {ViolationTicketCentreContact}."
        };

        return sendEmail;
    }

    private string CreateEmailVerificationUrl(SendEmailVerificationEmail data)
    {
        // https://tickets.gov.bc.ca/email/verify/{token}

        var format = _emailConfiguration.EmailVerificationUrl;
        if (string.IsNullOrEmpty(format))
        {
            var property = EmailConfiguration.Section + "__" + nameof(EmailConfiguration.EmailVerificationUrl);
            _logger.LogError("Cannot create email validation link. Configuration {Property} is null or empty", property);
            throw new InvalidOperationException($"Cannot create email validation link. Configuration {property} is null or empty.");
        }

        var token = _encoder.Encode(new DisputeEmailVerificationToken { NoticeOfDisputeGuid = data.NoticeOfDisputeGuid, Token = data.Token });
        string uri = format + "/" + token;

        return uri;
    }
}