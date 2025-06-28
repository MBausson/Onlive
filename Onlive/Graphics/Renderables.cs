using SFML.Graphics;
using SFML.System;

namespace Onlive.Graphics;

public class Renderables
{
    private readonly RectangleShape _activeCellRectangleShape;
    private readonly float _cellSize;
    private readonly RectangleShape _hoveredCellRectangleShape;
    private readonly RectangleShape _temporaryCellRectangleShape;

    public Renderables(float cellSize)
    {
        _cellSize = cellSize;

        _activeCellRectangleShape = new RectangleShape
        {
            FillColor = Color.White,
            Size = new Vector2f(_cellSize, _cellSize)
        };

        _temporaryCellRectangleShape = new RectangleShape
        {
            FillColor = new Color(180, 180, 180),
            Size = new Vector2f(_cellSize, _cellSize)
        };

        _hoveredCellRectangleShape = new RectangleShape
        {
            FillColor = Color.Transparent,
            Size = new Vector2f(_cellSize, _cellSize),
            OutlineThickness = 1f,
            OutlineColor = new Color(10, 220, 10, 220)
        };
    }

    public RectangleShape GetActiveCellShape(Vector2f position)
    {
        _activeCellRectangleShape.Position = position * _cellSize;
        return _activeCellRectangleShape;
    }

    public RectangleShape GetTemporaryCellShape(Vector2f position)
    {
        _temporaryCellRectangleShape.Position = position * _cellSize;
        return _temporaryCellRectangleShape;
    }

    public RectangleShape GetHoveredCellShape(Vector2f position)
    {
        _hoveredCellRectangleShape.Position = position * _cellSize;
        return _hoveredCellRectangleShape;
    }
}