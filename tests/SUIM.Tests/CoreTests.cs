namespace SUIM.Tests;

using Xunit;
using SUIM.Core;
using SUIM.Core.Elements;
using SUIM.Core.Layout;

public class SizingValueTests
{
    [Fact]
    public void PixelValue_ToString_ReturnsCorrectFormat()
    {
        var value = SizingValue.Pixels(100);
        Assert.Equal("100px", value.ToString());
    }

    [Fact]
    public void RemValue_ToString_ReturnsCorrectFormat()
    {
        var value = SizingValue.Rem(2);
        Assert.Equal("2rem", value.ToString());
    }

    [Fact]
    public void PercentageWidthValue_ToString_ReturnsCorrectFormat()
    {
        var value = SizingValue.PercentageWidth(50);
        Assert.Equal("50pw", value.ToString());
    }

    [Fact]
    public void StarValue_ToString_ReturnsCorrectFormat()
    {
        var value = SizingValue.Star(2);
        Assert.Equal("2*", value.ToString());
    }

    [Fact]
    public void AutoValue_ToString_ReturnsCorrectFormat()
    {
        var value = SizingValue.Auto();
        Assert.Equal("auto", value.ToString());
    }
}

public class BoxModelTests
{
    [Fact]
    public void DefaultSpacing_HasZeroValues()
    {
        var spacing = new Spacing();
        Assert.Equal(0, spacing.Top);
        Assert.Equal(0, spacing.Right);
        Assert.Equal(0, spacing.Bottom);
        Assert.Equal(0, spacing.Left);
    }

    [Fact]
    public void UniformSpacing_SetsAllValues()
    {
        var spacing = new Spacing(10);
        Assert.Equal(10, spacing.Top);
        Assert.Equal(10, spacing.Right);
        Assert.Equal(10, spacing.Bottom);
        Assert.Equal(10, spacing.Left);
    }

    [Fact]
    public void VerticalHorizontalSpacing_SetsCorrectValues()
    {
        var spacing = new Spacing(8, 12);
        Assert.Equal(8, spacing.Top);
        Assert.Equal(12, spacing.Right);
        Assert.Equal(8, spacing.Bottom);
        Assert.Equal(12, spacing.Left);
    }

    [Fact]
    public void Spacing_CalculatesHorizontalAndVertical()
    {
        var spacing = new Spacing(2, 3, 4, 5);
        Assert.Equal(8, spacing.Horizontal); // 5 + 3
        Assert.Equal(6, spacing.Vertical);   // 2 + 4
    }
}

public class LayoutRectTests
{
    [myFact]
    public void LayoutRect_CalculatesRightAndBottom()
    {
        var rect = new LayoutRect(10, 20, 100, 50);
        Assert.Equal(110, rect.Right);
        Assert.Equal(70, rect.Bottom);
    }

    [Fact]
    public void Contains_ReturnsTrueForPointInside()
    {
        var rect = new LayoutRect(0, 0, 100, 100);
        Assert.True(rect.Contains(50, 50));
    }

    [Fact]
    public void Contains_ReturnsFalseForPointOutside()
    {
        var rect = new LayoutRect(0, 0, 100, 100);
        Assert.False(rect.Contains(150, 150));
    }

    [Fact]
    public void Contains_ReturnsFalseForPointOnRightEdge()
    {
        var rect = new LayoutRect(0, 0, 100, 100);
        Assert.False(rect.Contains(100, 50));
    }
}

public class UIElementTests
{
    [Fact]
    public void AddChild_AddsChildToCollection()
    {
        var parent = new UIElement("div");
        var child = new UIElement("span");

        parent.AddChild(child);

        Assert.Single(parent.Children);
        Assert.Equal(parent, child.Parent);
    }

    [Fact]
    public void RemoveChild_RemovesChildFromCollection()
    {
        var parent = new UIElement("div");
        var child = new UIElement("span");
        parent.AddChild(child);

        parent.RemoveChild(child);

        Assert.Empty(parent.Children);
        Assert.Null(child.Parent);
    }

    [Fact]
    public void FindById_FindsElementByIdRecursively()
    {
        var root = new UIElement("div") { Id = "root" };
        var child1 = new UIElement("div") { Id = "child1" };
        var child2 = new UIElement("div") { Id = "child2" };
        var grandchild = new UIElement("span") { Id = "target" };

        root.AddChild(child1);
        root.AddChild(child2);
        child2.AddChild(grandchild);

        var found = root.FindById("target");
        Assert.NotNull(found);
        Assert.Equal(grandchild, found);
    }

    [Fact]
    public void EventHandler_IsTriggeredWhenEventFires()
    {
        var element = new UIElement("button");
        var triggered = false;

        element.On("click", (_) => triggered = true);
        element.Trigger("click");

        Assert.True(triggered);
    }
}

public class ThemeMetricTableTests
{
    [Fact]
    public void ResolveSizingValue_PixelsReturnExactValue()
    {
        var metrics = new ThemeMetricTable();
        var result = metrics.ResolveSizingValue(SizingValue.Pixels(50), 100);
        Assert.Equal(50, result);
    }

    [Fact]
    public void ResolveSizingValue_RemMultipliesRootFontSize()
    {
        var metrics = new ThemeMetricTable { RootFontSize = 16 };
        var result = metrics.ResolveSizingValue(SizingValue.Rem(2), 100);
        Assert.Equal(32, result);
    }

    [Fact]
    public void ResolveSizingValue_PercentageOfParent()
    {
        var metrics = new ThemeMetricTable();
        var result = metrics.ResolveSizingValue(SizingValue.PercentageWidth(50), 200);
        Assert.Equal(100, result);
    }
}

public class LayoutEngineTests
{
    [Fact]
    public void CalculateLayout_SetsLayoutRectForRoot()
    {
        var engine = new LayoutEngine();
        var root = new UIElement("div");

        engine.CalculateLayout(root, 800, 600);

        Assert.Equal(0, root.Layout.X);
        Assert.Equal(0, root.Layout.Y);
        Assert.Equal(800, root.Layout.Width);
        Assert.Equal(600, root.Layout.Height);
    }

    [Fact]
    public void CalculateLayout_LayoutsVStackChildren()
    {
        var engine = new LayoutEngine();
        var root = new VStackElement
        {
            BoxModel = new BoxModel { Width = SizingValue.Pixels(100), Height = SizingValue.Pixels(200) }
        };

        var child1 = new UIElement("div") { BoxModel = new BoxModel { Height = SizingValue.Pixels(50) } };
        var child2 = new UIElement("div") { BoxModel = new BoxModel { Height = SizingValue.Pixels(50) } };

        root.AddChild(child1);
        root.AddChild(child2);

        engine.CalculateLayout(root, 800, 600);

        Assert.Equal(0, child1.Layout.X);
        Assert.Equal(0, child1.Layout.Y);
        Assert.Equal(50, child2.Layout.Y);
    }
}
