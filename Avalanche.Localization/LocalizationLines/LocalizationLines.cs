// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization;

/// <summary>Record of line query settings and providers</summary>
public class LocalizationLines : LocalizationLinesBase
{
    /// <summary></summary>
    public static ILocalizationLines Instance => Localization.Default.Lines;

    /// <summary></summary>
    public LocalizationLines() : base()
    {
    }
}
