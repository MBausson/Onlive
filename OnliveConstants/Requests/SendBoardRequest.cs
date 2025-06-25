using OnliveConstants.Communication;

namespace OnliveConstants.Requests;

public readonly struct SendBoardRequest
{
    public IEnumerable<Position> ActiveCells { get; init; }

    public string ToRequestString()
    {
        var activeCellsStr = string.Join('|', ActiveCells.Select(c => c.ToRequestString()));
        
        return $"{(int)RequestAction.SendBoard:D2}|{activeCellsStr}";
    }
}
