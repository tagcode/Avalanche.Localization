// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization;
using Avalanche.Utilities;

/// <summary>File localizer</summary>
public class FileLocalizer : Localizer<ILocalizationFile[]>, IFileLocalizer
{
    /// <summary>Element Type</summary>
    public override Type ResourceType => typeof(ILocalizationFile);
    /// <summary>Try get localized file</summary>
    protected override ILocalized? GetLocalized(string? name)
    {
        // Get culture
        if (!this.CultureProvider.TryGetCulture(out string language)) { SearchedLocation(null, null, null); return null!; }
        // Get key
        string? key = CreateKey(name);
        // No key
        if (key == null) { SearchedLocation(null, language, null); return null!; }
        // Try get file(s)       
        if (!localization.FileQueryCached.TryGetValue((language, key), out IEnumerable<ILocalizationFile> files) || files == null) { SearchedLocation(key, language, null); return null; }
        // Wrap into localized
        ILocalized<ILocalizationFile[]>? localized = new Localized<ILocalizationFile[]> { Key = key, Culture = language, Value = files is ILocalizationFile[] _array ? _array : files.ToArray() }.SetReadOnly();
        // Handle result
        SearchedLocation(key, language, localized);
        // Return
        return localized;
    }

    /// <summary></summary>
    public FileLocalizer() : base() { }
    /// <summary></summary>
    public FileLocalizer(ILocalization localization) : base(localization) { }
    /// <summary></summary>
    public FileLocalizer(ILocalization localization, ICultureProvider cultureProvider) : base(localization, cultureProvider) { }
    /// <summary></summary>
    public FileLocalizer(ILocalization localization, ICultureProvider cultureProvider, string? @namespace) : base(localization, cultureProvider, @namespace) { }
    /// <summary>Print information</summary>
    public override string ToString() => Namespace ?? GetType().Name;
}

/// <summary>File localizer</summary>
/// <typeparam name="Namespace">Key namespace</typeparam>
public class FileLocalizer<Namespace> : FileLocalizer, IFileLocalizer<Namespace>
{
    /// <summary></summary>
    public FileLocalizer() : base(null!, null!, PrintNamespace<Namespace>()) { }
    /// <summary></summary>
    public FileLocalizer(ILocalization localization) : base(localization, null!, PrintNamespace<Namespace>()) { }
    /// <summary></summary>
    public FileLocalizer(ILocalization localization, ICultureProvider cultureProvider) : base(localization, cultureProvider, PrintNamespace<Namespace>()) { }
    /// <summary></summary>
    public FileLocalizer(ILocalization localization, ICultureProvider cultureProvider, string? @namespace) : base(localization, cultureProvider, @namespace) { }
}

/// <summary>File localizer</summary>
/// <typeparam name="Namespace">Key namespace</typeparam>
/// <typeparam name="CultureProvider">Class that implements <see cref="ICultureProvider"/>.</typeparam>
public class FileLocalizer<Namespace, CultureProvider> : FileLocalizer<Namespace>, IFileLocalizer<Namespace, CultureProvider> where CultureProvider : ICultureProvider
{
    /// <summary></summary>
    public FileLocalizer() : base(null!, Avalanche.Localization.CultureProvider.ByTypeCached[typeof(CultureProvider)]) { }
    /// <summary></summary>
    public FileLocalizer(ILocalization localization) : base(localization, Avalanche.Localization.CultureProvider.ByTypeCached[typeof(CultureProvider)]) { }
    /// <summary></summary>
    public FileLocalizer(ILocalization localization, ICultureProvider cultureProvider) : base(localization, cultureProvider) { }
    /// <summary></summary>
    public FileLocalizer(ILocalization localization, ICultureProvider cultureProvider, string? @namespace) : base(localization, cultureProvider, @namespace) { }
}
