using System.Net;
using OnliveConstants;
using OnliveConstants.Requests;

namespace OnliveServer;

public class PlayerClient(IPEndPoint endPoint)
{
    public IPEndPoint EndPoint { get; } = endPoint;

    public Position CurrentPosition { get; set; }

    public DateTimeOffset LastPing { get; set; } = DateTimeOffset.Now;

    public bool ShouldDisconnect =>
        LastPing < DateTimeOffset.Now + TimeSpan.FromSeconds(PingRequest.ServerTimeout);
}
