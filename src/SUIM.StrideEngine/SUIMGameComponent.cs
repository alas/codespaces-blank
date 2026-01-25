namespace SUIM.StrideEngine;

using Stride.Engine;
using Stride.Graphics;
using SUIM.Core;
using SUIM.Core.Layout;
using SUIM.Core.Parser;

/// <summary>
/// Stride game component for rendering SUIM UI.
/// </summary>
public class SUIMGameComponent : GameComponent
{
    private UIElement? _rootElement;
    private LayoutEngine? _layoutEngine;
    private SUIMParser? _parser;
    private GraphicsDevice? _graphicsDevice;

    public SUIMGameComponent(Game game) : base(game)
    {
        _parser = new SUIMParser();
        _layoutEngine = new LayoutEngine();
    }

    public override void Initialize()
    {
        base.Initialize();
        _graphicsDevice = Game.GraphicsDevice;
    }

    /// <summary>
    /// Loads and parses a SUIM markup file.
    /// </summary>
    public void LoadFromMarkup(string suimContent)
    {
        var result = _parser!.Parse(suimContent);

        if (!result.Success)
        {
            foreach (var error in result.Errors)
            {
                System.Console.WriteLine($"Parse Error: {error}");
            }
            return;
        }

        _rootElement = result.Root;

        if (_rootElement != null && _graphicsDevice != null)
        {
            _layoutEngine!.CalculateLayout(_rootElement, 
                _graphicsDevice.Presenter.BackBuffer.Width, 
                _graphicsDevice.Presenter.BackBuffer.Height);
        }
    }

    /// <summary>
    /// Gets the root UI element.
    /// </summary>
    public UIElement? RootElement => _rootElement;

    /// <summary>
    /// Finds a UI element by ID.
    /// </summary>
    public UIElement? FindElementById(string id)
    {
        return _rootElement?.FindById(id);
    }

    public override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);

        if (_rootElement == null)
            return;

        // Render the UI tree
        RenderElement(_rootElement);
    }

    private void RenderElement(UIElement element)
    {
        if (!element.IsVisible)
            return;

        // Draw background if specified
        if (!string.IsNullOrEmpty(element.BackgroundColor))
        {
            // TODO: Draw rectangle with background color
            // This is a placeholder for actual Stride rendering
        }

        // Recursively render children
        foreach (var child in element.Children)
        {
            RenderElement(child);
        }
    }

    /// <summary>
    /// Handles input and triggers events on elements.
    /// </summary>
    public void ProcessInput(float x, float y, string eventName = "click")
    {
        var element = FindElementAtPosition(_rootElement, x, y);
        if (element != null)
        {
            element.Trigger(eventName);
        }
    }

    private UIElement? FindElementAtPosition(UIElement? element, float x, float y)
    {
        if (element == null || !element.IsVisible)
            return null;

        if (!element.Layout.Contains(x, y))
            return null;

        // Check children in reverse order (top to bottom in z-order)
        for (int i = element.Children.Count - 1; i >= 0; i--)
        {
            var child = element.Children[i];
            var found = FindElementAtPosition(child, x, y);
            if (found != null)
                return found;
        }

        return element;
    }
}
