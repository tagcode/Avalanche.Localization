// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization;
using Avalanche.Localization.Internal;
using Avalanche.Utilities;
using Avalanche.Utilities.Provider;

/// <summary>Queries lines from file sources.</summary>
public class LocalizationFileLineQuerier : ProviderBase<(string? culture, string? key), IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>>>
{
    /// <summary>Singleton</summary>
    static LocalizationLineFileReader instance = new LocalizationLineFileReader();
    /// <summary>Singleton</summary>
    public static LocalizationLineFileReader Instance => instance;

    /// <summary>File querier</summary>
    protected IEnumerable<IProvider<(string? culture, string? key), IEnumerable<ILocalizationFile>>> fileProviders;
    /// <summary>Yields provider that returns file's lines, typically cached.</summary>
    protected IProvider<ILocalizationFile, IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>>> fileLinesReaderProvider;
    /// <summary></summary>
    public virtual IEnumerable<IProvider<(string? culture, string? key), IEnumerable<ILocalizationFile>>> FileProviders => fileProviders;
    /// <summary>Provider that returns file's lines, typically cached.</summary>
    public virtual IProvider<ILocalizationFile, IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>>> FileLinesReader => fileLinesReaderProvider;

    /// <summary></summary>
    public LocalizationFileLineQuerier(IEnumerable<IProvider<(string? culture, string? key), IEnumerable<ILocalizationFile>>> fileProviders, IProvider<ILocalizationFile, IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>>> fileLinesReader) 
    {
        this.fileProviders = fileProviders ?? throw new ArgumentNullException(nameof(fileProviders));
        this.fileLinesReaderProvider = fileLinesReader  ?? throw new ArgumentNullException(nameof(fileLinesReader));
    }

    /// <summary>Try query lines.</summary>
    /// <param name="query">query parameters, null means no constrictions.</param>
    /// <exception cref="Exception"></exception>
    public override bool TryGetValue((string? culture, string? key) query, out IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>> lines)
    {
        // Get snapshot
        IList<IProvider<(string? culture, string? key), IEnumerable<ILocalizationFile>>> _fileProviders = ArrayUtilities.GetSnapshot(FileProviders);
        var _fileLinesProvider = FileLinesReader;
        // List used for applying filter
        StructList16<IEnumerable<KeyValuePair<string, MarkedText>>> result = new();
        // Init
        lines = null!;
        // Extract all namespaces
        (string? @namespace, string? name)[] namespaces = LocalizationUtilities.GetAllNamespaceAndNameDotPermutations(query.key);
        // Query for files
        foreach(var _fileProvider in _fileProviders)
        {
            foreach ((string? @namespace, string? name) in namespaces)
            {
                // No namespace, but something was queried
                if (query.key != null && @namespace == null) continue;
                // Extract namespace
                (string? culture, string? key) fileQuery = (query.culture, @namespace);
                // Try read files
                if (!_fileProvider.TryGetValue(fileQuery, out IEnumerable<ILocalizationFile>? _files) || _files == null) continue;
                // Visit each file
                foreach (var _file in _files)
                {
                    // Read lines
                    if (!_fileLinesProvider.TryGetValue(_file, out IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>> _lines)) continue;
                    // No lines
                    if (_lines is ICollection<IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>>> collection && collection.Count == 0) continue;
                    // Concat unfiltered
                    if (query.culture == null && query.key == null) result.AddRange(_lines);
                    // Append filtered
                    else
                    {
                        // Filter
                        foreach (var line in _lines)
                        {
                            // Read values
                            line.ReadCultureKey(out MarkedText culture, out MarkedText key);
                            // Disqualify with culture criteria
                            if (query.culture != null && query.culture != culture.AsString) continue;
                            // Disqualify with key criteria
                            if (query.key != null && query.key != key.AsString) continue;
                            // Add to result
                            result.Add(line);
                        }
                        // Append
                        if (result.Count > 0) lines = lines == null ? _lines : lines.Concat(result.ToArray());
                    }
                }
            }
        }

        // Return
        lines = result.ToArray();
        return result.Count > 0;
    }

}
