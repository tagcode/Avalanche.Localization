// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization;
using System.Collections.Generic;
using Avalanche.Utilities;
using Avalanche.Utilities.Provider;

// <docs>
/// <summary>Record of line query settings and providers</summary>
public interface ILocalizationLines
{
    /// <summary>The provider that converts <see cref="ILocalizationFile"/> into lines.</summary>
    IProvider<ILocalizationFile, IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>>> FileReader { get; set; }
    /// <summary>The provider that converts <see cref="ILocalizationFile"/> into lines and caches.</summary>
    IProvider<ILocalizationFile, IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>>> FileReaderCached { get; set; }

    /// <summary>Queriers that return files that contain localization lines (Have format that implements <see cref="ILocalizationLineFileFormat"/>). Typically <see cref="ILocalizationFiles.Query"/></summary>
    IList<IProvider<(string? culture, string? @namespace), IEnumerable<ILocalizationFile>>> FileProviders { get; set; }
    /// <summary>Queriers that return files that contain localization lines (Have format that implements <see cref="ILocalizationLineFileFormat"/>). Typically <see cref="ILocalizationFiles.QueryCached"/></summary>
    IList<IProvider<(string? culture, string? @namespace), IEnumerable<ILocalizationFile>>> FileProvidersCached { get; set; }

    /// <summary>Explictly added lines</summary>
    IList<IEnumerable<KeyValuePair<string, MarkedText>>> Lines { get; set; }
    /// <summary>Line query providers.</summary>
    IList<IProvider<(string? culture, string? key), IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>>>> LineProviders { get; set; }
    /// <summary>Line query providers.</summary>
    IList<IProvider<(string? culture, string? key), IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>>>> LineProvidersCached { get; set; }

    /// <summary>Line query provider. Culture and Key are constraints, if null then less is filtered and more is passed.</summary>
    IProvider<(string? culture, string? key), IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>>> Query { get; set; }
    /// <summary>Line query provider. Culture and Key are constraints, if null then less is filtered and more is passed.</summary>
    IProvider<(string? culture, string? key), IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>>> QueryCached { get; set; }
}
// </docs>
