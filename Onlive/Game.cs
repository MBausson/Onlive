using OnliveConstants;
using OnliveConstants.Requests;

namespace Onlive;

public class Game
{
    public IReadOnlyCollection<Position> ActiveCells { get; private set; } = [];
    private GameClient _client = new();

    public async Task Connect()
    {
        await _client.StartAsync();

        _client.GameBoardRequestReceived += OnGameBoardReceived;
    }

    public async Task SwitchCellAsync(Position position)
    {
        await _client.SendSwitchCellRequest(new SwitchCellRequest
        {
            SwitchedCell = position
        });
    }

    private void OnGameBoardReceived(object? sender, GameBoardRequestReceivedEventArgs e)
    {
        ActiveCells = e.Request.ActiveCells.ToArray();
    }
}
