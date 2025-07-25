using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Extensions.Logging;
using OnliveConstants.Requests;
using OnliveServer.Utils;

namespace OnliveServer;

public class RequestReceivedEventArgs(PlayerClient client, string request) : EventArgs
{
    public PlayerClient Client { get; } = client;
    public string Request { get; } = request;
}

public class SocketServer(int port) : IDisposable
{
    private readonly Dictionary<IPEndPoint, PlayerClient> _clients = [];
    private readonly UdpClient _listener = new(port);
    private readonly ILogger<SocketServer> _logger = Logging.GetLogger<SocketServer>();

    public void Dispose()
    {
        _listener.Dispose();
    }

    public event EventHandler<RequestReceivedEventArgs> RequestReceived = null!;

    public async Task StartAsync()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            // https://stackoverflow.com/a/74327430
            // https://learn.microsoft.com/en-us/windows/win32/winsock/winsock-ioctls#sio_udp_connreset-opcode-setting-i-t3
            const uint iocIn = 0x80000000U;
            const uint iocVendor = 0x18000000U;
            const int sioUdpConnReset = unchecked((int)(iocIn | iocVendor | 12));

            _listener.Client.IOControl(sioUdpConnReset, [ 0x00 ], null);
        }

        _logger.LogInformation($"Server started on {_listener.Client.LocalEndPoint}");

        _ = TimeoutClientsAsync();

        while (true)
        {
            var result = await _listener.ReceiveAsync();
            var request = Encoding.UTF8.GetString(result.Buffer);

            if (request.Length == 0) continue;

            PlayerClient client;

            if (_clients.ContainsKey(result.RemoteEndPoint))
            {
                client = _clients[result.RemoteEndPoint];
            }
            else
            {
                client = new PlayerClient(result.RemoteEndPoint);
                _clients[result.RemoteEndPoint] = client;
            }

            _logger.LogTrace($">>> From {result.RemoteEndPoint} => {request}");

            RequestReceived.Invoke(this, new RequestReceivedEventArgs(client, request));
        }
    }

    public async Task SendToAllClientsAsync(Func<PlayerClient, string> func)
    {
        foreach (var (_, client) in _clients)
        {
            await WriteToClientAsync(client, func(client));
        }
    }

    private async Task TimeoutClientsAsync()
    {
        while (true)
        {
            await Task.Delay(PingRequest.ServerTimeout);

            foreach (var (ip, client) in _clients)
            {
                if (client.ShouldDisconnect)
                {
                    _logger.LogInformation($"Closing connection with client {ip}");
                    _clients.Remove(ip);
                }
            }
        }
    }

    private async Task WriteToClientAsync(PlayerClient client, string content)
    {
        var data = Encoding.UTF8.GetBytes(content);

        await _listener.SendAsync(data, client.EndPoint);
        _logger.LogTrace($"<<< To {client.EndPoint} => {content}");
    }
}
