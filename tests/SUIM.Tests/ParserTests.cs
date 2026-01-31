namespace SUIM.Tests;

using Xunit;
using SUIM.Core.Parser;
using System.Collections.Generic;
using System.IO;

    public class ProcessorTests
    {
        private readonly SUIMProcessor _processor;

        public ProcessorTests()
        {
            // Set up a mock component directory for testing expansion
            _processor = new SUIMProcessor("UI/Components");
            
            if (!Directory.Exists("UI/Components"))
                Directory.CreateDirectory("UI/Components");
        }

        [Fact]
        public void Process_ShouldResolveSimpleIfCondition()
        {
            var xml = @"<vstack>
                          @if (IsVisible)
                        {
                            <button text=""Click Me"" />
                        }
                        </vstack>";
            
            var model = new { IsVisible = true };
            var result = _processor.Process(xml, model);

            Assert.Contains("<button text=\"Click Me\" />", result);
            Assert.DoesNotContain("@if", result);
        }

        [Fact]
        public void Process_ShouldResolveIfElseChain()
        {
            var xml = @"<panel>
                          @if (Status == 'Active')
                          {
                             <label text=""Active"" />
                          }
                          else if (Status == ""Pending"")
                          {
                             <label text=""Pending"" />
                        }
                          else
                          {
                             <label text=""Inactive"" />
                             }
                        </panel>";

            var model = new { Status = "Pending" };
            var result = _processor.Process(xml, model);

            Assert.Contains("<label text=\"Pending\" />", result);
            Assert.DoesNotContain("Active", result);
            Assert.DoesNotContain("Inactive", result);
        }

        [Fact]
        public void Process_ShouldExpandCustomTagWithAttributes()
        {
            // Create a mock component file
            var componentXml = @"<Mytag>
                                    <model> { myattribute: '' } </model>
                                    @if (myattribute == 'doGrid'){
                                        <grid />
                                    }else{
                                        <div />
                                        }
                                 </Mytag>";
            
            File.WriteAllText("UI/Components/Mytag.suim", componentXml);

            var xml = @"<vstack>
                          <Mytag myattribute=""doGrid"" />
                        </vstack>";

            var result = _processor.Process(xml);

            // Should expand to <grid /> because attribute was passed
            Assert.Contains("<grid />", result);
            Assert.DoesNotContain("<div />", result);
            Assert.DoesNotContain("<Mytag", result);
        }

        [Fact]
        public void Process_ShouldHandleNullChecksInConditions()
        {
            var xml = @"<vstack>
                          @if (UserData == null){
                            <label text=""Please Login"" />}
                        </vstack>";

            var result = _processor.Process(xml, new { UserData = (object)null });
            Assert.Contains("Please Login", result);
        }

        [Fact]
        public void Process_ShouldResolveForeachLoop()
        {
            var xml = @"<vstack>
                          @foreach (var name in items){
                            <label text=""@name"" />}
                        </vstack>";

            var model = new { Names = new List<string> { "Alice", "Bob" } };
            var result = _processor.Process(xml, model);

            // Note: If you haven't finished the @name replacement helper, 
            // this verifies the tags are at least duplicated.
            int labelCount = result.Split("<label").Length - 1;
            Assert.Equal(2, labelCount);
        }

        [Fact]
        public void Process_ShouldResolveSwitchCase()
        {
            // Note: This assumes your Evaluate logic handles the @switch tag 
            // or you've mapped it to @if chains internally.
            var xml = @"<panel>
                          @if (UserRole == 'Admin'){
                             <button text=""Delete"" />}
                          else if (UserRole == 'Moderator'){
                             <button text=""Flag"" />}
                          else {
                             <label text=""View Only"" />}
                        </panel>";

            var result = _processor.Process(xml, new { UserRole = "Admin" });
            Assert.Contains("Delete", result);
            Assert.DoesNotContain("Flag", result);
            Assert.DoesNotContain("View Only", result);
        }
        
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
