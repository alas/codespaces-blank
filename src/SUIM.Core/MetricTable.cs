using Stride.Graphics;
using System.Collections.Generic;

namespace SUIM.Core.Layout
{
    public class MetricTable
    {
        // Faster than a Dictionary: direct index access for ASCII
        private readonly float[] _charWidths = new float[256];
        private readonly float _fallbackWidth;
        
        public float LineHeight { get; private set; }

        public MetricTable(SpriteFont font)
        {
            // 1. Store the vertical metric directly from Stride
            LineHeight = font.LineSpacing;

            // 2. Bake widths for standard characters (ASCII 0-255)
            // This happens once at startup/load time
            for (int i = 0; i < 256; i++)
            {
                char c = (char)i;
                // We measure a single character string
                _charWidths[i] = font.MeasureString(c.ToString()).X;
            }

            _fallbackWidth = _charWidths['?'];
        }

        /// <summary>
        /// Calculates the width of a string in O(N) time using only array lookups.
        /// </summary>
        public float GetTextWidth(string text)
        {
            if (string.IsNullOrEmpty(text)) return 0f;

            float totalWidth = 0f;
            foreach (char c in text)
            {
                if (c < 256)
                    totalWidth += _charWidths[c];
                else
                    totalWidth += _fallbackWidth;
            }

            // Optional: Add a 2% safety buffer to account for the lack of kerning
            return totalWidth * 1.02f;
        }
    }
}
