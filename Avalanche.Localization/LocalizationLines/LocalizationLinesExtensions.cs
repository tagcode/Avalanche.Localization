// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization;
using Avalanche.Utilities;
using Avalanche.Utilities.Provider;
using System.Collections;
using System.Resources;

/// <summary></summary>
public static class LocalizationLinesExtensions_
{
    /// <summary>Create enumerable reads a snapshot of <see cref="ILocalizationLines.FileProviders"/> field.</summary>
    public static IEnumerable<IProvider<(string? culture, string? @namespace), IEnumerable<ILocalizationFile>>> FileProvidersField(this ILocalizationLines localizationLines) => new FileProvidersFieldEnumerable(localizationLines);
    /// <summary>Create enumerable reads a snapshot of <see cref="ILocalizationLines.FileProvidersCached"/> field.</summary>
    public static IEnumerable<IProvider<(string? culture, string? @namespace), IEnumerable<ILocalizationFile>>> FileProvidersCachedField(this ILocalizationLines localizationLines) => new FileProvidersCachedFieldEnumerable(localizationLines);
    /// <summary>Create enumerable reads a snapshot of <see cref="ILocalizationLines.Lines"/> field.</summary>
    public static IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>> LinesField(this ILocalizationLines localizationLines) => new LinesFieldEnumerable(localizationLines);
    /// <summary>Create enumerable reads a snapshot of <see cref="ILocalizationLines.LineProviders"/> field.</summary>
    public static IEnumerable<IProvider<(string? culture, string? key), IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>>>> LineProvidersField(this ILocalizationLines localizationLines) => new LineProvidersFieldEnumerable(localizationLines);
    /// <summary>Create enumerable reads a snapshot of <see cref="ILocalizationLines.LineProvidersCached"/> field.</summary>
    public static IEnumerable<IProvider<(string? culture, string? key), IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>>>> LineProvidersCachedField(this ILocalizationLines localizationLines) => new LineProvidersCachedFieldEnumerable(localizationLines);

    /// <summary>Returns snapshot value of <see cref="ILocalizationLines.FileProviders"/>.</summary>
    internal class FileProvidersFieldEnumerable : IEnumerable<IProvider<(string? culture, string? @namespace), IEnumerable<ILocalizationFile>>>, ISnapshotProvider<IProvider<(string? culture, string? @namespace), IEnumerable<ILocalizationFile>>>
    {
        /// <summary></summary>
        public readonly ILocalizationLines LocalizationLines;
        /// <summary></summary>
        public IProvider<(string? culture, string? @namespace), IEnumerable<ILocalizationFile>>[] Snapshot { get => ArrayUtilities.GetSnapshotArray(LocalizationLines.FileProviders); set => throw new InvalidOperationException(); }
        /// <summary></summary>
        public FileProvidersFieldEnumerable(ILocalizationLines localizationLines) => this.LocalizationLines = localizationLines ?? throw new ArgumentNullException(nameof(localizationLines));
        /// <summary></summary>
        public IEnumerator<IProvider<(string? culture, string? @namespace), IEnumerable<ILocalizationFile>>> GetEnumerator() => ArrayUtilities.GetSnapshot(LocalizationLines.FileProviders).GetEnumerator();
        /// <summary></summary>
        IEnumerator IEnumerable.GetEnumerator() => ArrayUtilities.GetSnapshot(LocalizationLines.FileProviders).GetEnumerator();
        /// <summary>Print information</summary>
        public override string ToString() => $"{LocalizationLines}.{nameof(ILocalizationLines.FileProviders)}";
    }

    /// <summary>Returns snapshot value of <see cref="ILocalizationLines.FileProvidersCached"/>.</summary>
    internal class FileProvidersCachedFieldEnumerable : IEnumerable<IProvider<(string? culture, string? @namespace), IEnumerable<ILocalizationFile>>>, ISnapshotProvider<IProvider<(string? culture, string? @namespace), IEnumerable<ILocalizationFile>>>
    {
        /// <summary></summary>
        public readonly ILocalizationLines LocalizationLines;
        /// <summary></summary>
        public IProvider<(string? culture, string? @namespace), IEnumerable<ILocalizationFile>>[] Snapshot { get => ArrayUtilities.GetSnapshotArray(LocalizationLines.FileProvidersCached); set => throw new InvalidOperationException(); }
        /// <summary></summary>
        public FileProvidersCachedFieldEnumerable(ILocalizationLines localizationLines) => this.LocalizationLines = localizationLines ?? throw new ArgumentNullException(nameof(localizationLines));
        /// <summary></summary>
        public IEnumerator<IProvider<(string? culture, string? @namespace), IEnumerable<ILocalizationFile>>> GetEnumerator() => ArrayUtilities.GetSnapshot(LocalizationLines.FileProvidersCached).GetEnumerator();
        /// <summary></summary>
        IEnumerator IEnumerable.GetEnumerator() => ArrayUtilities.GetSnapshot(LocalizationLines.FileProvidersCached).GetEnumerator();
        /// <summary>Print information</summary>
        public override string ToString() => $"{LocalizationLines}.{nameof(ILocalizationLines.FileProvidersCached)}";
    }

    /// <summary>Returns snapshot value of <see cref="ILocalizationLines.Lines"/>.</summary>
    internal class LinesFieldEnumerable : IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>>, ISnapshotProvider<IEnumerable<KeyValuePair<string, MarkedText>>>
    {
        /// <summary></summary>
        public readonly ILocalizationLines LocalizationLines;
        /// <summary></summary>
        public IEnumerable<KeyValuePair<string, MarkedText>>[] Snapshot { get => ArrayUtilities.GetSnapshotArray(LocalizationLines.Lines); set => throw new InvalidOperationException(); }
        /// <summary></summary>
        public LinesFieldEnumerable(ILocalizationLines localizationLines) => this.LocalizationLines = localizationLines ?? throw new ArgumentNullException(nameof(localizationLines));
        /// <summary></summary>
        public IEnumerator<IEnumerable<KeyValuePair<string, MarkedText>>> GetEnumerator() => ArrayUtilities.GetSnapshot(LocalizationLines.Lines).GetEnumerator();
        /// <summary></summary>
        IEnumerator IEnumerable.GetEnumerator() => ArrayUtilities.GetSnapshot(LocalizationLines.Lines).GetEnumerator();
        /// <summary>Print information</summary>
        public override string ToString() => $"{LocalizationLines}.{nameof(ILocalizationLines.Lines)}";
    }

    /// <summary>Returns snapshot value of <see cref="ILocalizationLines.LineProviders"/>.</summary>
    internal class LineProvidersFieldEnumerable : IEnumerable<IProvider<(string? culture, string? key), IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>>>>, ISnapshotProvider<IProvider<(string? culture, string? key), IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>>>>
    {
        /// <summary></summary>
        public readonly ILocalizationLines LocalizationLines;
        /// <summary></summary>
        public IProvider<(string? culture, string? key), IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>>>[] Snapshot { get => ArrayUtilities.GetSnapshotArray(LocalizationLines.LineProviders); set => throw new InvalidOperationException(); }
        /// <summary></summary>
        public LineProvidersFieldEnumerable(ILocalizationLines localizationLines) => this.LocalizationLines = localizationLines ?? throw new ArgumentNullException(nameof(localizationLines));
        /// <summary></summary>
        public IEnumerator<IProvider<(string? culture, string? key), IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>>>> GetEnumerator() => ArrayUtilities.GetSnapshot(LocalizationLines.LineProviders).GetEnumerator();
        /// <summary></summary>
        IEnumerator IEnumerable.GetEnumerator() => ArrayUtilities.GetSnapshot(LocalizationLines.LineProviders).GetEnumerator();
        /// <summary>Print information</summary>
        public override string ToString() => $"{LocalizationLines}.{nameof(ILocalizationLines.LineProviders)}";
    }

    /// <summary>Returns snapshot value of <see cref="ILocalizationLines.LineProvidersCached"/>.</summary>
    internal class LineProvidersCachedFieldEnumerable : IEnumerable<IProvider<(string? culture, string? key), IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>>>>, ISnapshotProvider<IProvider<(string? culture, string? key), IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>>>>
    {
        /// <summary></summary>
        public readonly ILocalizationLines LocalizationLines;
        /// <summary></summary>
        public IProvider<(string? culture, string? key), IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>>>[] Snapshot { get => ArrayUtilities.GetSnapshotArray(LocalizationLines.LineProvidersCached); set => throw new InvalidOperationException(); }
        /// <summary></summary>
        public LineProvidersCachedFieldEnumerable(ILocalizationLines localizationLines) => this.LocalizationLines = localizationLines ?? throw new ArgumentNullException(nameof(localizationLines));
        /// <summary></summary>
        public IEnumerator<IProvider<(string? culture, string? key), IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>>>> GetEnumerator() => ArrayUtilities.GetSnapshot(LocalizationLines.LineProvidersCached).GetEnumerator();
        /// <summary></summary>
        IEnumerator IEnumerable.GetEnumerator() => ArrayUtilities.GetSnapshot(LocalizationLines.LineProvidersCached).GetEnumerator();
        /// <summary>Print information</summary>
        public override string ToString() => $"{LocalizationLines}.{nameof(ILocalizationLines.LineProvidersCached)}";
    }

    /*
    /// <summary>Returns snapshot value of <see cref="ILocalizationFileProviders.X"/>.</summary>
    internal class XField : IEnumerable<>, IArrayProvider<>
    {
        /// <summary></summary>
        public readonly ILocalizationLines2 LocalizationLines;
        /// <summary></summary>
        public [] Array { get => ArrayUtilities.GetSnapshotArray(LocalizationLines.); set => throw new InvalidOperationException(); }
        /// <summary></summary>
        public XField(ILocalizationLines2 localizationLines) => this.LocalizationLines = localizationLines ?? throw new ArgumentNullException(nameof(localizationLines));
        /// <summary></summary>
        public IEnumerator<> GetEnumerator() => ArrayUtilities.GetSnapshot(LocalizationLines.X).GetEnumerator();
        /// <summary></summary>
        IEnumerator IEnumerable.GetEnumerator() => ArrayUtilities.GetSnapshot(LocalizationLines.X).GetEnumerator();
        /// <summary>Print information</summary>
        public override string ToString() => $"{LocalizationLines}.{nameof(ILocalizationLines2.X)}";
    }
    */

}
