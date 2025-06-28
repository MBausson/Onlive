using SFML.Graphics;
using SFML.System;

namespace Onlive.Graphics;

public class Renderables(float cellSize)
{
    private readonly RectangleShape _activeCellRectangleShape = new()
    {
        FillColor = Color.White,
        Size = new Vector2f(cellSize, cellSize)
    };
    private readonly RectangleShape _hoveredCellRectangleShape = new()
    {
        FillColor = Color.Transparent,
        Size = new Vector2f(cellSize, cellSize),
        OutlineThickness = 1f,
        OutlineColor = new Color(10, 220, 10, 220)
    };
    private readonly RectangleShape _temporaryCellRectangleShape = new()
    {
        FillColor = new Color(180, 180, 180),
        Size = new Vector2f(cellSize, cellSize)
    };

    public RectangleShape GetActiveCellShape(Vector2f position)
    {
        _activeCellRectangleShape.Position = position;
        return _activeCellRectangleShape;
    }

    public RectangleShape GetTemporaryCellShape(Vector2f position)
    {
        _temporaryCellRectangleShape.Position = position;
        return _temporaryCellRectangleShape;
    }

    public RectangleShape GetHoveredCellShape(Vector2f position)
    {
        _hoveredCellRectangleShape.Position = position;
        return _hoveredCellRectangleShape;
    }
}
