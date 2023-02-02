using System.Net.Sockets;

namespace BCGov.VirusScan.Api.Network;

public class TcpClientWrapper : ITcpClient
{
    private bool _disposed;
    
    private TcpClient _client = new TcpClient();

    public int Available =>_client.Available;
    public bool Connected => _client.Connected;

    public ValueTask ConnectAsync(string host, int port, CancellationToken cancellationToken)
    {
        return _client.ConnectAsync(host, port, cancellationToken);
    }

    public void Close() => _client.Close();

    public NetworkStream GetStream() => _client.GetStream();

    public void Dispose()
    {
        Dispose(disposing: true);
        // This object will be cleaned up by the Dispose method.
        // Therefore, you should call GC.SuppressFinalize to
        // take this object off the finalization queue
        // and prevent finalization code for this object
        // from executing a second time.
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            GC.SuppressFinalize(this);
            _client.Dispose();
        }

        _disposed = true;
    }
}
