// Copyright (c) Toni Kalajainen 2022
using Avalanche.Utilities;

namespace Avalanche.Localization.Internal;

/// <summary>Localization utilities</summary>
public static class LocalizationUtilities
{
    /// <summary>Separate namespace. The last dot '.' operates as separator between namespace and name parts of p<paramref name="key"/>.</summary>
    /// <param name="key">Key, e.g. "Application.Namespace.Name"</param>
    /// <returns>Namespace and name, e.g. "Application.Namespace" and "Name". If there is no '.' then key will be returned as name, and namespace is empty.</returns>
    public static string? GetNamespace(string? key)
    {
        // 'null'
        if (key == null) return null;
        // Get index of last dot
        int dotIx = key.LastIndexOf('.');
        // Return namespace and name
        return dotIx <= 0 ? "" : key.Substring(0, dotIx);
    }

    /// <summary>Separate namespace and name. The last dot '.' operates as separator between namespace and name parts of p<paramref name="key"/>.</summary>
    /// <param name="key">Key, e.g. "Application.Namespace.Name"</param>
    /// <returns>Namespace and name, e.g. "Application.Namespace" and "Name". If there is no '.' then key will be returned as name, and namespace is empty.</returns>
    public static (string @namespace, string name) GetNamespaceAndName(string? key)
    {
        // 'null'
        if (key == null) return ("", "");
        // Get index of last dot
        int dotIx = key.LastIndexOf('.');
        // Return namespace and name
        return (dotIx <= 0 ? "" : key.Substring(0, dotIx),
                dotIx < 0 ? key : dotIx >= key.Length - 1 ? "" : key.Substring(dotIx + 1));
    }

    /// <summary>Null line singleton</summary>
    static (string? @namespace, string? name)[] nullLine = new (string? @namespace, string? name)[] { (null, null) };

    /// <summary>
    /// Get all "namespace1.namespace2.name" dot permutations, e.g. "namespace1.namespace2"+"name", "namespace1"+"namespace2.name".
    /// 
    /// If there is no '.' then (null, key) is returned.
    /// 
    /// Enumeration starts at last occuring '.' index and proceeds towards the first.
    /// </summary>
    /// <returns>All "Namespace.Name" permutations.</returns>
    public static (string? @namespace, string? name)[] GetAllNamespaceAndNameDotPermutations(string? key)
    {
        // 'null'
        if (key == null) return nullLine;
        // Get index of last dot
        int dotIx = key.Length - 1;
        // Place result here
        StructList6<(string? @namespace, string? name)> list = new();
        // Iterate all '.' separators, starting from last
        for (dotIx = key.LastIndexOf('.', dotIx); dotIx >= 0; dotIx = dotIx == 0 ? -1 : key.LastIndexOf('.', dotIx - 1))
        {
            // Create (namespace, name)
            (string @namespace, string name) line = (dotIx <= 0 ? "" : key.Substring(0, dotIx), dotIx < 0 ? key : dotIx >= key.Length - 1 ? "" : key.Substring(dotIx + 1));
            // Add to result
            list.Add(line);
        }
        // Add (null, key)
        list.Add((null, key));
        // Return array
        return list.ToArray();
    }


    /// <summary>Separate namespace and name. The last dot '.' operates as separator between namespace and name parts of p<paramref name="key"/>.</summary>
    /// <param name="key">Key, e.g. "Application.Namespace.Name"</param>
    /// <returns>Namespace and name, e.g. "Application.Namespace" and "Name". If there is no '.' then key will be returned as name, and namespace is empty.</returns>
    public static (ReadOnlyMemory<char> @namespace, ReadOnlyMemory<char> name) GetNamespaceAndName(ReadOnlyMemory<char> key)
    {
        // Get span
        ReadOnlySpan<char> keySpan = key.Span;
        // Get index of last dot
        int dotIx = -1;
        for (int i = keySpan.Length - 1; i >= 0; i--) if (keySpan[i] == '.') { dotIx = i; break; }
        // Return namespace and name
        return (dotIx <= 0 ? default : key.Slice(0, dotIx),
                dotIx < 0 ? key : dotIx >= key.Length - 1 ? default : key.Slice(dotIx + 1));
    }

    /// <summary>Separate namespace and name. The last dot '.' operates as separator between namespace and name parts of p<paramref name="key"/>.</summary>
    /// <param name="key">Key, e.g. "Application.Namespace.Name"</param>
    /// <returns>Namespace and name, e.g. "Application.Namespace" and "Name". If there is no '.' then key will be returned as name, and namespace is empty.</returns>
    public static void GetNamespaceAndName(ReadOnlySpan<char> key, out ReadOnlySpan<char> @namespace, out ReadOnlySpan<char> name)
    {
        // Get index of last dot
        int dotIx = -1;
        for (int i = key.Length - 1; i >= 0; i--) if (key[i] == '.') { dotIx = i; break; }
        // Return namespace and name
        @namespace = dotIx <= 0 ? default : key.Slice(0, dotIx);
        name = dotIx < 0 ? key : dotIx >= key.Length - 1 ? default : key.Slice(dotIx + 1);
    }

}

