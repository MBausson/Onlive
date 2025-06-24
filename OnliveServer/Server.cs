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
    }

    public async Task StartAsync()
    {
        _logger.LogInformation($"Server started on {Ip}:{Port}");

        await _socket.StartAsync();
    }

    private void HandleSwitchCellRequest(SwitchCellRequest request)
    {
        _board.SwitchValue(request.SwitchedCell);

        _logger.LogInformation($"Switched cell at {request.SwitchedCell}");
    }

    private void OnRequestReceived(object? sender, RequestReceivedEventArgs eventArgs)
    {
        var action = RequestDecoder.DecodeRequestAction(eventArgs.Request);

        _logger.LogDebug($"Received request <{action}> => {eventArgs.Request}");

        switch (action)
        {
            case RequestAction.SwitchCell:
                var switchCellRequest = RequestDecoder.DecodeSwitchCellRequest(eventArgs.Request);

                if (switchCellRequest.HasValue) HandleSwitchCellRequest(switchCellRequest.Value);
                break;

            default:
                _logger.LogWarning($"Could not process request <{action}> => {eventArgs.Request}");
                return;
        }
    }
}
