using Microsoft.Extensions.Options;
using TrafficCourts.Workflow.Service.Configuration;
using TrafficCourts.Messaging.MessageContracts;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using System.Runtime.Serialization;

namespace TrafficCourts.Workflow.Service.Services
{
    public class EmailSenderService : IEmailSenderService
    {
        private readonly ILogger<EmailSenderService> _logger;
        private readonly SmtpConfiguration _stmpConfiguration;

        public EmailSenderService(ILogger<EmailSenderService> logger, IOptions<SmtpConfiguration> stmpConfiguration)
        {
            _logger = logger;
            _stmpConfiguration = stmpConfiguration.Value;
        }

        public async Task SendEmailAsync(SendEmail emailMessage, CancellationToken cancellationToken)
        {
            // create email message
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(emailMessage.From));
            foreach (var recipient in emailMessage.To)
            {
                if(!String.IsNullOrEmpty(recipient)) {
                  email.To.Add(MailboxAddress.Parse(recipient));
                }
            }
            foreach (var ccRecipient in emailMessage.Cc)
            {
                if (!String.IsNullOrEmpty(ccRecipient))
                {
                    email.Cc.Add(MailboxAddress.Parse(ccRecipient));
                }
            }
            foreach (var bccRecipient in emailMessage.Bcc)
            {
                if (!String.IsNullOrEmpty(bccRecipient))
                {
                    email.Bcc.Add(MailboxAddress.Parse(bccRecipient));
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

            try
            {
                // send email asynchronously
                using var smtp = new SmtpClient();
                await smtp.ConnectAsync(_stmpConfiguration.Host, _stmpConfiguration.Port, SecureSocketOptions.Auto, cancellationToken);
                await smtp.SendAsync(email, cancellationToken);
                await smtp.DisconnectAsync(true);
            }
            catch (ArgumentNullException ane) {
                // host or message is null.
                _logger.LogError(ane, "Host or message is null.");
                throw new EmailSendFailedException($"Host or message is null", ane);
            }
            catch (ArgumentOutOfRangeException aore) {
                // port is not between 0 and 65535.
                _logger.LogError(aore, "Port is not between 0 and 65535.");
                throw new EmailSendFailedException($"Port is not between 0 and 65535", aore);
            }
            catch (ArgumentException ae)
            {
                // The host is a zero-length string.
                _logger.LogError(ae, "The host is a zero-length string.");
                throw new EmailSendFailedException($"The host is a zero-length string", ae);
            }
            catch (ObjectDisposedException ode)
            {
                // The MailKit.Net.Smtp.SmtpClient has been disposed.
                // The MailKit.MailTransport has been disposed.
                _logger.LogError(ode, "SmtpClient or MailTransport has been disposed.");
                throw new EmailSendFailedException($"SmtpClient or MailTransport has been disposed", ode);
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
            catch (NotSupportedException nse)
            {
                // options was set to MailKit.Security.SecureSocketOptions.StartTls and the SMTP
                // server does not support the STARTTLS extension.
                _logger.LogError(nse, "STMP server does not support the STARTTLS extension.");
                throw new EmailSendFailedException($"STMP server does not support the STARTTLS extension", nse);
            }
            /*catch (OperationCanceledException oce)
            {
                // The operation was canceled.
                _logger.LogError(oce, "The operation was canceled.");
                throw new EmailSendFailedException($"The operation was canceled", oce);
            }*/
            catch (System.Net.Sockets.SocketException se)
            {
                // A socket error occurred trying to connect to the remote host.
                _logger.LogError(se, "A socket error occurred trying to connect to the remote host.");
                throw new EmailSendFailedException($"A socket error occurred trying to connect to the remote host", se);
            }
            catch (MailKit.Security.SslHandshakeException she)
            {
                // An error occurred during the SSL/TLS negotiations.
                _logger.LogError(she, "An error occurred during the SSL/TLS negotiations.");
                throw new EmailSendFailedException($"An error occurred during the SSL/TLS negotiations", she);
            }
            catch (System.IO.IOException ioe)
            {
                // An I/O error occurred.
                _logger.LogError(ioe, "An I/O error occurred.");
                throw new EmailSendFailedException($"An I/O error occurred", ioe);
            }
            catch (MailKit.Net.Smtp.SmtpCommandException sce)
            {
                // An SMTP command failed.
                _logger.LogError(sce, "An SMTP command failed.");
                throw new EmailSendFailedException($"An SMTP command failed", sce);
            }
            catch (MailKit.Net.Smtp.SmtpProtocolException spe)
            {
                // An SMTP protocol error occurred.
                _logger.LogError(spe, "An SMTP protocol error occurred.");
                throw new EmailSendFailedException($"An SMTP protocol error occurred", spe);
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
                // The MailKit.Net.Smtp.SmtpClient is already connected.
                // A sender has not been specified.
                // -or-
                // No recipients have been specified.
                _logger.LogError(ioe, "The SmtpClient is already connected or a sender has not been specified or no recipients have been specified.");
                throw new EmailSendFailedException($"Possible missing sender or recipient info", ioe);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "General email send exception thrown.");
                throw new EmailSendFailedException($"General email send exception thrown", ex);

            }
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
