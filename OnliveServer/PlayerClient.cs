using System.Net;
using OnliveConstants;

namespace OnliveServer;

public class PlayerClient(IPEndPoint endPoint)
{
    public IPEndPoint EndPoint { get; } = endPoint;
    public Position CurrentPosition { get; set; }
}
