using System.Numerics;

namespace SUIM.Core.Layout
{
    public enum HeightStrategy
    {
        Fixed,
        Auto,
        Stretch
    }

    public class SUIMElement
    {
        // Bounds of the element in layout space
        public RectangleF Bounds { get; set; }

        // Padding inside the element
        public float PaddingLeft { get; set; }
        public float PaddingTop { get; set; }
        public float PaddingRight { get; set; }
        public float PaddingBottom { get; set; }

        // Margin outside the element
        public float MarginLeft { get; set; }
        public float MarginTop { get; set; }
        public float MarginRight { get; set; }
        public float MarginBottom { get; set; }

        // Spacing between child elements (if any)
        public float Spacing { get; set; }

        // Height behavior for layout
        public HeightStrategy HeightStrategy { get; set; } = HeightStrategy.Auto;

        public SUIMElement()
        {
            Bounds = new RectangleF(0, 0, 0, 0);
            PaddingLeft = PaddingTop = PaddingRight = PaddingBottom = 0f;
            MarginLeft = MarginTop = MarginRight = MarginBottom = 0f;
            Spacing = 0f;
        }

        // Compute layout given the available space. Override in derived classes.
        public virtual void ComputeLayout(RectangleF availableSpace)
        {
            // Default behavior: set Bounds to available space reduced by margins
            var x = availableSpace.X + MarginLeft;
            var y = availableSpace.Y + MarginTop;
            var width = availableSpace.Width - (MarginLeft + MarginRight);
            var height = availableSpace.Height - (MarginTop + MarginBottom);

            Bounds = new RectangleF(x, y, width, height);
        }
    }

    // Simple RectangleF struct for layout (if not already present in project)
    public struct RectangleF
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }

        public RectangleF(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }
    }
}
