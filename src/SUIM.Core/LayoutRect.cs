namespace SUIM.Core;

/// <summary>
/// Represents the calculated layout rectangle for a UI element.
/// </summary>
public struct LayoutRect
{
    public float X { get; set; }
    public float Y { get; set; }
    public float Width { get; set; }
    public float Height { get; set; }

    public LayoutRect(float x = 0, float y = 0, float width = 0, float height = 0)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
    }

    public float Right => X + Width;
    public float Bottom => Y + Height;

    public bool Contains(float x, float y)
    {
        return x >= X && x < Right && y >= Y && y < Bottom;
    }

    public override string ToString() => $"({X}, {Y}, {Width}, {Height})";
}
