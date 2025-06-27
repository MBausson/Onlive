using System.Net.Sockets;
using OnliveConstants;

namespace OnliveServer;

public class PlayerClient(TcpClient client)
{
    public NetworkStream Stream { get; } = client.GetStream();
    public Socket Socket { get; } = client.Client;
    public Position CurrentPosition { get; set; }

    public bool Connected => client.Connected;
}
