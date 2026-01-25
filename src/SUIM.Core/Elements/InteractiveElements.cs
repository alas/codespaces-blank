namespace SUIM.Core.Elements;

using System.Collections.Generic;

/// <summary>
/// Button interactive element.
/// </summary>
public class ButtonElement : UIElement
{
    public string? Text { get; set; }

    public ButtonElement() : base("button") { }
}

/// <summary>
/// Text input element.
/// </summary>
public class InputElement : UIElement
{
    public string? Value { get; set; }
    public string? Placeholder { get; set; }
    public string? InputType { get; set; } = "text";

    public InputElement() : base("input") { }
}

/// <summary>
/// Multi-line text input element.
/// </summary>
public class TextAreaElement : UIElement
{
    public string? Value { get; set; }
    public string? Placeholder { get; set; }
    public int Rows { get; set; } = 4;
    public int Columns { get; set; } = 50;

    public TextAreaElement() : base("textarea") { }
}

/// <summary>
/// Select dropdown element.
/// </summary>
public class SelectElement : UIElement
{
    public string? SelectedValue { get; set; }
    public List<string> Options { get; set; } = new();

    public SelectElement() : base("select") { }
}

/// <summary>
/// Progress bar element.
/// </summary>
public class ProgressElement : UIElement
{
    public float Value { get; set; } = 0f;
    public float Maximum { get; set; } = 100f;

    public ProgressElement() : base("progress") { }
}
