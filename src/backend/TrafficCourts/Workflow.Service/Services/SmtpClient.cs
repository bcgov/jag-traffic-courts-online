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
    public class SmtpClient : ISmtpClient
    {
        private readonly ILogger<SmtpClient> _logger;
        private readonly SmtpConfiguration _stmpConfiguration;

        public SmtpClient(ILogger<SmtpClient> logger, IOptions<SmtpConfiguration> stmpConfiguration)
        {
            _logger = logger;
            _stmpConfiguration = stmpConfiguration.Value;
        }

        public async Task<MailKit.Net.Smtp.SmtpClient> CreateAsync(CancellationToken cancellationToken)
        {
            try
            {
                using var smtp = new MailKit.Net.Smtp.SmtpClient();
                await smtp.ConnectAsync(_stmpConfiguration.Host, _stmpConfiguration.Port, SecureSocketOptions.Auto, cancellationToken);
                return smtp;
            }
            catch (ArgumentNullException ane) {
                // host or message is null.
                _logger.LogError(ane, "Host or message is null.");
                throw new SmtpConnectFailedException($"Host or message is null", ane);
            }
            catch (ArgumentOutOfRangeException aore) {
                // port is not between 0 and 65535.
                _logger.LogError(aore, "Port is not between 0 and 65535.");
                throw new SmtpConnectFailedException($"Port is not between 0 and 65535", aore);
            }
            catch (ArgumentException ae)
            {
                // The host is a zero-length string.
                _logger.LogError(ae, "The host is a zero-length string.");
                throw new SmtpConnectFailedException($"The host is a zero-length string", ae);
            }
            catch (ObjectDisposedException ode)
            {
                // The MailKit.Net.Smtp.SmtpClient has been disposed.
                _logger.LogError(ode, "SmtpClient has been disposed.");
                throw new SmtpConnectFailedException($"SmtpClient has been disposed", ode);
            }
            catch (NotSupportedException nse)
            {
                // options was set to MailKit.Security.SecureSocketOptions.StartTls and the SMTP
                // server does not support the STARTTLS extension.
                _logger.LogError(nse, "STMP server does not support the STARTTLS extension.");
                throw new SmtpConnectFailedException($"STMP server does not support the STARTTLS extension", nse);
            }
            /*catch (OperationCanceledException oce)
            {
                // The operation was canceled.
                _logger.LogError(oce, "The operation was canceled.");
                throw new SmtpConnectFailedException($"The operation was canceled", oce);
            }*/
            catch (System.Net.Sockets.SocketException se)
            {
                // A socket error occurred trying to connect to the remote host.
                _logger.LogError(se, "A socket error occurred trying to connect to the remote host.");
                throw new SmtpConnectFailedException($"A socket error occurred trying to connect to the remote host", se);
            }
            catch (MailKit.Security.SslHandshakeException she)
            {
                // An error occurred during the SSL/TLS negotiations.
                _logger.LogError(she, "An error occurred during the SSL/TLS negotiations.");
                throw new SmtpConnectFailedException($"An error occurred during the SSL/TLS negotiations", she);
            }
            catch (System.IO.IOException ioe)
            {
                // An I/O error occurred.
                _logger.LogError(ioe, "An I/O error occurred.");
                throw new SmtpConnectFailedException($"An I/O error occurred", ioe);
            }
            catch (MailKit.Net.Smtp.SmtpCommandException sce)
            {
                // An SMTP command failed.
                _logger.LogError(sce, "An SMTP command failed.");
                throw new SmtpConnectFailedException($"An SMTP command failed", sce);
            }
            catch (MailKit.Net.Smtp.SmtpProtocolException spe)
            {
                // An SMTP protocol error occurred.
                _logger.LogError(spe, "An SMTP protocol error occurred.");
                throw new SmtpConnectFailedException($"An SMTP protocol error occurred", spe);
            }
            catch (InvalidOperationException ioe)
            {
                // The MailKit.Net.Smtp.SmtpClient is already connected.
                _logger.LogError(ioe, "The SmtpClient is already connected.");
                throw new SmtpConnectFailedException($"The SmtpClient is already connected", ioe);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "General smtp connection exception thrown.");
                throw new SmtpConnectFailedException($"General smtp connection exception thrown", ex);

            }
        }
    }

    [Serializable]
    internal class SmtpConnectFailedException : Exception
    {
        public SmtpConnectFailedException() { }
        public SmtpConnectFailedException(string? message) : base(message) { }
        public SmtpConnectFailedException(string? message, Exception? innerException) : base(message, innerException) { }
        protected SmtpConnectFailedException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

}
