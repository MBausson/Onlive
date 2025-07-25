using OnliveConstants.Communication;

namespace OnliveConstants.Requests;

public readonly struct PingRequest
{
    /// <summary>
    /// Duration, in milliseconds, between each ping request sent by the client
    /// </summary>
    public const int ClientInterval = 4000;
    /// <summary>
    /// Duration, in milliseconds, before the server closes a client connection if it didn't send a ping request
    /// </summary>
    public const int ServerTimeout = 10000;

    public string ToRequestString() => $"{(int)RequestAction.Ping:D2}|ping";
}
