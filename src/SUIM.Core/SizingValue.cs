namespace SUIM.Core;

/// <summary>
/// Represents the different sizing unit types in SUIM.
/// </summary>
public enum SizingUnitType
{
    /// <summary>Fixed pixel values.</summary>
    Pixels,
    
    /// <summary>Multiplier of the global root font size (rem = relative to root em).</summary>
    Rem,
    
    /// <summary>Percentage of Parent Width (pw).</summary>
    PercentageWidth,
    
    /// <summary>Percentage of Parent Height (ph).</summary>
    PercentageHeight,
    
    /// <summary>Proportional sharing of remaining space (1*, 2*, etc).</summary>
    Star,
    
    /// <summary>Resolves to a pre-defined constant in the ThemeMetricTable.</summary>
    Auto
}

/// <summary>
/// Represents a sizing value with its unit type and numeric value.
/// </summary>
public struct SizingValue
{
    public SizingUnitType UnitType { get; set; }
    public float Value { get; set; }

    public SizingValue(SizingUnitType unitType, float value)
    {
        UnitType = unitType;
        Value = value;
    }

    public static SizingValue Pixels(float value) => new(SizingUnitType.Pixels, value);
    public static SizingValue Rem(float value) => new(SizingUnitType.Rem, value);
    public static SizingValue PercentageWidth(float value) => new(SizingUnitType.PercentageWidth, value);
    public static SizingValue PercentageHeight(float value) => new(SizingUnitType.PercentageHeight, value);
    public static SizingValue Star(float value) => new(SizingUnitType.Star, value);
    public static SizingValue Auto() => new(SizingUnitType.Auto, 1);

    public override string ToString() => UnitType switch
    {
        SizingUnitType.Pixels => $"{Value}px",
        SizingUnitType.Rem => $"{Value}rem",
        SizingUnitType.PercentageWidth => $"{Value}pw",
        SizingUnitType.PercentageHeight => $"{Value}ph",
        SizingUnitType.Star => $"{Value}*",
        SizingUnitType.Auto => "auto",
        _ => "unknown"
    };
}
