namespace SUIM.Core.Elements;

/// <summary>
/// Heading 1 element.
/// </summary>
public class H1Element : UIElement
{
    public string? Text { get; set; }

    public H1Element() : base("h1") { }
}

/// <summary>
/// Heading 2 element.
/// </summary>
public class H2Element : UIElement
{
    public string? Text { get; set; }

    public H2Element() : base("h2") { }
}

/// <summary>
/// Heading 3 element.
/// </summary>
public class H3Element : UIElement
{
    public string? Text { get; set; }

    public H3Element() : base("h3") { }
}

/// <summary>
/// Heading 4 element.
/// </summary>
public class H4Element : UIElement
{
    public string? Text { get; set; }

    public H4Element() : base("h4") { }
}

/// <summary>
/// Heading 5 element.
/// </summary>
public class H5Element : UIElement
{
    public string? Text { get; set; }

    public H5Element() : base("h5") { }
}

/// <summary>
/// Heading 6 element.
/// </summary>
public class H6Element : UIElement
{
    public string? Text { get; set; }

    public H6Element() : base("h6") { }
}

/// <summary>
/// Paragraph text element.
/// </summary>
public class PElement : UIElement
{
    public string? Text { get; set; }

    public PElement() : base("p") { }
}

/// <summary>
/// Label text element.
/// </summary>
public class LabelElement : UIElement
{
    public string? Text { get; set; }
    public string? For { get; set; }

    public LabelElement() : base("label") { }
}
