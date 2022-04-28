// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Avalanche.Utilities;
using Avalanche.Utilities.Provider;

/// <summary>File reader</summary>
public class LocalizationFileSystem : ILocalizationFileSystem
{
    /// <summary>File provider that points to application's binary root</summary>
    static LocalizationFileSystem applicationRoot = new LocalizationFileSystem(AppDomain.CurrentDomain.BaseDirectory, "ApplicationRoot");
    /// <summary>File provider that points to application's binary root</summary>
    static ILocalizationFileSystem applicationRootCached = applicationRoot.Cached(true, false);
    /// <summary>File provider that points to application's binary root</summary>
    public static LocalizationFileSystem ApplicationRoot => applicationRoot;
    /// <summary>File provider that points to application's binary root</summary>
    public static ILocalizationFileSystem ApplicationRootCached => applicationRootCached;

    /// <summary>Root directory</summary>
    protected string? root;
    /// <summary>File opener</summary>
    protected IProvider<string, Stream> fileOpener;
    /// <summary>Name for dignostincs</summary>
    protected string? name;

    /// <summary>Root directory absolute</summary>
    protected string rootAbsolute;

    /// <summary>Root directory</summary>
    public string? Root => root;
    /// <summary>File opener</summary>
    public IProvider<string, Stream> FileOpener => fileOpener;

    /// <summary></summary>
    public LocalizationFileSystem(string? root = null, string? name = null)
    {
        this.fileOpener = Providers.Func<string, Stream>(this.TryOpen);
        this.root = root;
        this.name = name;
        this.rootAbsolute = Path.GetFullPath(root ?? AppDomain.CurrentDomain.BaseDirectory);
    }

    /// <summary>
    /// Convert <paramref name="relativePath"/> to <paramref name="absolutePath"/>.
    /// Returns false if <paramref name="absolutePath"/> points beyond root path.
    /// </summary>
    protected virtual bool TryGetAbsolutePath(string relativePath, [NotNullWhen(true)] out string? absolutePath)
    {
        // Combine root to argument
        string _path = Path.Combine(this.rootAbsolute, relativePath);
        // Make absolute
        absolutePath = Path.IsPathRooted(_path) ? _path : Path.GetFullPath(_path);
        // Is valid?
        return absolutePath.StartsWith(this.rootAbsolute);
    }

    /// <summary></summary>
    public bool TryListFiles(string relativePath, [NotNullWhen(true)] out string[]? files)
    {
        // Get absolute path
        if (!TryGetAbsolutePath(relativePath, out string? absolutePath)) { files = null!; return false; }
        // No directory
        if (!Directory.Exists(absolutePath)) { files = null!; return false; }
        // List files
        string[] _files = Directory.GetFiles(absolutePath);
        // Get count
        int count = _files.Length;
        // No files
        if (count == 0) { files = Array.Empty<string>(); return true; }
        // Allocate result
        files = new string[count];
        // Re-usable string builder
        StringBuilder? sb = null;
        // Adapt each
        for (int i = 0; i < count; i++)
        {
            // Get name
            ReadOnlySpan<char> name = Path.GetFileName(_files[i].AsSpan());
            // Append to path
            files[i] = AppendPathAndName(relativePath, name, ref sb);
        }
        // Return
        return true;
    }
    
    /// <summary></summary>
    public bool TryListDirectories(string relativePath, [NotNullWhen(true)] out string[]? directories)
    {
        // Get absolute path
        if (!TryGetAbsolutePath(relativePath, out string? absolutePath)) { directories = null!; return false; }
        // No directory
        if (!Directory.Exists(absolutePath)) { directories = null!; return false; }
        // List directories
        string[] _directories = Directory.GetDirectories(absolutePath);
        // Get count
        int count = _directories.Length;
        // No directories
        if (count == 0) { directories = Array.Empty<string>(); return true; }
        // Allocate result
        directories = new string[count];
        // Re-usable string builder
        StringBuilder? sb = null;
        // Adapt each
        for (int i = 0; i < count; i++)
        {
            // Get name
            ReadOnlySpan<char> name = Path.GetFileName(_directories[i].AsSpan());
            // Append to path
            directories[i] = AppendPathAndName(relativePath, name, ref sb);
        }
        // Return
        return true;
    }

    /// <summary>Append <paramref name="path"/> to <paramref name="name"/> with separator '/'.</summary>
    /// <param name="sb">Recyclable string builder</param>
    /// <returns><paramref name="path"/> + '/' + <paramref name="name"/></returns>
    public static string AppendPathAndName(string? path, string name, ref StringBuilder? sb)
    {
        // Empty path
        if (string.IsNullOrEmpty(path)) return name;
        // Allocate/clear sb
        if (sb == null) sb = new StringBuilder(128); else sb.Clear();
        // Append path
        sb.Append(path);
        // Append separator
        if (path.Length>0 && path[path.Length-1] != '/') sb.Append('/');
        // Append name
        sb.Append(name);
        // Build result
        return sb.ToString();
    }

    /// <summary>Append <paramref name="path"/> to <paramref name="name"/> with separator '/'.</summary>
    /// <param name="sb">Recyclable string builder</param>
    /// <returns><paramref name="path"/> + '/' + <paramref name="name"/></returns>
    public static string AppendPathAndName(string? path, ReadOnlySpan<char> name, ref StringBuilder? sb)
    {
        // Empty path
        if (string.IsNullOrEmpty(path)) return name.ToString();
        // Allocate/clear sb
        if (sb == null) sb = new StringBuilder(128); else sb.Clear();
        // Append path
        sb.Append(path);
        // Append separator
        if (path.Length>0 && path[path.Length-1] != '/') sb.Append('/');
        // Append name
        sb.Append(name);
        // Build result
        return sb.ToString();
    }

    /// <summary></summary>
    public bool TryOpen(string filename, [NotNullWhen(true)] out Stream stream)
    {
        // Provider is rooted
        if (root != null)
        {
            // Rooted 'filename' not allowed on rooted provider
            if (Path.IsPathRooted(filename)) { stream = null!; return false; }
            // Add root path
            else filename = Path.Combine(root, filename);
        }
        // No file
        if (!File.Exists(filename)) { stream = null!; return false; }
        // Open file for reading
        stream = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        return true;
    }

    /// <summary></summary>
    public override int GetHashCode()
    {
        FNVHash32 hash = new FNVHash32();
        hash.HashIn(root);
        hash.HashIn(0x1234);
        return hash.Hash;
    }

    /// <summary></summary>
    public override bool Equals(object? obj)
    {
        if (obj is not LocalizationFileSystem other) return false;
        return other.root == this.root;
    }

    /// <summary>Print information</summary>
    public override string ToString() => name??(Root != null ? Root : GetType().Name);
}
