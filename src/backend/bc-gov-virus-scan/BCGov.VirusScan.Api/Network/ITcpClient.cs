using System.Net;
using System.Net.Sockets;

namespace BCGov.VirusScan.Api.Network;

/// <summary>
/// Represents an abstraction over <see cref="TcpClient"/>.
/// </summary>
public interface ITcpClient : IDisposable
{
    /// <summary>
    /// Connects the client to the specified TCP port on the specified host as an asynchronous operation.
    /// </summary>
    /// <param name="host">The DNS name of the remote host.</param>
    /// <param name="port">The port number of the remote host.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to signal the asynchronous operation should be canceled.</param>
    /// <returns>A task that represents the asynchronous connection operation.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="host"/> parameter is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The <paramref name="port"/> parameter is not between <see cref="IPEndPoint.MinPort" /> and <see cref="IPEndPoint.MaxPort" />.</exception>
    /// <exception cref="ObjectDisposedException">The <see cref="ITcpClient"/> is closed.</exception>
    /// <exception cref="SocketException">An error occurred when accessing the socket.</exception>
    ValueTask ConnectAsync(string host, int port, CancellationToken cancellationToken);

    /// <summary>
    /// Disposes this <see cref="ITcpClient"/> instance and requests that the underlying TCP connection be closed.
    /// </summary>
    void Close();

    /// <summary>
    /// Returns the <see cref="NetworkStream"/> used to send and receive data.
    /// </summary>
    /// <exception cref="InvalidOperationException">The <see cref="ITcpClient"/> is not connected to a remote host.</exception>
    /// <exception cref="ObjectDisposedException">The <see cref="ITcpClient"/> has been closed.</exception>
    /// <returns></returns>
    NetworkStream GetStream();

    /// <summary>
    /// Gets the amount of data that has been received from the network and is available to be read.
    /// </summary>
    /// <exception cref="SocketException">An error occurred when accessing the socket.</exception>
    int Available { get; }

    /// <summary>
    /// Gets a value indicating whether the underlying Socket for a <see cref="ITcpClient"/> is connected to a remote host.
    /// </summary>
    /// <value><see langword="true" /> if the  <see cref="ITcpClient"/> socket was connected to a remote resource as of the most recent operation; otherwise, <see langword="false" />.</value>
    bool Connected { get; }
}
