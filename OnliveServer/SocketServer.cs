using System.Net;
using System.Net.Sockets;
using System.Text;
using Microsoft.Extensions.Logging;
using OnliveServer.Utils;

namespace OnliveServer;

public class RequestReceivedEventArgs(PlayerClient client, string request) : EventArgs
{
    public PlayerClient Client { get; } = client;
    public string Request { get; } = request;
}

public class SocketServer(string ip, int port) : IDisposable
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
        _logger.LogInformation($"Server started on {_listener.Client.LocalEndPoint}");

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

    private async Task WriteToClientAsync(PlayerClient client, string content)
    {
        var data = Encoding.UTF8.GetBytes(content);

        await _listener.SendAsync(data, client.EndPoint);
        _logger.LogTrace($"<<< To {client.EndPoint} => {content}");
    }
}
