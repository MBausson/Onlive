using OnliveConstants;

namespace OnliveServer;

public class GameBoard
{
    private readonly Dictionary<Position, bool> _activeCells = [];

    public void SwitchValue(Position position)
    {
        if (!_activeCells.Remove(position, out var _))
        {
            _activeCells[position] = true;
        }
    }

    public IEnumerable<Position> GetNeighbors(Position position)
    {
        List<Position> neighbors = [];

        Position[] neighborsMatrix =
        [
            new(-1, -1), new(0, -1), new(1, -1),
            new(-1, 0), new(1, 0),
            new(-1, 1), new(0, 1), new(1, 1),
        ];

        foreach (var pos in neighborsMatrix) if (_activeCells.ContainsKey(position + pos)) neighbors.Add(pos);

        return neighbors;
    }

    public IEnumerable<Position> ActiveCells => _activeCells.Keys;
}
