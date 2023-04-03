using MimeKit;
using MimeKit.Text;
using TrafficCourts.Workflow.Service.Configuration;
using TrafficCourts.Common.Features.Mail;
using MailKit.Net.Smtp;

namespace TrafficCourts.Workflow.Service.Services
{
    public class EmailSenderService : IEmailSenderService
    {
        private readonly ILogger<EmailSenderService> _logger;
        private readonly EmailConfiguration _emailConfiguration;
        private readonly ISmtpClientFactory _smptClientFactory;

        public EmailSenderService(EmailConfiguration emailConfiguration, ISmtpClientFactory stmpClientFactory, ILogger<EmailSenderService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _emailConfiguration = emailConfiguration ?? throw new ArgumentNullException(nameof(emailConfiguration));
            _smptClientFactory = stmpClientFactory ?? throw new ArgumentNullException(nameof(stmpClientFactory));
         }

        /// <summary>
        /// Task for handling of validating and sending of email message
        /// </summary>
        /// <param name="emailMessage"></param>
        /// <returns></returns>
        /// <exception cref="EmailSendFailedException"></exception>
        public async Task<SendEmailResult> SendEmailAsync(EmailMessage emailMessage, CancellationToken cancellationToken)
        {
            MailKit.Net.Smtp.ISmtpClient? smtp = null;

            try
            {
                // create email message
                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse(emailMessage.From ?? _emailConfiguration.Sender));

                // add recipients
                bool toAdded = AddRecipient(emailMessage.To, email.To);

                if (IsAllowedListConfigured)
                {
                    // in development, some addresses many not allowed, add AddRecipients will filter out any unallowed email address
                    // we need to exit before an exception is thrown below due to noone left in the "To" address list.
                    if (!toAdded && IsEmailValid(emailMessage.To) && !IsEmailAllowed(emailMessage.To))
                    {
                        // there is a valid to address, however, non of them are allowed, note we are really using only CC and BCC emails
                        _logger.LogInformation("Not sending email because none of the valid email addresses are allowed to be set to. See configuration AllowList");
                        return SendEmailResult.Filtered;
                    }
                }

                email.Subject = emailMessage.Subject;

                if (!String.IsNullOrEmpty(emailMessage.TextContent))
                {
                    email.Body = new TextPart(TextFormat.Plain) { Text = emailMessage.TextContent };
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
                smtp = await _smptClientFactory.CreateAsync(cancellationToken);

                await SendAsync(smtp, email, cancellationToken);

                return SendEmailResult.Success;
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
            finally
            {
                if (smtp is not null && smtp.IsConnected)
                {
                    await DisconnectAsync(smtp, cancellationToken);
                }
            }
        }

        private async Task<string> SendAsync(MailKit.Net.Smtp.ISmtpClient client, MimeMessage message, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(client);
            ArgumentNullException.ThrowIfNull(message);

            using var operation = Instrumentation.Smtp.BeginOperation(nameof(ISmtpClient.SendAsync));

            try
            {
                var response = await client.SendAsync(message, cancellationToken, null).ConfigureAwait(false);
                return response;
            }
            catch (Exception exception)
            {
                Instrumentation.Smtp.EndOperation(operation, exception);
                throw;
            }
        }

        private async Task DisconnectAsync(MailKit.Net.Smtp.ISmtpClient client, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(client);

            using var operation = Instrumentation.Smtp.BeginOperation(nameof(ISmtpClient.DisconnectAsync));

            try
            {
                await client.DisconnectAsync(true, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                Instrumentation.Smtp.EndOperation(operation, exception);
                throw;
            }
        }

        /// <summary>
        /// Determines if any of the recipients are valid email addresses and are allowed to be set to.
        /// </summary>
        /// <param name="recipients"></param>
        /// <returns><c>true</c> if there is a valid email address and is allowed to be set to, otherwise <c>false</c>.</returns>
        private bool IsEmailAllowed(string recipient)
        {
            if (recipient is not null)
            {
                if (MailboxAddress.TryParse(recipient, out MailboxAddress mailboxAddress))
                {
                    if (IsEmailAllowed(mailboxAddress))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Determins if any of the recipients are valid email addresses
        /// </summary>
        /// <param name="recipients"></param>
        /// <returns><c>true</c> if there is a valid email address, otherwise <c>false</c>.</returns>
        private bool IsEmailValid(string recipient)
        {
            if (recipient is not null)
            {
                if (MailboxAddress.TryParse(recipient, out _))
                {
                    return true;
                }
            }

            return false;
        }


        /// <summary>
        /// Validate each recipient and if is a valid email address, adds to the <see cref="addressList"/>.
        /// </summary>
        /// <param name="recipients"></param>
        /// <param name="addressList"></param>
        /// <returns><c>true</c> if any recipient was added to the <see cref="addressList"/>, otherwise <c>false</c>.</returns>
        private bool AddRecipient(string recipient, InternetAddressList addressList)
        {
            bool added = false;

            if (recipient is not null)
            {
                if (MailboxAddress.TryParse(recipient, out MailboxAddress mailboxAddress))
                {
                    if (IsEmailAllowed(mailboxAddress))
                    {
                        addressList.Add(mailboxAddress);
                        added = true;
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

            return added;
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

        /// <summary>
        /// Determines if the allowed email list is configured or not.
        /// </summary>
        private bool IsAllowedListConfigured => _emailConfiguration.Allowed.Count > 0;
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
