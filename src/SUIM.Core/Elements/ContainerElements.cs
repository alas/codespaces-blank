namespace SUIM.Core.Elements;

/// <summary>
/// Container element using absolute positioning.
/// </summary>
public class DivElement : UIElement
{
    public DivElement() : base("div") { }
}

/// <summary>
/// Sequential container element.
/// </summary>
public class StackElement : UIElement
{
    public StackElement() : base("stack") { }
}

/// <summary>
/// Vertical stack container element.
/// </summary>
public class VStackElement : UIElement
{
    public float Spacing { get; set; } = 8f;

    public VStackElement() : base("vstack") { }
}

/// <summary>
/// Horizontal stack container element.
/// </summary>
public class HStackElement : UIElement
{
    public float Spacing { get; set; } = 8f;

    public HStackElement() : base("hstack") { }
}

/// <summary>
/// Grid layout container element.
/// </summary>
public class GridElement : UIElement
{
    public int Columns { get; set; } = 1;
    public int Rows { get; set; } = 1;
    public float Spacing { get; set; } = 8f;

    public GridElement() : base("grid") { }
}

/// <summary>
/// Dock panel container element that pins children to edges.
/// </summary>
public class DockElement : UIElement
{
    public bool LastChildFill { get; set; } = true;

    public DockElement() : base("dock") { }
}

/// <summary>
/// Overlay element that always renders above all z-index layers.
/// </summary>
public class OverlayElement : UIElement
{
    public OverlayElement() : base("overlay") { }
}

/// <summary>
/// Scrollable container with viewport.
/// </summary>
public class ScrollElement : UIElement
{
    public enum ScrollDirection { Vertical, Horizontal, Both }

    public ScrollDirection Direction { get; set; } = ScrollDirection.Vertical;
    public float ScrollX { get; set; }
    public float ScrollY { get; set; }

    public ScrollElement() : base("scroll") { }
}
