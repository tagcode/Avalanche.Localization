// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization;
using System.Diagnostics.CodeAnalysis;

// <docs>
/// <summary>Interface to file system, such as OS, Microsoft.Extensions.FileSystem, embedded resources file-system.</summary>
public interface ILocalizationFileSystem
{
    /// <summary>Try list files at <paramref name="path"/>.</summary>
    /// <param name="path">path, "" for root. Slash '/' is directory separator, e.g. "path/path".</param>
    /// <param name="files">File names relative to root.</param>
    /// <exception cref="Exception">On unexpected error.</exception>
    bool TryListFiles(string path, [NotNullWhen(true)] out string[]? files);

    /// <summary>Try list sub-directories of <paramref name="path"/>.</summary>
    /// <param name="path">path, "" for root. Slash '/' is directory separator, e.g. "path/path".</param>
    /// <param name="directories">Directory names relative to root.</param>
    /// <exception cref="Exception">On unexpected error.</exception>
    bool TryListDirectories(string path, [NotNullWhen(true)] out string[]? directories);

    /// <summary>Try open <paramref name="filepath"/>.</summary>
    /// <param name="filepath">Relative file path, "" for root. Slash '/' is directory separator, e.g. "path/path".</param>
    /// <exception cref="Exception">On unexpected error.</exception>
    bool TryOpen(string filepath, [NotNullWhen(true)] out Stream stream);
}
// </docs>
