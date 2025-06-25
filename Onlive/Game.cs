using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using OnliveConstants;
using OnliveConstants.Requests;

namespace Onlive;

public class Game(string serverIp, int serverPort)
{
    public IReadOnlyCollection<Position> ActiveCells { get; private set; } = [];
    public IReadOnlyCollection<Position> TemporaryClickedPositions => _temporaryClickedPositions;

    private readonly ConcurrentBag<Position> _temporaryClickedPositions = [];
    private readonly GameClient _client = new(serverIp, serverPort);
    private readonly ILogger<Game> _logger = Helpers.GetLogger<Game>();

    public async Task Connect()
    {
        await _client.StartAsync();

        _client.GameBoardRequestReceived += OnGameBoardReceived;
    }

    public async Task SwitchCellAsync(Position position)
    {
        _temporaryClickedPositions.Add(position);

        await _client.SendSwitchCellRequest(new SwitchCellRequest
        {
            SwitchedCell = position
        });
    }

    private void OnGameBoardReceived(object? sender, GameBoardRequestReceivedEventArgs e)
    {
        _logger.LogTrace("Received GameBoard update");

        _temporaryClickedPositions.Clear();
        ActiveCells = e.Request.ActiveCells.ToArray();
    }
}
