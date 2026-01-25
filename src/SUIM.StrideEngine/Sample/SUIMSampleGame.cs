namespace SUIM.StrideEngine.Sample;

using Stride.Engine;
using Stride.Games;
using SUIM.Core;
using SUIM.Core.Elements;

/// <summary>
/// Sample game demonstrating SUIM UI rendering in Stride.
/// </summary>
public class SUIMSampleGame : Game
{
    private SUIMGameComponent? _suimComponent;

    public SUIMSampleGame() : base()
    {
        Window.Title = "SUIM UI Sample - Stride Engine";
    }

    protected override async Task LoadContent()
    {
        await base.LoadContent();

        // Create and add SUIM component
        _suimComponent = new SUIMGameComponent(this);
        GameSystems.Add(_suimComponent);

        // Create a sample UI tree
        CreateSampleUI();
    }

    private void CreateSampleUI()
    {
        // Create root vertical stack
        var root = new VStackElement
        {
            Id = "root",
            BoxModel = new BoxModel
            {
                Width = SizingValue.PercentageWidth(100),
                Height = SizingValue.PercentageHeight(100),
                Padding = new Spacing(16)
            },
            BackgroundColor = "#2C3E50",
            Spacing = 16
        };

        // Add title
        var title = new H1Element
        {
            Text = "SUIM UI Sample",
            BoxModel = new BoxModel
            {
                Margin = new Spacing(0, 0, 8, 0)
            }
        };
        root.AddChild(title);

        // Add horizontal stack with buttons
        var buttonRow = new HStackElement
        {
            Id = "button-row",
            Spacing = 8
        };

        var button1 = new ButtonElement
        {
            Id = "btn-1",
            Text = "Button 1",
            BoxModel = new BoxModel
            {
                Width = SizingValue.Pixels(100),
                Height = SizingValue.Pixels(40)
            }
        };
        button1.On("click", (el) => System.Console.WriteLine("Button 1 clicked"));
        buttonRow.AddChild(button1);

        var button2 = new ButtonElement
        {
            Id = "btn-2",
            Text = "Button 2",
            BoxModel = new BoxModel
            {
                Width = SizingValue.Pixels(100),
                Height = SizingValue.Pixels(40)
            }
        };
        button2.On("click", (el) => System.Console.WriteLine("Button 2 clicked"));
        buttonRow.AddChild(button2);

        root.AddChild(buttonRow);

        // Add input field
        var inputField = new InputElement
        {
            Id = "text-input",
            Placeholder = "Enter text...",
            BoxModel = new BoxModel
            {
                Width = SizingValue.PercentageWidth(100),
                Height = SizingValue.Pixels(36)
            }
        };
        root.AddChild(inputField);

        // Add paragraph
        var paragraph = new PElement
        {
            Text = "This is a sample SUIM UI rendered in the Stride game engine.",
            BoxModel = new BoxModel
            {
                Margin = new Spacing(8, 0)
            }
        };
        root.AddChild(paragraph);

        // Load the UI
        if (_suimComponent != null)
        {
            _suimComponent.LoadFromMarkup(BuildSUIMarkup());
        }
    }

    private string BuildSUIMarkup()
    {
        return @"
<vstack spacing='16' width='100pw' height='100ph' padding='16'>
  <h1>SUIM UI Sample</h1>
  <hstack spacing='8'>
    <button id='btn-1' width='100' height='40'>Button 1</button>
    <button id='btn-2' width='100' height='40'>Button 2</button>
  </hstack>
  <input id='text-input' width='100pw' height='36' placeholder='Enter text...' />
  <p>This is a sample SUIM UI rendered in the Stride game engine.</p>
</vstack>
";
    }

    protected override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        // Handle input
        var input = Input;
        if (input.IsKeyPressed(Stride.Input.Keys.Escape))
        {
            Exit();
        }
    }
}
