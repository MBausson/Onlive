using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using OnliveConstants.Communication;
using OnliveConstants.Requests;

namespace Onlive;

public class GameBoardRequestReceivedEventArgs(SendBoardRequest request) : EventArgs
{
    public SendBoardRequest Request { get; } = request;
}

public class GameClient(string serverIp, int serverPort)
{
    public event EventHandler<GameBoardRequestReceivedEventArgs> GameBoardRequestReceived;

    private readonly ILogger<GameClient> _logger = Helpers.GetLogger<GameClient>();

    private TcpClient _client = new();

    public async Task StartAsync()
    {
        await _client.ConnectAsync(serverIp, serverPort);

        _ = ReadFromServerAsync();

        _logger.LogInformation($"Connected to server ! ({serverIp}:{serverPort})");
    }

    public async Task SendSwitchCellRequest(SwitchCellRequest request)
    {
        var stream = _client.GetStream();
        var writer = new StreamWriter(stream);

        _logger.LogDebug($"Sending SwitchCell request at {request.SwitchedCell}");

        await writer.WriteLineAsync(request.ToRequestString());
        await writer.FlushAsync();
    }

    private async Task ReadFromServerAsync()
    {
        var stream = _client.GetStream();
        var reader = new StreamReader(stream);

        while (true)
        {
            var request = await reader.ReadLineAsync();

            if (request is null) continue;

            ProcessRequest(request);
        }
    }

    private void ProcessRequest(string request)
    {
        var action = RequestDecoder.DecodeRequestAction(request);

        switch (action)
        {
            case RequestAction.SendBoard:
                var gameBoardRequest = RequestDecoder.DecodeSendBoardRequest(request);

                if (gameBoardRequest.HasValue) GameBoardRequestReceived.Invoke(this, new (gameBoardRequest.Value));
                break;

            default:
                _logger.LogWarning($"Could not process request <{action}> => {request}");
                break;
        }
    }
}
