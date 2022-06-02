using Microsoft.Extensions.Options;
using TrafficCourts.Messaging.MessageContracts;
using TrafficCourts.Workflow.Service.Configuration;
using MimeKit;
using MimeKit.Text;

namespace TrafficCourts.Workflow.Service.Services
{
    public class EmailSenderService : IEmailSenderService
    {
        private readonly ILogger<EmailSenderService> _logger;
        private readonly EmailConfiguration _emailConfiguration;
        private readonly ISmtpClientFactory _smptClientFactory;

        public EmailSenderService(ILogger<EmailSenderService> logger, IOptions<EmailConfiguration> emailConfiguration, ISmtpClientFactory stmpClientFactory)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _emailConfiguration = emailConfiguration.Value;
            _smptClientFactory = stmpClientFactory;
        }

        /// <summary>
        /// Task for handling of validating and sending of email message
        /// </summary>
        /// <param name="emailMessage"></param>
        /// <returns></returns>
        /// <exception cref="EmailSendFailedException"></exception>
        public async Task SendEmailAsync(SendEmail emailMessage, CancellationToken cancellationToken)
        {
            try
            {
                // create email message
                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse(emailMessage.From ?? _emailConfiguration.Sender));

                AddRecipients(emailMessage.To, email.To);
                AddRecipients(emailMessage.Cc, email.Cc);
                AddRecipients(emailMessage.Bcc, email.Bcc);

                email.Subject = emailMessage.Subject;

                if (!String.IsNullOrEmpty(emailMessage.PlainTextContent))
                {
                    email.Body = new TextPart(TextFormat.Plain) { Text = emailMessage.PlainTextContent };
                }
                else if (!String.IsNullOrEmpty(emailMessage.HtmlContent))
                {
                    email.Body = new TextPart(TextFormat.Html) { Text = emailMessage.HtmlContent };
                }

                // Should do a check to see if mandatory fields exist.
                if (String.IsNullOrEmpty(emailMessage.Subject) || email.Body is null)
                {
                    _logger.LogError("No subject or message body provided.");
                    throw new InvalidEmailMessageException("No subject or message body provided");
                }
                else if (email.To.Count == 0 && email.Cc.Count == 0 && email.Bcc.Count == 0)
                {
                    _logger.LogError("Missing recipient info.  No To, Cc or Bcc provided");
                    throw new InvalidEmailMessageException("Missing recipient info");
                }

                // send email asynchronously
                var smtp = await _smptClientFactory.CreateAsync(cancellationToken);
                await smtp.SendAsync(email, cancellationToken, null);
                await smtp.DisconnectAsync(true);
            }
            catch (ArgumentNullException ane)
            {
                // host or message is null.
                _logger.LogError(ane, "Host or message is null");
                throw new EmailSendFailedException("Host or message is null", ane);
            }
            catch (ObjectDisposedException ode)
            {
                // The MailKit.MailTransport has been disposed.
                _logger.LogError(ode, "MailTransport has been disposed");
                throw new EmailSendFailedException("MailTransport has been disposed", ode);
            }
            catch (MailKit.ServiceNotConnectedException snce)
            {
                // The MailKit.MailTransport is not connected.
                _logger.LogError(snce, "MailTransport is not connected");
                throw new EmailSendFailedException("MailTransport is not connected", snce);
            }
            catch (MailKit.ServiceNotAuthenticatedException snae)
            {
                // Authentication is required before sending a message.
                _logger.LogError(snae, "Authentication is required before sending a message");
                throw new EmailSendFailedException("Authentication is required before sending a message", snae);
            }
            catch (OperationCanceledException oce)
            {
                // The operation was canceled. (cancellationToken was called)
                _logger.LogError(oce, "The operation was canceled");
                throw new EmailSendFailedException("The operation was canceled", oce);
            }
            catch (System.IO.IOException ioe)
            {
                // An I/O error occurred.
                _logger.LogError(ioe, "An I/O error occurred");
                throw new EmailSendFailedException("An I/O error occurred", ioe);
            }
            catch (MailKit.CommandException ce)
            {
                // The send command failed.
                _logger.LogError(ce, "The send command failed");
                throw new EmailSendFailedException("The send command failed", ce);
            }
            catch (MailKit.ProtocolException pe)
            {
                // A protocol exception occurred.
                _logger.LogError(pe, "A protocol exception occurred");
                throw new EmailSendFailedException("Protocol exception", pe);
            }
            catch (InvalidOperationException ioe)
            {
                // A sender has not been specified.
                // -or-
                // No recipients have been specified.
                _logger.LogError(ioe, "A sender has not been specified");
                throw new EmailSendFailedException("Possible missing sender info", ioe);
            }
            catch (SmtpConnectFailedException scfe)
            {
                // An error connecting to the the SMTP Server
                _logger.LogError(scfe, "An error connecting to the the SMTP Server");
                throw new EmailSendFailedException("An error connecting to the the SMTP Server", scfe);
            }
        }

        /// <summary>
        /// Validate each recipient in the array and add to addressList, if valid.
        /// </summary>
        /// <param name="recipients"></param>
        /// <param name="addressList"></param>
        private void AddRecipients(IList<string> recipients, InternetAddressList addressList)
        {
            if (recipients is not null)
            {
                foreach (var recipient in recipients)
                {
                    if (MailboxAddress.TryParse(recipient, out MailboxAddress mailboxAddress))
                    {
                        if (IsEmailAllowed(mailboxAddress))
                        {
                            addressList.Add(mailboxAddress);
                        }
                        else
                        {
                            // not allowed, metrics? logs?
                            _logger.LogInformation("Recipient email was blocked from being sent to, due to not matching allow list: {Recipient}", recipient);
                        }
                    }
                    else
                    {
                        // invalid email address, metrics? logs?
                        _logger.LogInformation("Recipient email provided was invalid: {Recipient}", recipient);
                    }
                }
            }
        }

        /// <summary>
        /// Go through list of allowed emails, and make sure it matches
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <returns>boolean true, if email is allowed, false otherwise</returns>
        private bool IsEmailAllowed(MailboxAddress mailboxAddress)
        {
            var allowed = _emailConfiguration.Allowed;
            if (allowed.Count > 0)
            {
                // configured with an allow list, does the email address end with any of the allowed domains?
                // Note: assumes the emailAddress is valid.
                return allowed.Any(_ => mailboxAddress.Address.EndsWith(_, StringComparison.OrdinalIgnoreCase));
            }
            return true; // no allow list, production mode, send to anyone
        }
    }

    [Serializable]
    internal class EmailSendFailedException : Exception
    {
        public EmailSendFailedException(string? message, Exception? innerException) : base(message, innerException) { }
    }

    [Serializable]
    internal class InvalidEmailMessageException : Exception
    {
        public InvalidEmailMessageException(string? message) : base(message) { }
    }

}
