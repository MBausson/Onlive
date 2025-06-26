using System.Net.Sockets;

namespace OnliveServer;

public class PlayerClient(TcpClient client)
{
    public NetworkStream Stream { get; } = client.GetStream();
    public Socket Socket { get; } = client.Client;

    public bool Connected => client.Connected;
}
