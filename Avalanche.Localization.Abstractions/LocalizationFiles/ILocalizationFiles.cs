// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization;
using Avalanche.Template;
using Avalanche.Utilities.Provider;

// <docs>
/// <summary>Localization file properties and providers.</summary>
public interface ILocalizationFiles
{
    /// <summary>Localization file formats.</summary>
    /// <remarks>Reference to this field can used by the instances in <see cref="FileProviders"/>.</remarks>
    IList<ILocalizationFileFormat> FileFormats { get; set; }
    /// <summary>File systems.</summary>
    /// <remarks>Reference to this field can used by the instances in <see cref="FileProviders"/>.</remarks>
    IList<ILocalizationFileSystem> FileSystems { get; set; }
    /// <summary>File systems where file listings are cached. This enumerable is derived from <see cref="FileSystems"/> and contains cache decorated references.</summary>
    /// <remarks>Reference to this field can used by the instances in <see cref="FileProviders"/>.</remarks>
    IEnumerable<ILocalizationFileSystem> FileSystemsListCached { get; set; }
    /// <summary>File name patterns without extension, e.g. "{Culture}/{Namespace}". Template text can uses parameters: "Culture", "Namespace", "Name", "Key".</summary>
    /// <remarks>Reference to this field can used by file provider instances.</remarks>
    IList<ITemplateFormatPrintable> FilePatterns { get; set; }
    /// <summary>Explicitly assigned culture and key invariant localization files.</summary>
    /// <remarks>Reference to this field can used by the instances in <see cref="FileProviders"/>.</remarks>
    IList<ILocalizationFile> Files { get; set; }
    /// <summary>Localization file query providers</summary>
    IList<IProvider<(string? culture, string? key), IEnumerable<ILocalizationFile>>> FileProviders { get; set; }
    /// <summary>Localization file query providers</summary>
    IList<IProvider<(string? culture, string? key), IEnumerable<ILocalizationFile>>> FileProvidersCached { get; set; }

    /// <summary>Query provider that uses optional culture and key constraints. Sources content from the providers in <see cref="FileProviders"/>. (no caching)</summary>
    IProvider<(string? culture, string? key), IEnumerable<ILocalizationFile>> Query { get; set; }
    /// <summary>Query provider that uses optional culture and key constraints. Sources content from the providers in <see cref="FileProviders"/>. (cached)</summary>
    IProvider<(string? culture, string? key), IEnumerable<ILocalizationFile>> QueryCached { get; set; }
}
// </docs>
