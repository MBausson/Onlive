namespace OnliveConstants.Requests;

public readonly struct SendBoardRequest
{
    public IEnumerable<Position> ActiveCells { get; init; }
}
