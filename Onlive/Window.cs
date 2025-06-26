using Microsoft.Extensions.Logging;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Onlive;

public class Window
{
    private static readonly Vector2u DefaultSize = new(700, 700);
    private readonly RenderWindow _window = new(new VideoMode(DefaultSize.X, DefaultSize.Y), "OnLive - Online game of life");
    private float _cellSize = 16.0f;
    private readonly float _cameraSpeed = 4f;
    private bool _isStashingCells = false;

    private readonly Game _game;
    private readonly View _view = new(new FloatRect(0, 0, DefaultSize.X, DefaultSize.Y));
    private readonly ILogger<Window> _logger = Helpers.GetLogger<Window>();

    public Window(string serverIp, int serverPort)
    {
        _game = new(serverIp, serverPort);
        _game.Connect().GetAwaiter().GetResult();

        _window.SetView(_view);

        _window.Closed += (_, _) => _window.Close();
        _window.MouseButtonPressed += OnMousePressed;
        _window.MouseWheelScrolled += OnMouseScrolled;
        _window.KeyPressed += OnKeyPressed;
    }

    public void Run()
    {
        while (_window.IsOpen)
        {
            _window.DispatchEvents();
            _window.Clear();

            RenderBoard();

            _window.Display();
        }
    }

    public void RenderBoard()
    {
        var activeCells = _game.ActiveCells;
        var temporaryCells = _game.StashedCellsPositions;

        foreach (var activeCell in activeCells)
        {
            _window.Draw(new RectangleShape
            {
                FillColor = Color.White,
                Size = new Vector2f(_cellSize, _cellSize),
                Position = new Vector2f(activeCell.X * _cellSize, activeCell.Y * _cellSize)
            });
        }

        foreach (var temporaryCell in temporaryCells)
        {
            _window.Draw(new RectangleShape
            {
                FillColor = new Color(180, 180, 180),
                Size = new Vector2f(_cellSize, _cellSize),
                Position = new Vector2f(temporaryCell.X * _cellSize, temporaryCell.Y * _cellSize)
            });
        }
    }

    private void OnMousePressed(object? sender, MouseButtonEventArgs e)
    {
        var worldPosition = _window.MapPixelToCoords(new Vector2i(e.X, e.Y));
        var vectorPosition = new Vector2i(
            (int)Math.Round(worldPosition.X / _cellSize, MidpointRounding.ToNegativeInfinity),
            (int)Math.Round(worldPosition.Y / _cellSize, MidpointRounding.ToNegativeInfinity));
        var position = Helpers.PositionFromVector2(vectorPosition);

        _logger.LogTrace($"Click ! WorldPosition = {vectorPosition}");

        if (_isStashingCells)
        {
            _game.StashCell(position);
        }
        else
        {
            _ = _game.SwitchCellAsync(position);
        }
    }

    private void OnMouseScrolled(object? sender, MouseWheelScrollEventArgs e)
    {
        if (e.Wheel != Mouse.Wheel.VerticalWheel) return;

        _view.Zoom(1 + e.Delta * 0.1f);
        _window.SetView(_view);
    }

    private void OnKeyPressed(object? sender, KeyEventArgs e)
    {
        switch (e.Code)
        {
            case Keyboard.Key.Q:
                _view.Move(new Vector2f(-_cameraSpeed, 0));
                break;

            case Keyboard.Key.D:
                _view.Move(new Vector2f(_cameraSpeed, 0));
                break;

            case Keyboard.Key.Z:
                _view.Move(new Vector2f(0, -_cameraSpeed));
                break;

            case Keyboard.Key.S:
                _view.Move(new Vector2f(0, _cameraSpeed));
                break;

            case Keyboard.Key.LShift:
                _isStashingCells = !_isStashingCells;
                _logger.LogInformation($"Stashing cell is now set to {_isStashingCells}");
                break;

            case Keyboard.Key.Enter:
                _ = _game.SwitchStashedCellsAsync();
                break;
        }

        _window.SetView(_view);
    }
}
