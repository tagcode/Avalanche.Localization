// Copyright (c) Toni Kalajainen 2022
 namespace Avalanche.Localization;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Avalanche.Utilities;
using Avalanche.Utilities.Provider;
using Microsoft.Extensions.FileProviders;

/// <summary>Adapts <see cref="IFileProvider"/> to <see cref="ILocalizationFileSystem"/></summary>
public class LocalizationMsFileSystem : ILocalizationFileSystem
{
    /// <summary></summary>
    protected IFileProvider? fileProvider;
    /// <summary>File provider</summary>
    protected IProvider<string, Stream> fileOpener;
    /// <summary>File provider</summary>
    public IProvider<string, Stream> FileOpener => fileOpener;

    /// <summary></summary>
    public LocalizationMsFileSystem()
    {
        this.fileProvider = null!;
        this.fileOpener = Providers.Func<string, Stream>(this.TryOpen);
    }

    /// <summary></summary>
    public LocalizationMsFileSystem(IFileProvider fileProvider)
    {
        this.fileProvider = fileProvider ?? throw new ArgumentNullException(nameof(fileProvider));
        this.fileOpener = Providers.Func<string, Stream>(this.TryOpen);
    }

    /// <summary></summary>
    public bool TryListFiles(string relativePath, [NotNullWhen(true)] out string[]? files)
    {
        // Get file provider
        IFileProvider? _fileprovider = fileProvider;
        // No file provider
        if (_fileprovider == null) { files = null!; return false; }
        // Read directory
        IDirectoryContents directoryContents = _fileprovider.GetDirectoryContents(relativePath);
        // No directory
        if (!directoryContents.Exists) { files = null!; return false; }
        // Re-usable string builder
        StringBuilder? sb = null;
        // Place here file names
        StructList20<string> names = new();
        // 
        foreach (IFileInfo fi in directoryContents)
        {
            // Directories not requested
            if (fi.IsDirectory) continue;
            // Append path + '/' + name
            string name = AppendPathAndName(relativePath, fi.Name, ref sb);
            // Add to result
            names.Add(name);
        }
        // Return
        files = names.ToArray();
        return true;
    }

    /// <summary></summary>
    public bool TryListDirectories(string relativePath, [NotNullWhen(true)] out string[]? directories)
    {
        // Get file provider
        IFileProvider? _fileprovider = fileProvider;
        // No file provider
        if (_fileprovider == null) { directories = null!; return false; }
        // Read directory
        IDirectoryContents directoryContents = _fileprovider.GetDirectoryContents(relativePath);
        // No directory
        if (!directoryContents.Exists) { directories = null!; return false; }
        // Re-usable string builder
        StringBuilder? sb = null;
        // 
        StructList20<string> names = new();
        // 
        foreach (IFileInfo fi in directoryContents)
        {
            // Files not requested
            if (!fi.IsDirectory) continue;
            // Append path + '/' + name
            string name = AppendPathAndName(relativePath, fi.Name, ref sb);
            // Add to result
            names.Add(name);
        }
        // Return
        directories = names.ToArray();
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
        if (path.Length > 0 && path[path.Length - 1] != '/') sb.Append('/');
        // Append name
        sb.Append(name);
        // Build result
        return sb.ToString();
    }

    /// <summary></summary>
    public bool TryOpen(string filename, [NotNullWhen(true)] out Stream stream)
    {
        // Get file provider
        var _fileprovider = fileProvider;
        // No file provider
        if (_fileprovider == null) { stream = null!; return false; }
        // Get info
        IFileInfo fileinfo = _fileprovider.GetFileInfo(filename);
        // No file
        if (!fileinfo.Exists || fileinfo.IsDirectory) { stream = null!; return false; }
        // Open file for reading
        stream = fileinfo.CreateReadStream();
        return true;
    }

    /// <summary></summary>
    public override int GetHashCode()
    {
        // Get file provider
        var _fileprovider = fileProvider;
        // No file provider
        if (_fileprovider == null) return 0;
        // Make hash
        int hash = _fileprovider.GetHashCode() ^ 0x5342;
        // Return hash
        return hash;
    }

    /// <summary></summary>
    public override bool Equals(object? obj)
    {
        // Cast
        if (obj is not LocalizationMsFileSystem other) return false;
        // Get references
        IFileProvider? _fileprovider1 = this.fileProvider, _fileprovider2 = other.fileProvider;
        // Compare nulls
        if (_fileprovider1 == null && _fileprovider2 == null) return true;
        if (_fileprovider1 == null || _fileprovider2 == null) return false;
        return _fileprovider1.Equals(_fileprovider2);
    }

    /// <summary>Print information</summary>
    public override string ToString() => fileProvider?.ToString() ?? GetType().Name;
}

