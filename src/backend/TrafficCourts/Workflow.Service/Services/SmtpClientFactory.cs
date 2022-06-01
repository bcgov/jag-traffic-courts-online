using MailKit.Net.Smtp;
using MailKit.Security;
using TrafficCourts.Workflow.Service.Configuration;

namespace TrafficCourts.Workflow.Service.Services;

public class SmtpClientFactory : ISmtpClientFactory
{
    private readonly ILogger<SmtpClientFactory> _logger;
    private readonly SmtpConfiguration _stmpConfiguration;

    public SmtpClientFactory(ILogger<SmtpClientFactory> logger, SmtpConfiguration stmpConfiguration)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _stmpConfiguration = stmpConfiguration ?? throw new ArgumentNullException(nameof(stmpConfiguration));
    }

    public async Task<ISmtpClient> CreateAsync(CancellationToken cancellationToken)
    {
        try
        {
            SmtpClient smtp = new();
            await smtp.ConnectAsync(_stmpConfiguration.Host, _stmpConfiguration.Port, SecureSocketOptions.Auto, cancellationToken);
            return smtp;
        }
        catch (ArgumentNullException exception) {
            // host or message is null.
            _logger.LogError(exception, "Host or message is null");
            throw new SmtpConnectFailedException("Host or message is null", exception);
        }
        catch (ArgumentOutOfRangeException exception) {
            // port is not between 0 and 65535.
            _logger.LogError(exception, "Port is not between 0 and 65535");
            throw new SmtpConnectFailedException("Port is not between 0 and 65535", exception);
        }
        catch (ArgumentException exception)
        {
            // The host is a zero-length string.
            _logger.LogError(exception, "The host is a zero-length string");
            throw new SmtpConnectFailedException("The host is a zero-length string", exception);
        }
        catch (ObjectDisposedException exception)
        {
            // The MailKit.Net.Smtp.SmtpClient has been disposed.
            _logger.LogError(exception, "SmtpClient has been disposed");
            throw new SmtpConnectFailedException("SmtpClient has been disposed", exception);
        }
        catch (NotSupportedException exception)
        {
            // options was set to MailKit.Security.SecureSocketOptions.StartTls and the SMTP
            // server does not support the STARTTLS extension.
            _logger.LogError(exception, "STMP server does not support the STARTTLS extension");
            throw new SmtpConnectFailedException("STMP server does not support the STARTTLS extension", exception);
        }
        /*catch (OperationCanceledException oce)
        {
            // The operation was canceled.
            _logger.LogError(oce, "The operation was canceled.");
            throw new SmtpConnectFailedException($"The operation was canceled", oce);
        }*/
        catch (System.Net.Sockets.SocketException exceptione)
        {
            // A socket error occurred trying to connect to the remote host.
            _logger.LogError(exceptione, "A socket error occurred trying to connect to the remote host");
            throw new SmtpConnectFailedException("A socket error occurred trying to connect to the remote host", exceptione);
        }
        catch (SslHandshakeException exception)
        {
            // An error occurred during the SSL/TLS negotiations.
            _logger.LogError(exception, "An error occurred during the SSL/TLS negotiations");
            throw new SmtpConnectFailedException("An error occurred during the SSL/TLS negotiations", exception);
        }
        catch (IOException exception)
        {
            // An I/O error occurred.
            _logger.LogError(exception, "An I/O error occurred");
            throw new SmtpConnectFailedException("An I/O error occurred", exception);
        }
        catch (SmtpCommandException exception)
        {
            // An SMTP command failed.
            _logger.LogError(exception, "An SMTP command failed");
            throw new SmtpConnectFailedException("An SMTP command failed", exception);
        }
        catch (SmtpProtocolException exception)
        {
            // An SMTP protocol error occurred.
            _logger.LogError(exception, "An SMTP protocol error occurred");
            throw new SmtpConnectFailedException("An SMTP protocol error occurred", exception);
        }
        catch (InvalidOperationException exception)
        {
            // The MailKit.Net.Smtp.SmtpClient is already connected.
            _logger.LogError(exception, "The SmtpClient is already connected");
            throw new SmtpConnectFailedException("The SmtpClient is already connected", exception);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "General smtp connection exception thrown");
            throw new SmtpConnectFailedException("General smtp connection exception thrown", exception);
        }
    }
}
