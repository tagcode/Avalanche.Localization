// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

/// <summary>Extension methods for <see cref="ILocalizationFileSystem"/></summary>
public static class LocalizationFileSystemExtensions
{
    /// <summary>Try list files at <paramref name="path"/>.</summary>
    /// <param name="path">path, "" for root. Slash '/' is directory separator, e.g. "path/path".</param>
    /// <exception cref="IOException"></exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining), DebuggerHidden]
    public static string[] ListFiles(this ILocalizationFileSystem filesystem, string path) => filesystem.TryListFiles(path, out string[]? files) ? files : throw new FileNotFoundException(path);

    /// <summary>Try list sub-directories of <paramref name="path"/>.</summary>
    /// <param name="path">path, "" for root. Slash '/' is directory separator, e.g. "path/path".</param>
    /// <exception cref="Exception">On unexpected error.</exception>
    /// <exception cref="IOException"></exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining), DebuggerHidden]
    public static string[]? ListDirectores(this ILocalizationFileSystem filesystem, string path) => filesystem.TryListDirectories(path, out string[]? files) ? files : throw new DirectoryNotFoundException(path);

    /// <summary>Try open <paramref name="filepath"/>.</summary>
    /// <param name="filepath">Relative file path, "" for root. Slash '/' is directory separator, e.g. "path/path".</param>
    /// <exception cref="Exception">On unexpected error.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining), DebuggerHidden]
    public static Stream Open(this ILocalizationFileSystem filesystem, string filepath) => filesystem.TryOpen(filepath, out Stream? stream) ? stream : throw new FileNotFoundException(filepath);

    /// <summary>Try open <paramref name="filepath"/>.</summary>
    /// <param name="filepath">Relative file path, "" for root. Slash '/' is directory separator, e.g. "path/path".</param>
    /// <exception cref="IOException"></exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining), DebuggerHidden]
    public static bool TryReadFully(this ILocalizationFileSystem filesystem, string filepath, [NotNullWhen(false)] out byte[]? data)
    {
        // Open
        if (!filesystem.TryOpen(filepath, out Stream? stream)) { data = null!; return false; }
        try
        {
            // Read
            data = LocalizationFileExtensions.ReadFully(stream);
            // Return
            return true;
        }
        finally
        {
            // Close file
            stream.Dispose();
        }
    }

    /// <summary>Read file</summary>
    /// <exception cref="IOException"></exception>
    public static byte[] ReadFully(this ILocalizationFileSystem filesystem, string filepath)
    {
        // Open
        if (!filesystem.TryOpen(filepath, out Stream? stream)) throw new FileNotFoundException(filepath);
        try
        {
            // Read
            byte[] data = LocalizationFileExtensions.ReadFully(stream);
            // Return
            return data;
        } finally
        {
            // Close file
            stream.Dispose();
        }
    }

    /// <summary>List all directories recursive</summary>
    /// <param name="filesystem"></param>
    /// <param name="path">start path, "" for root. Slash '/' is directory separator, e.g. "path/path".</param>
    /// <param name="depth">Levels to browse starting from <paramref name="path"/>.</param>
    public static IList<string> ListAllDirectories(this ILocalizationFileSystem filesystem, string path, int depth = int.MaxValue, bool detectCycles = false)
    {
        // Initialize
        List<string> result = new List<string>();
        List<(string, int)> queue = new List<(string, int)> { (path, depth) };
        HashSet<string>? visited = detectCycles ? new() : null;
        // Visit paths
        while (queue.Count>0)
        {
            // Next index
            int ix = queue.Count - 1;
            // Take next
            (string _path, int _depth) = queue[ix]; queue.RemoveAt(ix);
            // Detect cycles
            if (visited!=null && !visited!.Add(_path)) continue;
            // Get children
            if (_depth>0 && filesystem.TryListDirectories(_path, out string[]? children) && children != null) foreach (string child in children) queue.Add((child, _depth - 1));
            // Add to result
            result.Add(_path);
        }
        // Return
        return result;
    }

    /// <summary>List all files recursively.</summary>
    /// <param name="filesystem"></param>
    /// <param name="path">start path, "" for root. Slash '/' is directory separator, e.g. "path/path".</param>
    /// <param name="depth">Levels to browse starting from <paramref name="path"/>.</param>
    public static IList<string> ListAllFiles(this ILocalizationFileSystem filesystem, string path, int depth = int.MaxValue, bool detectCycles = false)
    {
        // Initialize
        List<string> result = new List<string>();
        List<(string, int)> queue = new List<(string, int)> { (path, depth) };
        HashSet<string>? visited = detectCycles ? new() : null;
        // Visit paths
        while (queue.Count>0)
        {
            // Next index
            int ix = queue.Count - 1;
            // Take next
            (string _path, int _depth) = queue[ix]; queue.RemoveAt(ix);
            // Detect cycles
            if (visited != null && !visited!.Add(_path)) continue;
            // Get child directories
            if (_depth>0 && filesystem.TryListDirectories(_path, out string[]? children) && children != null) foreach(string child in children) queue.Add((child, _depth-1));
            // Get files
            if (filesystem.TryListFiles(_path, out string[]? files) && files != null) result.AddRange(files);
        }
        // Return
        return result;
    }

}

