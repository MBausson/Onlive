using System.Net;
using System.Net.Sockets;
using System.Text;
using Microsoft.Extensions.Logging;
using Onlive.Utils;
using OnliveConstants;
using OnliveConstants.Communication;
using OnliveConstants.Requests;

namespace Onlive;

public class GameBoardRequestReceivedEventArgs(SendBoardRequest request) : EventArgs
{
    public SendBoardRequest Request { get; } = request;
}

public class SocketClient(string serverIp, int serverPort)
{
    private readonly UdpClient _client = new();
    private readonly IPEndPoint _serverEndPoint = new(IPAddress.Parse(serverIp), serverPort);
    private readonly ILogger<SocketClient> _logger = Logging.GetLogger<SocketClient>();
    public event EventHandler<GameBoardRequestReceivedEventArgs> GameBoardRequestReceived = null!;

    public async Task StartAsync()
    {
        _client.Client.Bind(new IPEndPoint(IPAddress.Any, 0));
        _logger.LogInformation($"Client started (expecting server on {serverIp}:{serverPort})");

        await SendCurrentPositionAsync(new(){ CurrentPosition = Position.Zero});

        _ = ReadFromServerAsync();
        _ = SendPingsAsync();
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
        var data = Encoding.UTF8.GetBytes(content);
        await _client.SendAsync(data, data.Length, _serverEndPoint);

        _logger.LogTrace($"<<< {content}");
    }

    private async Task SendPingsAsync()
    {
        while (true)
        {
            await Task.Delay(PingRequest.ClientInterval);
            await WriteToServerAsync(new PingRequest().ToRequestString());
        }
    }

    private async Task ReadFromServerAsync()
    {
        while (true)
        {
            var request = await _client.ReceiveAsync();

            if (!Equals(request.RemoteEndPoint, _serverEndPoint))
            {
                _logger.LogCritical($"Received an unexpected request from {request.RemoteEndPoint}");
            }

            var content = Encoding.UTF8.GetString(request.Buffer);
            _logger.LogTrace($">>> \n{content}");

            if (string.IsNullOrWhiteSpace(content)) continue;

            ProcessRequest(content);
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
