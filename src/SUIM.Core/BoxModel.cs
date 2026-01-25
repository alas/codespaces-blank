namespace SUIM.Core;

/// <summary>
/// Represents the box model for layout calculations: margins, padding, borders, and dimensions.
/// </summary>
public class BoxModel
{
    /// <summary>Space outside the border.</summary>
    public Spacing Margin { get; set; } = new();
    
    /// <summary>Space inside the border.</summary>
    public Spacing Padding { get; set; } = new();
    
    /// <summary>Border width.</summary>
    public Spacing Border { get; set; } = new();
    
    /// <summary>Element width.</summary>
    public SizingValue Width { get; set; } = SizingValue.Auto();
    
    /// <summary>Element height.</summary>
    public SizingValue Height { get; set; } = SizingValue.Auto();
    
    /// <summary>Minimum width constraint.</summary>
    public float? MinWidth { get; set; }
    
    /// <summary>Maximum width constraint.</summary>
    public float? MaxWidth { get; set; }
    
    /// <summary>Minimum height constraint.</summary>
    public float? MinHeight { get; set; }
    
    /// <summary>Maximum height constraint.</summary>
    public float? MaxHeight { get; set; }
}

/// <summary>
/// Represents directional spacing (top, right, bottom, left).
/// </summary>
public class Spacing
{
    public float Top { get; set; }
    public float Right { get; set; }
    public float Bottom { get; set; }
    public float Left { get; set; }

    public Spacing(float all = 0)
    {
        Top = Right = Bottom = Left = all;
    }

    public Spacing(float vertical, float horizontal)
    {
        Top = Bottom = vertical;
        Left = Right = horizontal;
    }

    public Spacing(float top, float right, float bottom, float left)
    {
        Top = top;
        Right = right;
        Bottom = bottom;
        Left = left;
    }

    public float Horizontal => Left + Right;
    public float Vertical => Top + Bottom;
}
