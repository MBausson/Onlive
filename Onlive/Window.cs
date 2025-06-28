using Microsoft.Extensions.Logging;
using Onlive.Graphics;
using Onlive.Utils;
using OnliveConstants;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Onlive;

public class Window
{
    private static readonly Vector2u DefaultSize = new(700, 700);
    private readonly RenderWindow _window = new(new VideoMode(DefaultSize.X, DefaultSize.Y), "OnLive - Online game of life");
    private const float CellSize = 16.0f;
    private readonly Renderables _renderables = new(CellSize);
    private readonly float _cameraSpeed = 4f;
    private bool _isStashingCells;

    private readonly Game _game;
    private readonly View _view = new(new FloatRect(0, 0, DefaultSize.X, DefaultSize.Y));
    private readonly ILogger<Window> _logger = Logging.GetLogger<Window>();

    public Window(string serverIp, int serverPort)
    {
        _game = new(serverIp, serverPort, () => _view.Center.ToPosition());
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
            RenderHoveredCell();

            _window.Display();
        }
    }

    private void RenderBoard()
    {
        var activeCells = _game.ActiveCells;
        var temporaryCells = _game.StashedCellsPositions;

        foreach (var activeCell in activeCells)
        {
            _window.Draw(_renderables.GetActiveCellShape(new Vector2f(activeCell.X, activeCell.Y)));
        }

        foreach (var temporaryCell in temporaryCells)
        {
            _window.Draw(_renderables.GetTemporaryCellShape(new Vector2f(temporaryCell.X, temporaryCell.Y)));
        }
    }

    private void RenderHoveredCell()
    {
        var worldPosition = _window.MapPixelToCoords(Mouse.GetPosition(_window));
        var vectorPosition = new Vector2f(
            (int)Math.Round(worldPosition.X / CellSize, MidpointRounding.ToNegativeInfinity),
            (int)Math.Round(worldPosition.Y / CellSize, MidpointRounding.ToNegativeInfinity));

        _window.Draw(_renderables.GetHoveredCellShape(vectorPosition));
    }

    private void OnMousePressed(object? sender, MouseButtonEventArgs e)
    {
        var worldPosition = _window.MapPixelToCoords(new Vector2i(e.X, e.Y));
        var vectorPosition = new Vector2i(
            (int)Math.Round(worldPosition.X / CellSize, MidpointRounding.ToNegativeInfinity),
            (int)Math.Round(worldPosition.Y / CellSize, MidpointRounding.ToNegativeInfinity));
        var position = vectorPosition.ToPosition();

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

        _view.Zoom(1 + e.Delta * -0.1f);
        _window.SetView(_view);
    }

    private void OnKeyPressed(object? sender, KeyEventArgs e)
    {
        switch (e.Code)
        {
            case Keyboard.Key.Left:
                _view.Move(new Vector2f(-_cameraSpeed, 0));
                break;

            case Keyboard.Key.Right:
                _view.Move(new Vector2f(_cameraSpeed, 0));
                break;

            case Keyboard.Key.Up:
                _view.Move(new Vector2f(0, -_cameraSpeed));
                break;

            case Keyboard.Key.Down:
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
