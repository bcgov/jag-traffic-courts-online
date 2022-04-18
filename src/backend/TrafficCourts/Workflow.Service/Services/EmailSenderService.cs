using Microsoft.Extensions.Options;
using TrafficCourts.Workflow.Service.Configuration;
using TrafficCourts.Workflow.Service.Models;
using MimeKit;
using MimeKit.Text;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace TrafficCourts.Workflow.Service.Services
{
    public class EmailSenderService : IEmailSenderService
    {
        private readonly ILogger<EmailSenderService> _logger;
        private readonly EmailConfiguration _emailConfiguration;
        private readonly ISmtpClientFactory _smptClientFactory;

        public EmailSenderService(ILogger<EmailSenderService> logger, IOptions<EmailConfiguration> emailConfiguration, ISmtpClientFactory stmpClientFactory)
        {
            _logger = logger;
            _emailConfiguration = emailConfiguration.Value;
            _smptClientFactory = stmpClientFactory;
        }

        public async Task SendEmailAsync(EmailMessage emailMessage)
        {
            try
            {
                // create email message
                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse(emailMessage.From??_emailConfiguration.Sender));

                if(emailMessage.To != null)
                {
                    foreach (var recipient in emailMessage.To)
                    {
                        if(IsValidEmail(recipient) && IsEmailAllowed(recipient))
                        {
                          email.To.Add(MailboxAddress.Parse(recipient));
                        }
                    }
                }
                if(emailMessage.Cc != null)
                {
                    foreach (var ccRecipient in emailMessage.Cc)
                    {
                        if (IsValidEmail(ccRecipient) && IsEmailAllowed(ccRecipient))
                        {
                            email.Cc.Add(MailboxAddress.Parse(ccRecipient));
                        }
                    }
                }
                if(emailMessage.Bcc != null)
                {
                    foreach (var bccRecipient in emailMessage.Bcc)
                    {
                        if (IsValidEmail(bccRecipient) && IsEmailAllowed(bccRecipient))
                        {
                            email.Bcc.Add(MailboxAddress.Parse(bccRecipient));
                        }
                    }
                }

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
                if(String.IsNullOrEmpty(emailMessage.Subject) || email.Body == null /*||
                    (email.To.Count == 0 && email.Cc.Count == 0 && email.Bcc.Count == 0)*/)
                {
                    throw new ArgumentNullException();
                }
                else if(email.To.Count == 0 && email.Cc.Count == 0 && email.Bcc.Count == 0)
                {
                    throw new InvalidOperationException();
                }

                // TODO: add time-out config for connection? (hard-coded as 5000 ms for now)

                CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
                cancellationTokenSource.CancelAfter(5000);
                var cancellationToken = cancellationTokenSource.Token;

                // send email asynchronously
                var smtp = await _smptClientFactory.CreateAsync(cancellationToken);
                await smtp.SendAsync(email, cancellationToken, null);
                await smtp.DisconnectAsync(true);
            }
            catch (ArgumentNullException ane) {
                // host or message is null.
                _logger.LogError(ane, "Host or message is null.");
                throw new EmailSendFailedException($"Host or message is null", ane);
            }
            catch (ObjectDisposedException ode)
            {
                // The MailKit.MailTransport has been disposed.
                _logger.LogError(ode, "MailTransport has been disposed.");
                throw new EmailSendFailedException($"MailTransport has been disposed", ode);
            }
            catch (MailKit.ServiceNotConnectedException snce)
            {
                // The MailKit.MailTransport is not connected.
                _logger.LogError(snce, "MailTransport is not connected.");
                throw new EmailSendFailedException($"MailTransport is not connected", snce);
            }
            catch (MailKit.ServiceNotAuthenticatedException snae)
            {
                // Authentication is required before sending a message.
                _logger.LogError(snae, "Authentication is required before sending a message.");
                throw new EmailSendFailedException($"Authentication is required before sending a message", snae);
            }
            catch (OperationCanceledException oce)
            {
                // The operation was canceled. (cancellationToken was called)
                _logger.LogError(oce, "The operation was canceled.");
                throw new EmailSendFailedException($"The operation was canceled", oce);
            }
            catch (System.IO.IOException ioe)
            {
                // An I/O error occurred.
                _logger.LogError(ioe, "An I/O error occurred.");
                throw new EmailSendFailedException($"An I/O error occurred", ioe);
            }
            catch (MailKit.CommandException ce)
            {
                // The send command failed.
                _logger.LogError(ce, "The send command failed.");
                throw new EmailSendFailedException($"The send command failed", ce);
            }
            catch (MailKit.ProtocolException pe)
            {
                // A protocol exception occurred.
                _logger.LogError(pe, "A protocol exception occurred.");
                throw new EmailSendFailedException($"Protocol exception", pe);
            }
            catch (InvalidOperationException ioe)
            {
                // A sender has not been specified.
                // -or-
                // No recipients have been specified.
                _logger.LogError(ioe, "A sender has not been specified or no recipients have been specified.");
                throw new EmailSendFailedException($"Possible missing sender or recipient info", ioe);
            }
            catch(SmtpConnectFailedException scfe)
            {
                // An error connecting to the the SMTP Server
                _logger.LogError(scfe, "An error connecting to the the SMTP Server.");
                throw new EmailSendFailedException($"An error connecting to the the SMTP Server", scfe);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "General email send exception thrown.");
                throw new EmailSendFailedException($"General email send exception thrown", ex);

            }
        }

        // Validation to check if the passed-in e-mail is in a valid format.
        private static bool IsValidEmail(string emailAddress)
        {
            try
            {
                System.Net.Mail.MailAddress m = new System.Net.Mail.MailAddress(emailAddress);

                return true;
            }
            catch (ArgumentNullException)
            {
                return false;
            }
            catch (ArgumentException)
            {
                return false;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        // Go through list of allowed emails, and make sure it matches
        private bool IsEmailAllowed(string emailAddress)
        {
            if(_emailConfiguration.AllowList != null)
            {
                foreach (var allowedEmail in _emailConfiguration.AllowList)
                {

                    Match m = Regex.Match(emailAddress, allowedEmail, RegexOptions.IgnoreCase);
                    if (m.Success)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }


    [Serializable]
    internal class EmailSendFailedException : Exception
    {
        public EmailSendFailedException() { }
        public EmailSendFailedException(string? message) : base(message) { }
        public EmailSendFailedException(string? message, Exception? innerException) : base(message, innerException) { }
        protected EmailSendFailedException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

}
