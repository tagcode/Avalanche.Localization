// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text;

/// <summary>FileSystem for embedded resources of <see cref="Assembly"/>ies.</summary>
public class LocalizationFileSystemEmbedded : ILocalizationFileSystem
{
    /// <summary></summary>
    static LocalizationFileSystemEmbedded appDomain = new LocalizationFileSystemEmbedded(System.AppDomain.CurrentDomain);
    /// <summary></summary>
    public static LocalizationFileSystemEmbedded AppDomain => appDomain;

    /// <summary></summary>
    protected IEnumerable<Assembly> assemblies;
    /// <summary></summary>
    public virtual IEnumerable<Assembly> Assemblies => assemblies;

    /// <summary></summary>
    public LocalizationFileSystemEmbedded(AppDomain appDomain)
    {
        this.assemblies = new AppDomainEnumerable(appDomain);
    }

    /// <summary></summary>
    public LocalizationFileSystemEmbedded(Assembly assembly)
    {
        this.assemblies = new Assembly[] { assembly ?? throw new ArgumentNullException(nameof(assembly)) };
    }

    /// <summary></summary>
    public LocalizationFileSystemEmbedded(params Assembly[] assemblies)
    {
        this.assemblies = assemblies ?? throw new ArgumentNullException(nameof(assemblies));
    }

    /// <summary></summary>
    public LocalizationFileSystemEmbedded(IEnumerable<Assembly> assemblyEnumerable)
    {
        this.assemblies = assemblyEnumerable ?? throw new ArgumentNullException(nameof(assemblyEnumerable));
    }

    /// <summary></summary>
    public bool TryListDirectories(string path, [NotNullWhen(true)] out string[]? directories)
    {
        // List assemblies
        if (path == "")
        {
            // Get snapshot
            IEnumerable<Assembly> _assemblies = this.Assemblies;
            // Init result list
            SortedSet<string> list = new(StringComparer.Ordinal);
            foreach (Assembly assembly in _assemblies)
            {
                if (assembly.IsDynamic) continue;
                string? name = assembly.GetName().Name;
                if (name == null) continue;
                list.Add(name);
            }
            // Return list
            directories = list.ToArray();
            return true;
        }

        // Find '/'
        int ix = path.IndexOf('/');
        if (ix > 0)
        {
            // Get assembly name
            ReadOnlySpan<char> assemblyName = path.AsSpan().Slice(0, ix);
            // Get assembly
            if (TryGetAssembly(assemblyName, out Assembly? assembly))
            {
                // Got dynamic assembly
                if (assembly.IsDynamic) { directories = null!; return false; }
                // Got assembly, but no subdirectories
                directories = Array.Empty<string>();
                return true;
            }
        }
        // No match
        directories = null!;
        return false;
    }

    /// <summary></summary>
    public bool TryListFiles(string path, [NotNullWhen(true)] out string[]? files)
    {
        // Place here assembly name
        ReadOnlySpan<char> assemblyName = default;
        // Get separator
        int ix = path.IndexOf('/');
        // Got separator
        if (ix >= 0) {
            // Last char
            if (ix == path.Length - 1) assemblyName = path.AsSpan().Slice(0, ix);
            // Should not have separator
            else { files = null; return false; }
        } else assemblyName = path.AsSpan();
        // Get assembly
        if (!TryGetAssembly(assemblyName, out Assembly? assembly)) { files = null; return false; }
        // Dynamic assembly does not have resources
        if (assembly == null || assembly.IsDynamic) { files = null; return false; }
        // Get manifest
        string[] manifest = assembly.GetManifestResourceNames();
        // Get count
        int count = manifest.Length;
        // No resources
        if (count == 0) { files = Array.Empty<string>(); return true; }
        // Allocate result
        files = new string[manifest.Length];
        // Re-usable string builder
        StringBuilder? sb = null;
        // Adapt each
        for(int i=0; i<count; i++) files[i] = AppendPathAndName(assemblyName, manifest[i], ref sb);
        // Done
        return true;
    }

    /// <summary>Append <paramref name="path"/> to <paramref name="name"/> with separator '/'.</summary>
    /// <param name="sb">Recyclable string builder</param>
    /// <returns><paramref name="path"/> + '/' + <paramref name="name"/></returns>
    public static string AppendPathAndName(ReadOnlySpan<char> path, ReadOnlySpan<char> name, ref StringBuilder? sb)
    {
        // Allocate/clear sb
        if (sb == null) sb = new StringBuilder(128); else sb.Clear();
        // Append path
        sb.Append(path);
        // Append separator
        sb.Append('/');
        // Append resource name
        sb.Append(name);
        // Build result
        return sb.ToString();
    }

    /// <summary>Try find <paramref name="assemblyName"/>.</summary>
    protected virtual bool TryGetAssembly(ReadOnlySpan<char> assemblyName, [NotNullWhen(true)] out Assembly? assembly)
    {
        // Get snapshot
        IEnumerable<Assembly> _assemblies = this.Assemblies;
        // Iterate
        foreach (Assembly _assembly in _assemblies)
        {
            // Dynamic assembly does not have resources
            if (_assembly == null || _assembly.IsDynamic) continue;
            // Get assembly name
            ReadOnlySpan<char> _assemblyName = _assembly.GetName().Name.AsSpan();
            // Not match
            if (!MemoryExtensions.SequenceEqual(assemblyName, _assemblyName)) continue;
            // Match
            assembly = _assembly; return true;
        }
        // No match
        assembly = null!;
        return false;
    }

    /// <summary></summary>
    public bool TryOpen(string resourcePath, [NotNullWhen(true)] out Stream stream)
    {
        // Get separator
        int ix = resourcePath.IndexOf('/');
        // No separator
        if (ix < 0) { stream = null!; return false; }
        // Get assembly name
        ReadOnlySpan<char> assemblyName = resourcePath.AsSpan().Slice(0, ix);
        // Get assembly
        if (!TryGetAssembly(assemblyName, out Assembly? assembly)) { stream = null!; return false; }
        // Dynamic assembly does not have resources
        if (assembly == null || assembly.IsDynamic) { stream = null!; return false; }
        // Get resource name
        string resourceName = resourcePath.Substring(ix + 1);
        // Open stream
        stream = assembly.GetManifestResourceStream(resourceName)!;
        return stream != null;
    }

    /// <summary></summary>
    public override string ToString() => $"LocalizationFileSystemEmbedded({Assemblies.ToString()})";

    /// <summary></summary>
    class AppDomainEnumerable : IEnumerable<Assembly>
    {
        /// <summary></summary>
        protected AppDomain appDomain;

        /// <summary></summary>
        public AppDomainEnumerable(AppDomain appDomain)
        {
            this.appDomain = appDomain ?? throw new ArgumentNullException(nameof(appDomain));
        }

        /// <summary></summary>
        public IEnumerator<Assembly> GetEnumerator()
        {
            Assembly[] assemblies = appDomain.GetAssemblies();
            return ((IEnumerable<Assembly>)assemblies).GetEnumerator();
        }

        /// <summary></summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            Assembly[] assemblies = appDomain.GetAssemblies();
            return assemblies.GetEnumerator();
        }
    }

}
