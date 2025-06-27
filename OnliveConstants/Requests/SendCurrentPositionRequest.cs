using OnliveConstants.Communication;

namespace OnliveConstants.Requests;

public readonly struct SendCurrentPositionRequest
{
    public Position CurrentPosition { get; init; }

    public string ToRequestString() => $"{(int)RequestAction.SendCurrentPosition:D2}|{CurrentPosition.ToRequestString()}";
}
