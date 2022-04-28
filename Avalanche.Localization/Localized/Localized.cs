// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization;

/// <summary></summary>
public class Localized<T> : LocalizedBase<T>
{
    /// <summary>Return <paramref name="value"/> if it already implements <![CDATA[ILocalized<T>]]>, otherwise wrap it into one.</summary>
    public static ILocalized<T> GetOrCreate(string key, string culture, T value)
        => value is ILocalized<T> localized ? localized : 
        new Localized<T>
        {
            Key = key,
            Culture = culture,
            Value = value
        };
}
