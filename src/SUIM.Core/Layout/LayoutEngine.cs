namespace SUIM.Core.Layout;

using System;
using System.Collections.Generic;
using System.Linq;
using SUIM.Core.Elements;

/// <summary>
/// Layout engine implementing the O(N) single-pass layout algorithm.
/// </summary>
public class LayoutEngine
{
    private readonly ThemeMetricTable _themeMetrics;

    public LayoutEngine(ThemeMetricTable? metrics = null)
    {
        _themeMetrics = metrics ?? new ThemeMetricTable();
    }

    /// <summary>
    /// Calculates layout for the element tree.
    /// Single-pass O(N) algorithm.
    /// </summary>
    public void CalculateLayout(UIElement root, float parentWidth, float parentHeight)
    {
        var rootLayout = new LayoutRect(0, 0, parentWidth, parentHeight);
        CalculateLayoutRecursive(root, rootLayout, 0, 0);
    }

    private void CalculateLayoutRecursive(UIElement element, LayoutRect parentLayout, float parentX, float parentY)
    {
        if (!element.IsVisible)
            return;

        // Resolve sizing values to pixels
        var width = ResolveDimension(element.BoxModel.Width, parentLayout.Width);
        var height = ResolveDimension(element.BoxModel.Height, parentLayout.Height);

        // Apply constraints
        if (element.BoxModel.MinWidth.HasValue)
            width = Math.Max(width, element.BoxModel.MinWidth.Value);
        if (element.BoxModel.MaxWidth.HasValue)
            width = Math.Min(width, element.BoxModel.MaxWidth.Value);
        if (element.BoxModel.MinHeight.HasValue)
            height = Math.Max(height, element.BoxModel.MinHeight.Value);
        if (element.BoxModel.MaxHeight.HasValue)
            height = Math.Min(height, element.BoxModel.MaxHeight.Value);

        // Calculate position
        float x = parentX + element.BoxModel.Margin.Left;
        float y = parentY + element.BoxModel.Margin.Top;

        if (element.Attributes.TryGetValue("x", out var xVal) && float.TryParse(xVal?.ToString(), out var xPos))
            x = xPos;
        if (element.Attributes.TryGetValue("y", out var yVal) && float.TryParse(yVal?.ToString(), out var yPos))
            y = yPos;

        // Set calculated layout
        element.Layout = new LayoutRect(x, y, width, height);

        // Layout children based on container type
        LayoutChildren(element, width, height);
    }

    private void LayoutChildren(UIElement parent, float parentWidth, float parentHeight)
    {
        if (parent.Children.Count == 0)
            return;

        var containerLayout = parent.Layout;
        float contentX = containerLayout.X + parent.BoxModel.Padding.Left;
        float contentY = containerLayout.Y + parent.BoxModel.Padding.Top;
        float contentWidth = parentWidth - parent.BoxModel.Padding.Horizontal;
        float contentHeight = parentHeight - parent.BoxModel.Padding.Vertical;

        switch (parent.TagName.ToLower())
        {
            case "vstack":
            case "vbox":
                LayoutVStack(parent, contentX, contentY, contentWidth, contentHeight);
                break;
            case "hstack":
            case "hbox":
                LayoutHStack(parent, contentX, contentY, contentWidth, contentHeight);
                break;
            case "grid":
                LayoutGrid(parent, contentX, contentY, contentWidth, contentHeight);
                break;
            case "dock":
                LayoutDock(parent, contentX, contentY, contentWidth, contentHeight);
                break;
            default:
                // Default: layout children sequentially
                float currentY = contentY;
                foreach (var child in parent.Children)
                {
                    var childLayout = new LayoutRect(contentX, currentY, contentWidth, 0);
                    CalculateLayoutRecursive(child, childLayout, contentX, currentY);
                    currentY += child.Layout.Height + child.BoxModel.Margin.Bottom;
                }
                break;
        }
    }

    private void LayoutVStack(UIElement parent, float x, float y, float width, float height)
    {
        var vstack = parent as VStackElement ?? new VStackElement();
        float spacing = vstack.Spacing;
        float currentY = y;

        // Calculate star units
        float totalStarUnits = 0f;
        List<float> starWeights = new();

        foreach (var child in parent.Children)
        {
            var childHeight = child.BoxModel.Height;
            if (childHeight.UnitType == SizingUnitType.Star)
                totalStarUnits += childHeight.Value;
            starWeights.Add(childHeight.UnitType == SizingUnitType.Star ? childHeight.Value : 0);
        }

        float usedHeight = CalculateUsedHeight(parent.Children, width, height);
        float availableHeight = height - usedHeight;

        for (int i = 0; i < parent.Children.Count; i++)
        {
            var child = parent.Children[i];
            float childHeight = ResolveDimension(child.BoxModel.Height, height);

            if (child.BoxModel.Height.UnitType == SizingUnitType.Star && totalStarUnits > 0)
                childHeight = (starWeights[i] / totalStarUnits) * availableHeight;

            var childLayout = new LayoutRect(x, currentY, width, childHeight);
            CalculateLayoutRecursive(child, childLayout, x, currentY);

            currentY += child.Layout.Height + spacing;
        }
    }

    private void LayoutHStack(UIElement parent, float x, float y, float width, float height)
    {
        var hstack = parent as HStackElement ?? new HStackElement();
        float spacing = hstack.Spacing;
        float currentX = x;

        // Similar to VStack but horizontal
        foreach (var child in parent.Children)
        {
            var childLayout = new LayoutRect(currentX, y, 0, height);
            CalculateLayoutRecursive(child, childLayout, currentX, y);
            currentX += child.Layout.Width + spacing;
        }
    }

    private void LayoutGrid(UIElement parent, float x, float y, float width, float height)
    {
        var grid = parent as GridElement ?? new GridElement();
        float cellWidth = width / grid.Columns;
        float cellHeight = height / grid.Rows;

        int row = 0, col = 0;

        foreach (var child in parent.Children)
        {
            float cellX = x + (col * cellWidth);
            float cellY = y + (row * cellHeight);

            var childLayout = new LayoutRect(cellX, cellY, cellWidth, cellHeight);
            CalculateLayoutRecursive(child, childLayout, cellX, cellY);

            col++;
            if (col >= grid.Columns)
            {
                col = 0;
                row++;
            }
        }
    }

    private void LayoutDock(UIElement parent, float x, float y, float width, float height)
    {
        // Simplified dock layout
        foreach (var child in parent.Children)
        {
            var childLayout = new LayoutRect(x, y, width, height);
            CalculateLayoutRecursive(child, childLayout, x, y);
        }
    }

    private float ResolveDimension(SizingValue size, float parentSize)
    {
        return _themeMetrics.ResolveSizingValue(size, parentSize, _themeMetrics);
    }

    private float CalculateUsedHeight(List<UIElement> children, float parentWidth, float parentHeight)
    {
        float total = 0;
        foreach (var child in children)
        {
            if (child.BoxModel.Height.UnitType != SizingUnitType.Star)
            {
                total += ResolveDimension(child.BoxModel.Height, parentHeight);
                total += child.BoxModel.Margin.Vertical;
            }
        }
        return total;
    }
}
