using OnliveConstants;

namespace OnliveServer;

public class GameBoard
{
    private Dictionary<Position, bool> _activeCells = [];

    public IEnumerable<Position> ActiveCells => _activeCells.Keys;

    public void SwitchValue(Position position)
    {
        if (!_activeCells.Remove(position, out var _)) _activeCells[position] = true;
    }

    public void Update()
    {
        var newActiveCells = new Dictionary<Position, bool>();
        var neighborCounts = new Dictionary<Position, int>();

        // Count neighbors for each living cell and their neighbors
        foreach (var cell in _activeCells.Keys)
        foreach (var offset in GetNeighborOffsets())
        {
            var neighbor = cell + offset;

            neighborCounts.TryAdd(neighbor, 0);
            neighborCounts[neighbor]++;
        }

        // Apply rules
        foreach (var kvp in neighborCounts)
        {
            var position = kvp.Key;
            var count = kvp.Value;
            var isAlive = _activeCells.ContainsKey(position);

            if (isAlive && count is 2 or 3)
                newActiveCells[position] = true;
            else if (!isAlive && count == 3) newActiveCells[position] = true;
        }

        _activeCells = newActiveCells;
    }

    private static IEnumerable<Position> GetNeighborOffsets()
    {
        yield return new Position(-1, -1);
        yield return new Position(0, -1);
        yield return new Position(1, -1);
        yield return new Position(-1, 0);
        yield return new Position(1, 0);
        yield return new Position(-1, 1);
        yield return new Position(0, 1);
        yield return new Position(1, 1);
    }
}
