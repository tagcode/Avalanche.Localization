// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Avalanche.Localization.Internal;
using Avalanche.Template;
using Avalanche.Utilities;
using Avalanche.Utilities.Provider;

/// <summary>Queries files using template patterns. This provider responds only when culture and key are supplied.</summary>
[Obsolete("Deprecated, this version does not support wildcards.")]
public class LocalizationFilesQuerierWithPatternOld : ProviderBase<(string? culture, string? key), IEnumerable<ILocalizationFile>>
{
    /// <summary>Acknowledged localization file formats.</summary>
    protected IEnumerable<ILocalizationFileFormat> fileFormats = null!;
    /// <summary>File system file readers</summary>
    protected IEnumerable<ILocalizationFileSystem> fileSystems = null!;
    /// <summary>File name patterns without extension, e.g. "{Culture}/{Namespace}". Template text can uses parameters: "Culture", "Namespace", "Name", "Key"</summary>
    protected IEnumerable<ITemplateFormatPrintable> filePatterns = null!;

    /// <summary>Acknowledged localization file formats.</summary>
    public virtual IEnumerable<ILocalizationFileFormat> FileFormats => fileFormats;
    /// <summary>File system file readers</summary>
    public virtual IEnumerable<ILocalizationFileSystem> FileSystems => fileSystems;
    /// <summary>File name patterns without extension, e.g. "{Culture}/{Namespace}". Template text can uses parameters: "Culture", "Namespace", "Name", "Key"</summary>
    public virtual IEnumerable<ITemplateFormatPrintable> FilePatterns => filePatterns;

    /// <summary></summary>
    public LocalizationFilesQuerierWithPatternOld(IEnumerable<ILocalizationFileFormat> fileFormats, IEnumerable<ILocalizationFileSystem> fileSystems, IEnumerable<ITemplateFormatPrintable> filePatterns)
    {
        this.fileFormats = fileFormats ?? throw new ArgumentNullException(nameof(fileFormats));
        this.fileSystems = fileSystems ?? throw new ArgumentNullException(nameof(fileSystems));
        this.filePatterns = filePatterns ?? throw new ArgumentNullException(nameof(filePatterns));
    }

    /// <summary></summary>
    protected virtual bool TryMatchPattern((string? culture, string? key) query, out IEnumerable<ILocalizationFile> files)
    {
        // Get snapshots
        IList<ILocalizationFileFormat> _fileformats = ArrayUtilities.GetSnapshot(FileFormats);
        IList<ILocalizationFileSystem> _filesystems = ArrayUtilities.GetSnapshot(FileSystems);
        IList<ITemplateFormatPrintable> _filepatterns = ArrayUtilities.GetSnapshot(FilePatterns);

        // No patterns
        if (_filepatterns.Count == 0 || _filesystems.Count == 0 || _fileformats.Count == 0) { files = Array.Empty<ILocalizationFile>(); return true; }

        // Print file extension pattern
        StringBuilder sb = new();
        List<string> groupNames = new List<string>(_filepatterns.Count);
        sb.Append("(?:");
        for (int i=0; i<_fileformats.Count; i++)
        {
            ILocalizationFileFormat _format = _fileformats[i];        
            if (i > 0) sb.Append("|");
            string groupName = "ext" + i;
            groupNames.Add(groupName);
            sb.Append("(?<");
            sb.Append(groupName);
            sb.Append(">");
            for (int j = 0; j < _format.Extensions.Length; j++)
            {
                if (j > 0) sb.Append("|");
                sb.Append(Regex.Escape(_format.Extensions[j]));
            }
            sb.Append(")");
        }
        string unknownExtension = "unknownExtension";
        sb.Append("|(?<");
        sb.Append(unknownExtension);
        sb.Append(">\\.[^\\.]*)");
        sb.Append(")");
        groupNames.Add(unknownExtension);
        string extensionsPattern = sb.ToString();
        int extensionCount = groupNames.Count;

        // Place here regex patterns
        List<Regex> patterns = new List<Regex>(_filepatterns.Count);
        Dictionary<string, string> parameterNameToGroupNameMap = new Dictionary<string, string>(groupNames.Count + 5);

        // Extract Namespace.Name
        Dictionary<string, object?>? args = null!;
        if (query.key != null || query.culture != null)
        {
            args = new Dictionary<string, object?>(4);
            if (query.key != null)
            {
                (string @namespace, string name) = LocalizationUtilities.GetNamespaceAndName(query.key);
                args["Key"] = query.key ?? "";
                args["Namespace"] = @namespace;
                args["Name"] = name;
            }
            if (query.culture != null) args["Culture"] = query.culture ?? "";
        }

        // Create regex patterns
        foreach (ITemplateFormatPrintable _filepattern in _filepatterns)
        {
            //
            parameterNameToGroupNameMap.Clear();
            foreach (string groupName in groupNames) parameterNameToGroupNameMap["_Reserved_" + groupName] = groupName;
            // Cast
            if (_filepattern is not ITemplateText templateText) continue;
            // Create pattern string
            string patternString = "^" + CreatePatternString(templateText.Breakdown, parameterNameToGroupNameMap, args) + extensionsPattern + "$";
            // Compile pattern
            Regex pattern = new Regex(patternString, RegexOptions.Compiled | RegexOptions.Singleline);
            // Add
            patterns.Add(pattern);
        }
        // No patterns
        if (patterns.Count == 0) { files = Array.Empty<ILocalizationFile>(); return true; }

        // Place results here
        StructList10<ILocalizationFile> result = new();

        // Iterate file-systems
        foreach (ILocalizationFileSystem filesystem in _filesystems)
        {
            // Read all files
            foreach (string filepath in filesystem.ListAllFiles(""))
            {
                // Match to pattern
                foreach (Regex pattern in patterns)
                {
                    // Get match
                    Match match = pattern.Match(filepath);
                    // No match
                    if (!match.Success) continue;
                    // Culture and key
                    Group? cultureGroup = null, keyGroup = null;
                    // Capture 
                    string? capturedCulture = query.culture ?? ((cultureGroup = match.Groups["Culture"]).Success ? cultureGroup.Value : null);
                    string? capturedKey = query.key ?? ((keyGroup = match.Groups["Key"]).Success ? keyGroup.Value : null);
                    // Disqualify by culture
                    if (query.culture != null && query.culture != capturedCulture) continue;
                    // Disqualify by key
                    if (query.key != null && query.key != capturedKey) continue;
                    // Find which file format matched
                    ILocalizationFileFormat? fileFormat = null;
                    for (int i = 0; i < extensionCount; i++)
                    {
                        Group formatGroup = match.Groups[groupNames[i]];
                        if (!formatGroup.Success) continue;
                        fileFormat = i == _fileformats.Count ? LocalizationFileFormat.Cached[formatGroup.Value] : _fileformats[i];
                        break;
                    }
                    // No file format matched?? Regex is broken.
                    if (fileFormat == null) continue;
                    // Create file info
                    ILocalizationFile localizationFile = new LocalizationFile { FileName = filepath, FileFormat = fileFormat, FileSystem = filesystem, Culture = capturedCulture ?? "", Key = capturedKey! }.SetReadOnly();
                    //
                    result.Add(localizationFile);
                }
            }
        }
        // Done
        files = result.ToArray();
        return true;
    }

    /// <summary>Create regular expression pattern of <paramref name="templateBreakdown"/>.</summary>
    /// <param name="templateBreakdown"></param>
    /// <returns></returns>
    protected string CreatePatternString(ITemplateBreakdown templateBreakdown, IDictionary<string, string>? parameterNameToGroupNameMap = null, IDictionary<string, object?>? args = null)
    {
        // 
        StringBuilder sb = new();
        if (parameterNameToGroupNameMap == null) parameterNameToGroupNameMap = new Dictionary<string, string>();
        //
        foreach (ITemplatePart part in templateBreakdown.Parts)
        {
            // Convert placeholder into capture group
            if (part is ITemplatePlaceholderPart placeholderPart)
            {
                // Add value
                if (args != null && args.TryGetValue(placeholderPart.Parameter.Unescaped.AsString(), out object? value) && value != null)
                {
                    sb.Append(value);
                }
                // Add pattern
                else
                {
                    // Get parameter name
                    string parameterName = placeholderPart.Parameter.Unescaped.AsString();
                    // Convert to group name
                    string groupName = TemplateArgumentExtractExtensions.ConvertParameterNameToGroupName(parameterName, parameterNameToGroupNameMap);
                    // Open capture group
                    sb.Append("(?<");
                    sb.Append(groupName);
                    sb.Append(">");
                    string parameterPattern =
                        parameterName switch
                        {
                            "Culture" => "[^\\.]*",
                            "Name" => "[^\\.]*",
                            _ => ".*"
                        };
                    sb.Append(parameterPattern);
                    sb.Append(")");
                }
            }
            // Other part
            else
            {
                // Append part
                sb.Append(Regex.Escape(part.Unescaped.AsString()));
            }
        }
        // 
        string pattern = sb.ToString();
        // Return 
        return pattern;
    }

    /// <summary></summary>
    public override bool TryGetValue((string? culture, string? key) query, out IEnumerable<ILocalizationFile> files)
    {
        // Culture and/or key not supplied
        if (query.culture == null || query.key == null) return TryMatchPattern(query, out files);
        // Get snapshots
        IList<ILocalizationFileFormat> _fileformats = ArrayUtilities.GetSnapshot(FileFormats);
        IList<ILocalizationFileSystem> _filesystems = ArrayUtilities.GetSnapshot(FileSystems);
        IList<ITemplateFormatPrintable> _filepatterns = ArrayUtilities.GetSnapshot(FilePatterns);
        // Number of read success counts
        int okCount = 0, failedCount = 0;
        // Place results here
        StructList10<ILocalizationFile> result = new();
        // Extract Namespace.Name
        (string @namespace, string name) = LocalizationUtilities.GetNamespaceAndName(query.key);
        Dictionary<string, object?> args = new Dictionary<string, object?>(4);
        args["Culture"] = query.culture ?? "";
        args["Key"] = query.key ?? "";
        args["Namespace"] = @namespace;
        args["Name"] = name;
        // Place filename stems here
        StructList6<(string path, string stem)> filenames = new();
        // Print filename stems
        for (int i=0; i<_filepatterns.Count; i++)
        {
            // Get pattern
            ITemplateFormatPrintable _filepattern = _filepatterns[i];
            //
            if (string.IsNullOrEmpty(query.culture) == _filepattern.ParameterNames.Contains("Culture")) continue;
            // Print filename stem, e.g. "en/Avalanche.FileSystem"
            string fileprint = _filepattern.Print(null, args);
            // Trim off initial '/' 
            if (fileprint.StartsWith('/')) fileprint = fileprint.Substring(1);
            // Separator ix
            int lastSeparatorIx = -1;
            // Find separator
            for (int j=fileprint.Length-1; j>=0; j--)
            {
                // Get char
                char ch = fileprint[j];
                // Not separator
                if (ch != '/' && ch != '\\') continue;
                // Got separator
                lastSeparatorIx = j;
                break;
            }
            // Split to path and filename
            (string path, string stem) filename = lastSeparatorIx < 0 ? ("", fileprint) : (fileprint.Substring(0, lastSeparatorIx), fileprint/*.Substring(lastSeparatorIx+1)*/);
            // Add to patterns
            filenames.Add(filename);
        }

        // Iterate file-systems
        foreach (ILocalizationFileSystem filesystem in _filesystems)
        {
            // Visit all stems
            for (int i = 0; i < filenames.Count; i++)
            {
                // Split to path and filename
                (string path, string stem) = filenames[i];
                // Try read files
                if (!filesystem.TryListFiles(path, out string[]? _filenames) || _filenames == null) { failedCount++; continue; } else okCount++;
                // Match each file
                foreach (string _filename in _filenames)
                {
                    // Place here revised 
                    string __filename = _filename, extension = null!;
                    // No match
                    if (!_filename.StartsWith(stem)) continue;
                    // No extension match
                    if (!_fileformats.HandlesFileName(__filename, out ILocalizationFileFormat? fileFormat, out extension) || _filename.Substring(0, _filename.Length-extension.Length) != stem)
                    {
                        extension = System.IO.Path.GetExtension(__filename);
                        __filename = _filename.Substring(0, _filename.Length - extension.Length);
                        // No match
                        if (!__filename.Equals(stem)) continue;
                        fileFormat = LocalizationFileFormat.Cached[extension];
                    }
                    // Create file info
                    ILocalizationFile localizationFile = new LocalizationFile { FileName = _filename, FileFormat = fileFormat, FileSystem = filesystem, Culture = query.culture!, Key = query.key! }.SetReadOnly();
                    // Add result
                    result.Add(localizationFile);
                }
            }
        }
        // Nothing was queried (true but empty result)
        if (okCount == 0 && failedCount == 0) { files = Array.Empty<ILocalizationFile>(); return false; }
        // All queries to filesystems failed (false result)
        if (okCount == 0 && failedCount > 0) { files = null!; return false; }
        // All or some filesystems returned results
        files = result.ToArray();
        return true;
    }

    /// <summary>Print information</summary>
    public override string ToString() => $"{GetType().Name}({fileFormats}, {fileSystems}, {filePatterns})";
}

