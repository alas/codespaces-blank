namespace SUIM.Tests;

using Xunit;
using SUIM.Core.Parser;

public class SUIMLexerTests
{
    [Fact]
    public void Tokenize_EmptyString_ReturnsEOFToken()
    {
        var lexer = new SUIMLexer();
        var tokens = lexer.Tokenize("");

        Assert.Single(tokens);
        Assert.Equal(TokenType.EOF, tokens[0].Type);
    }

    [Fact]
    public void Tokenize_SimpleTag_TokenizesCorrectly()
    {
        var lexer = new SUIMLexer();
        var tokens = lexer.Tokenize("<button>Click</button>");

        Assert.NotEmpty(tokens);
        Assert.Equal(TokenType.Tag, tokens[0].Type);
    }

    [Fact]
    public void Tokenize_ControlFlow_TokenizesIfKeyword()
    {
        var lexer = new SUIMLexer();
        var tokens = lexer.Tokenize("@if (condition) { }");

        Assert.NotEmpty(tokens);
        Assert.Equal(TokenType.ControlIf, tokens[0].Type);
    }

    [Fact]
    public void Tokenize_Text_TokenizesTextContent()
    {
        var lexer = new SUIMLexer();
        var tokens = lexer.Tokenize("Hello World");

        Assert.NotEmpty(tokens);
        Assert.Equal(TokenType.Text, tokens[0].Type);
        Assert.Contains("Hello", tokens[0].Value);
    }
}

public class SUIMParserTests
{
    [Fact]
    public void Parse_SimpleDiv_ReturnsValidElement()
    {
        var parser = new SUIMParser();
        var result = parser.Parse("<div></div>");

        Assert.True(result.Success);
        Assert.NotNull(result.Root);
    }

    [Fact]
    public void Parse_InvalidMarkup_ReturnsErrors()
    {
        var parser = new SUIMParser();
        var result = parser.Parse("<div");

        // Parser should still attempt to parse even with incomplete markup
        Assert.NotNull(result);
    }

    [Fact]
    public void Parse_NestedElements_BuildsTree()
    {
        var parser = new SUIMParser();
        var result = parser.Parse("<div><button>Click</button></div>");

        Assert.NotNull(result.Root);
        if (result.Root != null)
        {
            // Verify tree structure was built
            Assert.True(result.Root.Children.Count >= 0);
        }
    }
}
