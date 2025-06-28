using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Onlive.Utils;
using OnliveConstants;
using OnliveConstants.Requests;

namespace Onlive;

public class Game(string serverIp, int serverPort, Func<Position> currentPositionFunc)
{
    public IReadOnlyCollection<Position> ActiveCells { get; private set; } = [];
    public IEnumerable<Position> StashedCellsPositions => _stashedCellsPositions.Keys;

    private readonly ConcurrentDictionary<Position, bool> _stashedCellsPositions = [];
    private readonly GameClient _client = new(serverIp, serverPort);
    private readonly ILogger<Game> _logger = Logging.GetLogger<Game>();

    public async Task Connect()
    {
        await _client.StartAsync();

        _client.GameBoardRequestReceived += OnGameBoardReceived;
        _ = SendCurrentPositionPeriodicallyAsync();
    }

    public async Task SwitchCellAsync(Position position)
    {
        await _client.SendSwitchCellsRequest(new SwitchCellsRequest
        {
            SwitchedCells = [position]
        });
    }

    public async Task SwitchStashedCellsAsync()
    {
        if (_stashedCellsPositions.Count == 0) return;

        await _client.SendSwitchCellsRequest(new SwitchCellsRequest
        {
            SwitchedCells = _stashedCellsPositions.Where(kvp => kvp.Value).Select(kvp => kvp.Key)
        });

        _stashedCellsPositions.Clear();
    }

    public void StashCell(Position position)
    {
        if (_stashedCellsPositions.ContainsKey(position))
        {
            _stashedCellsPositions.Remove(position, out var _);
        }
        else
        {
            //  Stashing a new value means flipping its current value
            _stashedCellsPositions[position] = !ActiveCells.Contains(position);
        }
    }

    private void OnGameBoardReceived(object? sender, GameBoardRequestReceivedEventArgs e)
    {
        _logger.LogTrace("Received GameBoard update");

        ActiveCells = e.Request.ActiveCells.ToArray();
    }

    private async Task SendCurrentPositionPeriodicallyAsync()
    {
        Position? lastPosition = null;

        while (true)
        {
            await Task.Delay(1000);

            var currentPosition = currentPositionFunc();

            //  Avoid sending the same position over and over
            if (lastPosition.HasValue && lastPosition.Value == currentPosition) continue;
            lastPosition = currentPosition;

            var request = new SendCurrentPositionRequest{ CurrentPosition = currentPositionFunc() };
            await _client.SendCurrentPositionAsync(request);
        }
    }
}
