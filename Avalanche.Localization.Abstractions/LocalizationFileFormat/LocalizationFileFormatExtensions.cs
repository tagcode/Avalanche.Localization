// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

/// <summary>Extension methods for <see cref="ILocalized{T}"/></summary>
public static class LocalizationFileFormatExtensions
{
    /// <summary>Evaluates whether <paramref name="localizationFileFormat"/> handles <paramref name="filename"/>.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HandlesFileName(this ILocalizationFileFormat localizationFileFormat, string filename)
    {
        // Get snapshot
        string[]? _extensions = localizationFileFormat?.Extensions;
        // No extensions
        if (_extensions == null) return false;
        // Find match
        foreach (string _extension in _extensions)
            if (filename.EndsWith(_extension, StringComparison.InvariantCultureIgnoreCase))
                return true;
        // No match
        return false;
    }

    /// <summary>Evaluates whether <paramref name="localizationFileFormat"/> handles <paramref name="filename"/>.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HandlesFileName(this ILocalizationFileFormat localizationFileFormat, ReadOnlySpan<char> filename)
    {
        // Get snapshot
        string[]? _extensions = localizationFileFormat?.Extensions;
        // No extensions
        if (_extensions == null) return false;
        // Find match
        foreach (string _extension in _extensions)
            if (MemoryExtensions.EndsWith(filename, _extension.AsSpan(), StringComparison.InvariantCultureIgnoreCase))
                return true;
        // No match
        return false;
    }

    /// <summary>Evaluates whether <paramref name="localizationFileFormats"/> handles <paramref name="filename"/>.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HandlesFileName(this IList<ILocalizationFileFormat> localizationFileFormats, string filename, [NotNullWhen(true)] out ILocalizationFileFormat? fileFormat)
    {
        // 'null'
        if (localizationFileFormats == null) { fileFormat = null!; return false; }
        //
        for (int i = 0; i < localizationFileFormats.Count; i++)
        {
            //
            ILocalizationFileFormat localizationFileFormat = localizationFileFormats[i];
            // Get snapshot
            string[]? _extensions = localizationFileFormat?.Extensions;
            // No extensions
            if (_extensions == null) continue;
            // Find match
            foreach (string _extension in _extensions)
                if (filename.EndsWith(_extension, StringComparison.InvariantCultureIgnoreCase))
                {
                    fileFormat = localizationFileFormat!;
                    return true;
                }
        }
        // No match
        fileFormat = null!;
        return false;
    }

    /// <summary>Evaluates whether <paramref name="localizationFileFormats"/> handles <paramref name="filename"/>.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HandlesFileName(this IList<ILocalizationFileFormat> localizationFileFormats, ReadOnlySpan<char> filename, [NotNullWhen(true)] out ILocalizationFileFormat? fileFormat)
    {
        // 'null'
        if (localizationFileFormats == null) { fileFormat = null!; return false; }
        //
        for (int i = 0; i < localizationFileFormats.Count; i++)
        {
            //
            ILocalizationFileFormat localizationFileFormat = localizationFileFormats[i];
            // Get snapshot
            string[]? _extensions = localizationFileFormat?.Extensions;
            // No extensions
            if (_extensions == null) continue;
            // Find match
            foreach (string _extension in _extensions)
                if (MemoryExtensions.EndsWith(filename, _extension.AsSpan(), StringComparison.InvariantCultureIgnoreCase))
                {
                    fileFormat = localizationFileFormat!;
                    return true;
                }
        }
        // No match
        fileFormat = null!;
        return false;
    }

    /// <summary>Evaluates whether <paramref name="localizationFileFormats"/> handles <paramref name="filename"/>.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HandlesFileName(this IList<ILocalizationFileFormat> localizationFileFormats, ReadOnlySpan<char> filename, [NotNullWhen(true)] out ILocalizationFileFormat? fileFormat, out string extension)
    {
        // 'null'
        if (localizationFileFormats == null) { fileFormat = null!; extension = null!;  return false; }
        //
        for (int i = 0; i < localizationFileFormats.Count; i++)
        {
            //
            ILocalizationFileFormat localizationFileFormat = localizationFileFormats[i];
            // Get snapshot
            string[]? _extensions = localizationFileFormat?.Extensions;
            // No extensions
            if (_extensions == null) continue;
            // Find match
            foreach (string _extension in _extensions)
                if (MemoryExtensions.EndsWith(filename, _extension.AsSpan(), StringComparison.InvariantCultureIgnoreCase))
                {
                    fileFormat = localizationFileFormat!;
                    extension = _extension;
                    return true;
                }
        }
        // No match
        fileFormat = null!;
        extension = null!;
        return false;
    }

    /// <summary>Evaluates whether <paramref name="localizationFileFormat"/> contains <paramref name="extensions"/>.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Contains(this ILocalizationFileFormat localizationFileFormat, string extensions)
    {
        // Get snapshot
        string[]? _extensions = localizationFileFormat?.Extensions;
        // No extensions
        if (_extensions == null) return false;
        // Find match
        foreach (string _extension in _extensions)
            if (extensions.Equals(_extension, StringComparison.InvariantCultureIgnoreCase))
                return true;
        // No match
        return false;
    }

    /// <summary>Evaluates whether <paramref name="localizationFileFormat"/> contains <paramref name="extensions"/>.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Contains(this ILocalizationFileFormat localizationFileFormat, ReadOnlySpan<char> extensions)
    {
        // Get snapshot
        string[]? _extensions = localizationFileFormat?.Extensions;
        // No extensions
        if (_extensions == null) return false;
        // Find match
        foreach (string _extension in _extensions)
            if (MemoryExtensions.Equals(extensions, _extension.AsSpan(), StringComparison.InvariantCultureIgnoreCase))
                return true;
        // No match
        return false;
    }

    /// <summary>Evaluates whether <paramref name="localizationFileFormats"/> Contains <paramref name="extensions"/>.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Contains(this IList<ILocalizationFileFormat> localizationFileFormats, string extensions, [NotNullWhen(true)] out ILocalizationFileFormat? fileFormat)
    {
        // 'null'
        if (localizationFileFormats == null) { fileFormat = null!; return false; }
        //
        for (int i = 0; i < localizationFileFormats.Count; i++)
        {
            //
            ILocalizationFileFormat localizationFileFormat = localizationFileFormats[i];
            // Get snapshot
            string[]? _extensions = localizationFileFormat?.Extensions;
            // No extensions
            if (_extensions == null) continue;
            // Find match
            foreach (string _extension in _extensions)
                if (extensions.Equals(_extension, StringComparison.InvariantCultureIgnoreCase))
                {
                    fileFormat = localizationFileFormat!;
                    return true;
                }
        }
        // No match
        fileFormat = null!;
        return false;
    }

    /// <summary>Evaluates whether <paramref name="localizationFileFormats"/> contains <paramref name="extensions"/>.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Contains(this IList<ILocalizationFileFormat> localizationFileFormats, ReadOnlySpan<char> extensions, [NotNullWhen(true)] out ILocalizationFileFormat? fileFormat)
    {
        // 'null'
        if (localizationFileFormats == null) { fileFormat = null!; return false; }
        //
        for (int i = 0; i < localizationFileFormats.Count; i++)
        {
            //
            ILocalizationFileFormat localizationFileFormat = localizationFileFormats[i];
            // Get snapshot
            string[]? _extensions = localizationFileFormat?.Extensions;
            // No extensions
            if (_extensions == null) continue;
            // Find match
            foreach (string _extension in _extensions)
                if (MemoryExtensions.Equals(extensions, _extension.AsSpan(), StringComparison.InvariantCultureIgnoreCase))
                {
                    fileFormat = localizationFileFormat!;
                    return true;
                }
        }
        // No match
        fileFormat = null!;
        return false;
    }

}
