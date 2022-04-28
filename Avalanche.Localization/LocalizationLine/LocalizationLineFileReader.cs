// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization;
using Avalanche.Utilities;
using Avalanche.Utilities.Provider;

/// <summary>Reads lines from a file. Combines file provider and file formats.</summary>
public class LocalizationLineFileReader : ProviderBase<ILocalizationFile, IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>>>
{
    /// <summary>Singleton</summary>
    static LocalizationLineFileReader instance = new LocalizationLineFileReader();
    /// <summary>Singleton</summary>
    public static LocalizationLineFileReader Instance => instance;
 
    /// <summary></summary>
    public LocalizationLineFileReader() { }

    /// <summary>Try to read localization lines from <paramref name="file"/>.</summary>
    /// <exception cref="Exception"></exception>
    public override bool TryGetValue(ILocalizationFile file, out IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>> lines)
    {
        // Cast localization format
        if (file.FileFormat is not ILocalizationLineFileFormat lineFormat) { lines = null!; return false; }
        // Try open file
        if (!file.TryOpen(out Stream? stream)) { lines = null!; return false; }
        // 
        try
        {
            // Read lines
            lines = lineFormat.Read(stream);
            // Add filename for diagnostics
            if (file.FileName != null) lines = lines.AnnotateFilename(file.FileName);
            // Return
            return true;
        } finally
        {
            stream.Dispose();
        }
    }
}
