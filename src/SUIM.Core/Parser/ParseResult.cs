namespace SUIM.Core.Parser;

using System.Collections.Generic;

/// <summary>
/// Represents the result of parsing a SUIM file.
/// </summary>
public class ParseResult
{
    public bool Success { get; set; }
    public UIElement? Root { get; set; }
    public List<ParseError> Errors { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
}

/// <summary>
/// Represents a parsing error with location information.
/// </summary>
public class ParseError
{
    public string Message { get; set; }
    public int LineNumber { get; set; }
    public int ColumnNumber { get; set; }

    public ParseError(string message, int lineNumber = 0, int columnNumber = 0)
    {
        Message = message;
        LineNumber = lineNumber;
        ColumnNumber = columnNumber;
    }

    public override string ToString() => $"Parse Error at ({LineNumber}, {ColumnNumber}): {Message}";
}
