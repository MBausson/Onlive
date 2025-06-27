using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.Logging;

namespace OnliveServer;

public class RequestReceivedEventArgs(PlayerClient client, string request) : EventArgs
{
    public PlayerClient Client { get; } = client;
    public string Request { get; } = request;
}

public class SocketServer(string ip, int port) : IDisposable
{
    public event EventHandler<RequestReceivedEventArgs> RequestReceived = null!;

    private readonly TcpListener _listener = new(IPAddress.Parse(ip), port);
    private readonly List<PlayerClient> _clients = [];
    private readonly ILogger<SocketServer> _logger = Helpers.GetLogger<SocketServer>();

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

            var writer = new StreamWriter(client.Stream);

            await writer.WriteLineAsync(func(client));
            await writer.FlushAsync();
        }
    }

    private async Task ReadClientRequestsAsync(PlayerClient client)
    {
        var reader = new StreamReader(client.Stream);

        while (true)
        {
            var request = await ReadRequest(client, reader);

            if (request is null) continue;

            RequestReceived.Invoke(this, new RequestReceivedEventArgs(client, request));
        }
    }

    private async Task<string?> ReadRequest(PlayerClient client, StreamReader reader)
    {
        try
        {
            return await reader.ReadLineAsync();
        }
        catch (IOException)
        {
            EndClientConnection(client);

            return null;
        }
    }

    private void EndClientConnection(PlayerClient client)
    {
        _logger.LogInformation($"Disconnecting client {client.Socket.RemoteEndPoint}");
        _clients.Remove(client);

        client.Socket.Close();
        client.Socket.Dispose();
    }

    public void Dispose()
    {
        _listener.Dispose();
    }
}
