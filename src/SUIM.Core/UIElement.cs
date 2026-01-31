namespace SUIM.Core;

using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Base class for all UI elements in the SUIM engine.
/// </summary>
public abstract class UIElement
{
    /// <summary>Unique identifier for this element.</summary>
    public string? Id { get; set; }

    /// <summary>Element tag name (e.g., "div", "button", "vstack").</summary>
    public string TagName { get; set; }

    /// <summary>Parent element.</summary>
    public UIElement? Parent { get; set; }

    /// <summary>Child elements.</summary>
    public List<UIElement> Children { get; set; } = new();

    /// <summary>Box model containing sizing and spacing information.</summary>
    public BoxModel BoxModel { get; set; } = new();

    /// <summary>Calculated layout rectangle.</summary>
    public LayoutRect Layout { get; set; }

    /// <summary>Global z-index for layering.</summary>
    public int ZIndex { get; set; } = 0;

    /// <summary>Background color (CSS-like format).</summary>
    public string? BackgroundColor { get; set; }

    /// <summary>Whether this element is visible.</summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>Whether this element is enabled for interaction.</summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>Custom attributes dictionary.</summary>
    public Dictionary<string, object?> Attributes { get; set; } = new();

    /// <summary>Event handlers by event name.</summary>
    public Dictionary<string, List<Action<UIElement>>> EventHandlers { get; set; } = new();

    public List<PropertyBinding> Bindings { get; } = new();
    public List<SUIMElement> Children { get; } = new();
    
    public void Refresh() {
        foreach(var b in Bindings) b.Apply();
        foreach(var child in Children) child.Refresh();
    }

    protected UIElement(string tagName)
    {
        TagName = tagName;
    }

    /// <summary>
    /// Adds a child element.
    /// </summary>
    public virtual void AddChild(UIElement child)
    {
        child.Parent = this;
        Children.Add(child);
    }

    /// <summary>
    /// Removes a child element.
    /// </summary>
    public virtual void RemoveChild(UIElement child)
    {
        child.Parent = null;
        Children.Remove(child);
    }

    /// <summary>
    /// Registers an event handler.
    /// </summary>
    public virtual void On(string eventName, Action<UIElement> handler)
    {
        if (!EventHandlers.ContainsKey(eventName))
        {
            EventHandlers[eventName] = new();
        }
        EventHandlers[eventName].Add(handler);
    }

    /// <summary>
    /// Triggers an event.
    /// </summary>
    public virtual void Trigger(string eventName)
    {
        if (EventHandlers.TryGetValue(eventName, out var handlers))
        {
            foreach (var handler in handlers)
            {
                handler?.Invoke(this);
            }
        }
    }

    /// <summary>
    /// Finds a descendant element by ID.
    /// </summary>
    public UIElement? FindById(string id)
    {
        if (Id == id)
            return this;

        foreach (var child in Children)
        {
            var found = child.FindById(id);
            if (found != null)
                return found;
        }

        return null;
    }
}
