using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using Onlive.Utils;
using OnliveConstants.Communication;
using OnliveConstants.Requests;

namespace Onlive;

public class GameBoardRequestReceivedEventArgs(SendBoardRequest request) : EventArgs
{
    public SendBoardRequest Request { get; } = request;
}

public class SocketClient(string serverIp, int serverPort)
{
    private readonly TcpClient _client = new();

    private readonly ILogger<SocketClient> _logger = Logging.GetLogger<SocketClient>();
    private NetworkStream _stream = null!;
    public event EventHandler<GameBoardRequestReceivedEventArgs> GameBoardRequestReceived = null!;

    public async Task StartAsync()
    {
        _client.NoDelay = true;

        _logger.LogDebug($"Connecting to server ({serverIp}:{serverPort})...");

        await _client.ConnectAsync(serverIp, serverPort);
        _stream = _client.GetStream();
        _ = ReadFromServerAsync();

        _logger.LogDebug("Connected to server !");
    }

    public async Task SendSwitchCellsRequest(SwitchCellsRequest request)
    {
        _logger.LogDebug($"Sending SwitchCells request for {request.SwitchedCells.Count()} cells");
        await WriteToServerAsync(request.ToRequestString());
    }

    public async Task SendCurrentPositionAsync(SendCurrentPositionRequest request)
    {
        _logger.LogDebug($"Sending current position ({request.CurrentPosition})");
        await WriteToServerAsync(request.ToRequestString());
    }

    private async Task WriteToServerAsync(string content)
    {
        var writer = new StreamWriter(_stream);

        _logger.LogTrace($"<<< {content}");

        await writer.WriteLineAsync(content);
        await writer.FlushAsync();
    }

    private async Task ReadFromServerAsync()
    {
        var reader = new StreamReader(_stream);

        while (true)
        {
            var request = await reader.ReadLineAsync();
            _logger.LogTrace($">>> \n{request}");

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

                if (gameBoardRequest.HasValue)
                    GameBoardRequestReceived.Invoke(this,
                        new GameBoardRequestReceivedEventArgs(gameBoardRequest.Value));
                break;

            default:
                _logger.LogWarning($"Could not process request <{action}> => {request}");
                break;
        }
    }
}