using OnliveConstants.Communication;

namespace OnliveConstants.Requests;

public readonly struct SwitchCellsRequest
{
    public IEnumerable<Position> SwitchedCells { get; init; }

    public string ToRequestString()
    {
        var switchedCellsStr = string.Join('|', SwitchedCells.Select(c => c.ToRequestString()));

        return $"{(int)RequestAction.SwitchCells:D2}|{switchedCellsStr}";
    }
}
