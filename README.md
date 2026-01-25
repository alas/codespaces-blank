# SUIM C# Implementation

A comprehensive C# implementation of the SUIM (Simple UI Markup) specification for Stride game engine and desktop applications.

## Project Structure

### src/SUIM.Core
Core library implementing the SUIM specification:
- **SizingValue.cs** - Sizing unit types and conversions (px, rem, pw, ph, *, auto)
- **BoxModel.cs** - Box model with margins, padding, borders, and sizing=
- **LayoutRect.cs** - Calculated layout rectangles for elements
- **UIElement.cs** - Base class for all UI elements
- **ThemeMetricTable.cs** - Global theme metrics and sizing resolution
- **Elements/** - Specific element implementations:
  - `ContainerElements.cs` - div, stack, vstack, hstack, grid, dock, overlay, scroll
  - `InteractiveElements.cs` - button, input, textarea, select, progress
  - `TextElements.cs` - h1-h6, p, label
- **Parser/** - SUIM parsing and layout:
  - `SUIMLexer.cs` - Tokenizes SUIM markup
  - `SUIMParser.cs` - Parses tokens into element trees
  - `ParseResult.cs` - Parser output and error handling
- **Layout/** - Layout engine:
  - `LayoutEngine.cs` - O(N) single-pass layout algorithm

### src/SUIM.StrideEngine
Integration with Stride game engine:
- **SUIMGameComponent.cs** - Game component for rendering SUIM UI
- **Sample/SUIMSampleGame.cs** - Example game demonstrating SUIM usage
- **Sample/Program.cs** - Entry point for sample application

### tests/SUIM.Tests
Comprehensive test suite using xUnit:
- **CoreTests.cs** - Tests for core data structures and layout
- **ParserTests.cs** - Tests for lexer and parser

## Building

```bash
# Build the entire solution
dotnet build

# Build specific project
dotnet build src/SUIM.Core/SUIM.Core.csproj
dotnet build src/SUIM.StrideEngine/SUIM.StrideEngine.csproj

# Run tests
dotnet test tests/SUIM.Tests/SUIM.Tests.csproj

# Run the sample game
dotnet run --project src/SUIM.StrideEngine/Sample/Program.cs
```

## Features Implemented

### Layout System
- ✅ Box model (margins, padding, borders)
- ✅ Sizing units (px, rem, pw, ph, *, auto)
- ✅ Single-pass O(N) layout algorithm
- ✅ Container types (vstack, hstack, grid, dock)
- ✅ Absolute positioning (div)
- ✅ Scrollable containers

### Elements
- ✅ Text elements (h1-h6, p, label)
- ✅ Interactive elements (button, input, textarea, select, progress)
- ✅ Container elements (div, stack, grid, dock, overlay)

### Parsing
- ✅ Basic SUIM markup tokenization
- ✅ Element tree construction
- ✅ Control flow structure recognition (@if, @foreach)
- ✅ Error reporting with line/column information

### Stride Integration
- ✅ Game component for rendering
- ✅ Input event handling
- ✅ Element finding by ID
- ✅ Sample game with working UI

## Next Steps / TODO

- [ ] Complete parser implementation with full attribute handling
- [ ] Implement rendering system with Stride graphics
- [ ] Add data binding (@property) support
- [ ] Implement event handler execution (@click, @change, etc.)
- [ ] Add style (@style) and model (@model) parsing
- [ ] Support for custom components
- [ ] Scroll desugaring transformation
- [ ] Star unit distribution algorithm
- [ ] Full control flow evaluation (@if, @switch, @foreach)
- [ ] Z-index global layering system
- [ ] Advanced grid layout with rowspan/colspan
- [ ] Dock panel edge pinning
- [ ] Performance optimizations

## Usage Example

```csharp
// Create UI programmatically
var root = new VStackElement
{
    BoxModel = new BoxModel
    {
        Width = SizingValue.PercentageWidth(100),
        Height = SizingValue.PercentageHeight(100)
    },
    Spacing = 16
};

var button = new ButtonElement
{
    Text = "Click Me",
    BoxModel = new BoxModel { Width = SizingValue.Pixels(100) }
};

button.On("click", (el) => Console.WriteLine("Clicked!"));
root.AddChild(button);

// Or use SUIM markup
var parser = new SUIMParser();
var result = parser.Parse(@"
<vstack spacing='16' width='100pw' height='100ph'>
    <h1>Hello SUIM</h1>
    <button id='btn'>Click Me</button>
</vstack>
");
```
