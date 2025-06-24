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

    public async Task StartAsync()
    {
        _listener.Start();

        while (true)
        {
            var client = await _listener.AcceptTcpClientAsync();

            _ = ProcessClient(client);
        }
    }

    private async Task ProcessClient(TcpClient client)
    {
        //  string? clientEndPoint = client.Client.RemoteEndPoint?.ToString();

        var stream = client.GetStream();
        var reader = new StreamReader(stream);

        while (true)
        {
            string? request = await reader.ReadLineAsync();

            if (request is null) continue;

            RequestReceived.Invoke(this, new RequestReceivedEventArgs(request));
        }
    }

    public void Dispose()
    {
        _listener.Dispose();
    }
}
