using System.Net;
using System.Net.Sockets;

namespace OnliveServer;

public class RequestReceivedEventArgs(string request) : EventArgs
{
    public string Request { get; } = request;
}

public class SocketServer(string ip, int port) : IDisposable
{
    public event EventHandler<RequestReceivedEventArgs> RequestReceived = null!;

    private readonly TcpListener _listener = new(IPAddress.Parse(ip), port);
    private readonly List<TcpClient> _clients = [];

    public async Task StartAsync()
    {
        _listener.Start();

        while (true)
        {
            var client = await _listener.AcceptTcpClientAsync();

            _clients.Add(client);

            _ = ReadClientRequestsAsync(client);
        }
    }

    public async Task SendToAllClientsAsync(Func<TcpClient, string> func)
    {
        foreach (var client in _clients)
        {
            if (!client.Connected)
            {
                EndClientConnection(client);
                continue;
            }

            var writer = new StreamWriter(client.GetStream());

            await writer.WriteLineAsync(func(client));
            await writer.FlushAsync();
        }
    }

    private async Task ReadClientRequestsAsync(TcpClient client)
    {
        //  string? clientEndPoint = client.Client.RemoteEndPoint?.ToString();

        while (true)
        {
            var request = await ReadRequest(client);

            if (request is null) continue;

            RequestReceived.Invoke(this, new RequestReceivedEventArgs(request));
        }
    }

    private async Task<string?> ReadRequest(TcpClient client)
    {
        var stream = client.GetStream();
        var reader = new StreamReader(stream);

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

    private void EndClientConnection(TcpClient client)
    {
        _clients.Remove(client);

        client.Close();
        client.Dispose();
    }

    public void Dispose()
    {
        _listener.Dispose();
    }
}
