namespace SUIM.Core.Parser;

using System;
using System.Collections.Generic;
using SUIM.Core.Elements;

/// <summary>
/// Parser for SUIM markup content.
/// </summary>
public class SUIMParser
{
    private readonly SUIMLexer _lexer = new();
    private List<Token> _tokens = new();
    private int _current = 0;
    private readonly List<ParseError> _errors = new();
    private readonly List<string> _warnings = new();

    /// <summary>
    /// Parses SUIM markup content.
    /// </summary>
    public ParseResult Parse(string content)
    {
        _tokens = _lexer.Tokenize(content);
        _current = 0;
        _errors.Clear();
        _warnings.Clear();

        try
        {
            var root = ParseElement();
            
            return new ParseResult
            {
                Success = _errors.Count == 0,
                Root = root,
                Errors = _errors,
                Warnings = _warnings
            };
        }
        catch (Exception ex)
        {
            _errors.Add(new ParseError($"Fatal parse error: {ex.Message}"));
            return new ParseResult
            {
                Success = false,
                Errors = _errors,
                Warnings = _warnings
            };
        }
    }

    private UIElement? ParseElement()
    {
        if (IsAtEnd())
            return null;

        var token = Peek();

        if (token.Type == TokenType.Tag)
        {
            return ParseTag();
        }
        else if (token.Type == TokenType.ControlIf)
        {
            return ParseIfStatement();
        }
        else if (token.Type == TokenType.ControlForEach)
        {
            return ParseForEach();
        }
        else if (token.Type == TokenType.Text)
        {
            Advance();
            // Create a text element (placeholder)
            return new UIElement("text") { Attributes = { { "content", token.Value } } };
        }

        return null;
    }

    private UIElement ParseTag()
    {
        var token = Advance();
        var tagContent = token.Value;
        
        // Simple tag parsing (real implementation would be more complex)
        var parts = tagContent.Split(' ');
        var tagName = parts[0];

        var element = CreateElementByTag(tagName);
        element.TagName = tagName;

        // Parse attributes (simplified)
        for (int i = 1; i < parts.Length; i++)
        {
            if (parts[i].Contains("="))
            {
                var attrParts = parts[i].Split('=');
                element.Attributes[attrParts[0]] = attrParts.Length > 1 ? attrParts[1] : "";
            }
        }

        // Parse children until closing tag
        while (!IsAtEnd() && Peek().Value != $"/{tagName}")
        {
            var child = ParseElement();
            if (child != null)
                element.AddChild(child);
        }

        // Consume closing tag
        if (!IsAtEnd() && Peek().Type == TokenType.Tag)
            Advance();

        return element;
    }

    private UIElement ParseIfStatement()
    {
        Advance(); // consume 'if'
        
        // For now, return a container element
        // Real implementation would evaluate conditions
        var container = new UIElement("fragment");
        
        while (!IsAtEnd() && Peek().Value != "else" && Peek().Value != "@else")
        {
            var child = ParseElement();
            if (child != null)
                container.AddChild(child);
        }

        return container;
    }

    private UIElement ParseForEach()
    {
        Advance(); // consume 'foreach'
        
        // For now, return a container element
        // Real implementation would iterate
        var container = new UIElement("fragment");

        while (!IsAtEnd())
        {
            var child = ParseElement();
            if (child != null)
                container.AddChild(child);
        }

        return container;
    }

    private UIElement CreateElementByTag(string tagName) => tagName.ToLower() switch
    {
        "div" => new UIElement("div"),
        "vstack" or "vbox" => new VStackElement(),
        "hstack" or "hbox" => new HStackElement(),
        "grid" => new GridElement(),
        "dock" => new DockElement(),
        "overlay" => new OverlayElement(),
        "scroll" => new ScrollElement(),
        "button" => new ButtonElement(),
        "input" => new InputElement(),
        "textarea" => new TextAreaElement(),
        "select" => new SelectElement(),
        "progress" => new ProgressElement(),
        "h1" => new H1Element(),
        "h2" => new H2Element(),
        "h3" => new H3Element(),
        "h4" => new H4Element(),
        "h5" => new H5Element(),
        "h6" => new H6Element(),
        "p" => new PElement(),
        "label" => new LabelElement(),
        _ => new UIElement(tagName)
    };

    private Token Peek() => IsAtEnd() ? _tokens[^1] : _tokens[_current];
    private Token Advance() => _tokens[_current++];
    private bool IsAtEnd() => _current >= _tokens.Count || Peek().Type == TokenType.EOF;
}
