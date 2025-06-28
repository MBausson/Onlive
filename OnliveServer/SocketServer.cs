using System.Net;
using System.Net.Sockets;
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
    private readonly List<PlayerClient> _clients = [];

    private readonly TcpListener _listener = new(IPAddress.Parse(ip), port);
    private readonly ILogger<SocketServer> _logger = Logging.GetLogger<SocketServer>();

    public void Dispose()
    {
        _listener.Dispose();
    }

    public event EventHandler<RequestReceivedEventArgs> RequestReceived = null!;

    public async Task StartAsync()
    {
        _listener.Start();

        while (true)
        {
            var tcpClient = await _listener.AcceptTcpClientAsync();
            tcpClient.NoDelay = true;

            var client = new PlayerClient(tcpClient);

            _logger.LogInformation($"New connected client : {tcpClient.Client.RemoteEndPoint}");

            _clients.Add(client);
            _ = ReadClientRequestsAsync(client);
        }
    }

    public async Task SendToAllClientsAsync(Func<PlayerClient, string> func)
    {
        foreach (var client in _clients)
        {
            if (!client.Connected)
            {
                EndClientConnection(client);
                continue;
            }

            await WriteToClientAsync(client, func(client));
        }
    }

    private async Task ReadClientRequestsAsync(PlayerClient client)
    {
        var reader = new StreamReader(client.Stream);

        while (true)
        {
            var request = await ReadRequestAsync(client, reader);
            _logger.LogTrace($">>> From {client.Socket.RemoteEndPoint} => {request}");

            if (request.TerminateConnection) break;

            if (request.Content is null)
            {
                _logger.LogDebug($"Could not read request from {client.Socket.RemoteEndPoint}");
                continue;
            }

            RequestReceived.Invoke(this, new RequestReceivedEventArgs(client, request.Content));
        }
    }

    private async Task WriteToClientAsync(PlayerClient client, string content)
    {
        var writer = new StreamWriter(client.Stream);

        await writer.WriteLineAsync(content);
        _logger.LogTrace($"<<< To {client.Socket.RemoteEndPoint} => {content}");

        await writer.FlushAsync();
    }

    private async Task<ReadResult> ReadRequestAsync(PlayerClient client, StreamReader reader)
    {
        try
        {
            return new (false, await reader.ReadLineAsync());
        }
        catch (IOException)
        {
            EndClientConnection(client);

            return new(true, null);
        }
    }

    private void EndClientConnection(PlayerClient client)
    {
        _logger.LogInformation($"Disconnecting client {client.Socket.RemoteEndPoint}");
        _clients.Remove(client);

        client.Socket.Close();
        client.Socket.Dispose();
    }

    private record struct ReadResult(bool TerminateConnection, string? Content);
}
