using OnliveConstants.Communication;

namespace OnliveConstants.Requests;

public readonly struct SwitchCellRequest
{
    public Position SwitchedCell { get; init; }

    public string ToRequestString() => $"{(int)RequestAction.SwitchCell:D2}|{SwitchedCell.ToRequestString()}";
}
