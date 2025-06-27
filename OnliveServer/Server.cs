using Microsoft.Extensions.Logging;
using OnliveConstants.Communication;
using OnliveConstants.Requests;

namespace OnliveServer;

public class Server
{
    private const string Ip = "127.0.0.1";
    private const int Port = 8001;

    private readonly SocketServer _socket = new(Ip, Port);
    private readonly GameBoard _board = new();
    private readonly ILogger<Server> _logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<Server>();

    public Server()
    {
        _socket.RequestReceived += OnRequestReceived;

        _ = UpdateGameBoardAsync();
    }

    public async Task StartAsync()
    {
        _logger.LogInformation($"Server started on {Ip}:{Port}");

        _ = SendGameBoardUpdatesAsync();
        await _socket.StartAsync();
    }

    private async Task UpdateGameBoardAsync()
    {
        while (true)
        {
            await Task.Delay(300);

            _board.Update();
        }
    }

    private void HandleSwitchCellsRequest(SwitchCellsRequest request)
    {
        foreach (var switchedCell in request.SwitchedCells)
        {
            _board.SwitchValue(switchedCell);
        }

        _logger.LogInformation($"Switched {request.SwitchedCells.Count()} cells");
    }

    private void OnRequestReceived(object? sender, RequestReceivedEventArgs eventArgs)
    {
        var action = RequestDecoder.DecodeRequestAction(eventArgs.Request);

        _logger.LogDebug($"Received request <{action}> => {eventArgs.Request}");

        switch (action)
        {
            case RequestAction.SwitchCells:
                var switchCellsRequest = RequestDecoder.DecodeSwitchCellsRequest(eventArgs.Request);

                if (switchCellsRequest.HasValue) HandleSwitchCellsRequest(switchCellsRequest.Value);
                break;

            case RequestAction.SendCurrentPosition:
                var sendCurrentPositionRequest = RequestDecoder.DecodeSendCurrentPositionRequest(eventArgs.Request);

                if (sendCurrentPositionRequest.HasValue)
                    eventArgs.Client.CurrentPosition = sendCurrentPositionRequest.Value.CurrentPosition;
                break;

            default:
                _logger.LogWarning($"Could not process request <{action}> => {eventArgs.Request}");
                return;
        }
    }

    private async Task SendGameBoardUpdatesAsync()
    {
        while (true)
        {
            await Task.Delay(300);

            await _socket.SendToAllClientsAsync(client =>
                new SendBoardRequest { ActiveCells = _board.ActiveCells }.ToRequestString());
        }
    }
}
