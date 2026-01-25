namespace SUIM.Core;

/// <summary>
/// Theme metrics table for resolving auto sizing and global constants.
/// </summary>
public class ThemeMetricTable
{
    /// <summary>Root font size in pixels for rem calculations.</summary>
    public float RootFontSize { get; set; } = 16f;

    /// <summary>Default button height in pixels.</summary>
    public float DefaultButtonHeight { get; set; } = 40f;

    /// <summary>Default input height in pixels.</summary>
    public float DefaultInputHeight { get; set; } = 36f;

    /// <summary>Default spacing (gap) between elements in pixels.</summary>
    public float DefaultSpacing { get; set; } = 8f;

    /// <summary>Default border radius in pixels.</summary>
    public float DefaultBorderRadius { get; set; } = 4f;

    /// <summary>Resolves a sizing value to pixel constants.</summary>
    public float ResolveSizingValue(SizingValue size, float parentSize, ThemeMetricTable? metrics = null)
    {
        metrics ??= this;
        
        return size.UnitType switch
        {
            SizingUnitType.Pixels => size.Value,
            SizingUnitType.Rem => size.Value * metrics.RootFontSize,
            SizingUnitType.PercentageWidth => (size.Value / 100f) * parentSize,
            SizingUnitType.PercentageHeight => (size.Value / 100f) * parentSize,
            SizingUnitType.Auto => metrics.DefaultButtonHeight, // Default auto value
            _ => 0f
        };
    }
}
