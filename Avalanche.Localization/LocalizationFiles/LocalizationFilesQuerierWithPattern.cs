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
public class LocalizationFilesQuerierWithPattern : ProviderBase<(string? culture, string? key), IEnumerable<ILocalizationFile>>
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
    public LocalizationFilesQuerierWithPattern(IEnumerable<ILocalizationFileFormat> fileFormats, IEnumerable<ILocalizationFileSystem> fileSystems, IEnumerable<ITemplateFormatPrintable> filePatterns)
    {
        this.fileFormats = fileFormats ?? throw new ArgumentNullException(nameof(fileFormats));
        this.fileSystems = fileSystems ?? throw new ArgumentNullException(nameof(fileSystems));
        this.filePatterns = filePatterns ?? throw new ArgumentNullException(nameof(filePatterns));
    }

    /// <summary>Create regular expression pattern of <paramref name="templateBreakdown"/>.</summary>
    /// <param name="templateBreakdown"></param>
    /// <param name="sb">String builder where to append pattern</param>
    /// <param name="parameterNameToGroupNameMap">If provided, parameter's converted group name is written here</param>
    /// <param name="args">Assigned arguments</param>
    public static void AppendPatternString(StringBuilder sb, ITemplateBreakdown templateBreakdown, IDictionary<string, string> parameterNameToGroupNameMap, IDictionary<string, object?>? args)
    {
        //
        foreach (ITemplatePart part in templateBreakdown.Parts)
        {
            // Convert placeholder into capture group
            if (part is ITemplatePlaceholderPart placeholderPart)
            {
                // Add argument
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
                    sb.Append('>');
                    string parameterPattern =
                        parameterName switch
                        {
                            "Culture" => "[^\\.]*",
                            "Name" => "[^\\.]*",
                            _ => ".*"
                        };
                    sb.Append(parameterPattern);
                    sb.Append(')');
                }
            }
            // Other part
            else
            {
                // Get part as string
                string txt = part.Unescaped.AsString();
                // Scan until '**', '*', or '?'
                int ix = 0;
                // Start of non-wildcards segment.
                int start = 0;
                while (ix < txt.Length)
                {
                    // Get char
                    char ch = txt[ix];
                    // Not wild card
                    if (ch != '*' && ch != '?') { ix++; continue; }
                    // Append previous non-wildcards segment as escaped
                    if (start < ix) { sb.Append(Regex.Escape(txt.Substring(start, ix-start))); start = ix; }
                    // Next char
                    ix++;
                    // '?'
                    if (ch == '?') { sb.Append("[^/]"); start = ix; }
                    // '**'
                    else if (ch == '*' && ix < txt.Length && txt[ix] == '*') { ix++; sb.Append(".*"); start = ix; }
                    // '*'
                    else if (ch == '*') { sb.Append("[^/]*"); start = ix; }
                }
                // Append remaining part
                if (start < ix) sb.Append(Regex.Escape(txt.Substring(start, ix-start)));
            }
        }
    }

    /// <summary>Estimate root and depth</summary>
    /// <param name="root">Root path</param>
    /// <param name="depth">Depth after root path</param>
    /// <param name="sb">Path builder</param>
    public static void EstimateRootAndDepth(ITemplateBreakdown templateBreakdown, StringBuilder sb, IDictionary<string, object?>? args, out string root, out int depth)
    {
        // Build root path here
        string _root = "";
        int _depth = 0;
        // Clear current directory
        sb.Clear();
        // Is root building completed, ends at '*', '?', '**' or '{Parameter}'
        bool rootComplete = false;
        bool gotDoubleStar = false;
        // Process parts
        foreach (ITemplatePart part in templateBreakdown.Parts)
        {
            // Convert placeholder into capture group
            if (part is ITemplatePlaceholderPart placeholderPart)
            {
                // Append argument
                if (args != null && args.TryGetValue(placeholderPart.Parameter.Unescaped.AsString(), out object? value) && value != null)
                {
                    // Append to path
                    if (!rootComplete) sb.Append(value);
                    // Next part
                    continue;
                }
                // Root completes on '{Parameter}'
                rootComplete = true;
            }
            // Other part
            else
            {
                // Get part as string
                string txt = part.Unescaped.AsString();
                // Process 'txt'
                for (int ix=0; ix<txt.Length; ix++)
                {
                    // Get char
                    char ch = txt[ix];

                    // Got wild card
                    if (ch == '*' || ch == '?')
                    {
                        // Ends root building
                        rootComplete = true;
                        // '**'
                        if (ch == '*' && ix + 1 < txt.Length && txt[ix + 1] == '*') { ix++; gotDoubleStar = true; }
                        // Next char
                        continue;
                    }

                    // Append to path builder
                    if (!rootComplete) sb.Append(ch);

                    // Got directory separator
                    if (ch == '/')
                    {
                        // Append 'sb' to '_root'
                        if (!rootComplete)
                        {
                            if (sb.Length > 0) { _root = _root.Length == 0 ? sb.ToString() : _root + sb.ToString(); sb.Clear(); }
                        }
                        // Append to depth after root
                        else
                        {
                            _depth++;
                        }
                        // Process next char
                        continue;
                    }
                }
            }
        }
        // Build result
        depth = _depth;
        root = _root.EndsWith('/') ? _root.Substring(0, _root.Length-1) : _root;
        if (gotDoubleStar) depth = int.MaxValue;
    }

    /// <summary></summary>
    public override bool TryGetValue((string? culture, string? key) query, out IEnumerable<ILocalizationFile> results)
    {
        // Get snapshots
        IList<ILocalizationFileFormat> _fileformats = ArrayUtilities.GetSnapshot(FileFormats);
        IList<ITemplateFormatPrintable> _filepatterns = ArrayUtilities.GetSnapshot(FilePatterns);
        IList<ILocalizationFileSystem> _filesystems = ArrayUtilities.GetSnapshot(FileSystems);
        // Place results here
        StructList10<ILocalizationFile> result = new();
        // Build here pattern strings
        StringBuilder sb = new StringBuilder();
        // Paramter name to pattern group name mapping
        Dictionary<string, string> parameterNameToGroupName = new Dictionary<string, string>(4);
        // Manage here files
        List<string> filepaths = new List<string>();
        // File browse queue
        List<(string, int)> queue = new List<(string, int)>();
        // Root path to browse
        string root = null!;
        // Number of levels to browse
        int depth = 0;
        // Init arguments
        Dictionary<string, object?> args = new Dictionary<string, object?>(4);
        (string @namespace, string name) = LocalizationUtilities.GetNamespaceAndName(query.key);
        if (query.key != null) args["Key"] = query.key ?? "";
        if (!string.IsNullOrEmpty(@namespace)) args["Namespace"] = @namespace;
        if (!string.IsNullOrEmpty(name)) args["Name"] = name;
        if (query.culture != null) args["Culture"] = query.culture;

        // Try each pattern
        foreach (ITemplateFormatPrintable filePattern in _filepatterns)
        {
            // Must implement ITemplateText
            if (filePattern is not ITemplateText text) continue;
            // Query for specific culture
            if (query.culture != null)
            {
                // Invariant culture "" query is queried with pattern without "{Culture}", other culture queries using patterns with "{Culture}".
                if ((query.culture == "") == text.ParameterNames.Contains("Culture")) continue;
            }
            // Create pattern
            sb.Clear();
            parameterNameToGroupName.Clear();
            sb.Append("^");
            AppendPatternString(sb, text.Breakdown, parameterNameToGroupName, args);
            sb.Append("(?<ext>\\.[^/]*)");
            sb.Append("$");
            Regex pattern = new Regex(sb.ToString());
            // Estimate root and depth
            EstimateRootAndDepth(text.Breakdown, sb, args, out root, out depth);

            // Try each file system
            foreach(ILocalizationFileSystem _filesystem in _filesystems)
            {
                // Browse files
                ListAllFiles(_filesystem, root ?? "", depth, filepaths, queue);
                // Match pattern
                foreach (string filepath in filepaths)
                {
                    // Match filepath to pattern
                    Match match = pattern.Match(filepath);
                    // Not match
                    if (!match.Success) continue;

                    // Place here file format
                    ILocalizationFileFormat? fileformat = null;
                    // Find matching file format
                    foreach (ILocalizationFileFormat _fileFormat in _fileformats) if (_fileFormat.HandlesFileName(filepath)) { fileformat = _fileFormat; break; }
                    // No file format match
                    if (fileformat == null)
                    {
                        // Get extension
                        string extension = match.Groups["ext"].Value;
                        // Get '.' index
                        int separatorIx = extension.LastIndexOf('.');
                        // Fallback extension cannot have '.'
                        if (separatorIx > 0) continue;
                        // Create fallback extension
                        fileformat = LocalizationFileFormat.Cached[extension];
                    }
                    
                    // Create file info
                    LocalizationFile localizationFile = new LocalizationFile 
                    { 
                        FileName = filepath, 
                        FileFormat = fileformat, 
                        FileSystem = _filesystem,
                    };

                    // Place here group
                    Group g, g2;
                    // Culture from query
                    if (query.culture != null) localizationFile.Culture = query.culture;
                    // Capture "{Culture}"
                    else if ((g = match.Groups["Culture"]).Success) localizationFile.Culture = g.Value;
                    // Fallback ""
                    else localizationFile.Culture = "";

                    // Key from query
                    if (query.key != null) localizationFile.Key = query.key;
                    // Capture from "{Key}"
                    else if ((g = match.Groups["Key"]).Success) localizationFile.Key = g.Value;
                    // Capture from "{Namespace}.{Name}"
                    else if ((g = match.Groups["Namespace"]).Success && (g2 = match.Groups["Name"]).Success) localizationFile.Key = g.Value + "." + g2.Value;
                    // No key
                    else continue;

                    // Make immutable
                    localizationFile.SetReadOnly();
                    // Add result
                    result.Add(localizationFile);
                }
            }
        }

        // Return
        results = result.ToArray();
        return result.Count>0;
    }

    /// <summary>List all files recursively.</summary>
    /// <param name="filesystem"></param>
    /// <param name="path">start path, "" for root. Slash '/' is directory separator, e.g. "path/path".</param>
    /// <param name="depth">Levels to browse starting from <paramref name="path"/>.</param>
    public static void ListAllFiles(ILocalizationFileSystem filesystem, string path, int depth, List<string> files, List<(string, int)> queue)
    {
        // Initialize
        files.Clear();
        queue.Clear();
        queue.Add((path, depth));
        // Visit paths
        while (queue.Count > 0)
        {
            // Next index
            int ix = queue.Count - 1;
            // Take next
            (string _path, int _depth) = queue[ix]; queue.RemoveAt(ix);
            // Get child directories
            if (_depth > 0 && filesystem.TryListDirectories(_path, out string[]? children) && children != null) foreach (string child in children) queue.Add((child, _depth - 1));
            // Get files
            if (filesystem.TryListFiles(_path, out string[]? _files) && _files != null) files.AddRange(_files);
        }
    }

    /// <summary>Print information</summary>
    public override string ToString() => $"{GetType().Name}({fileFormats}, {fileSystems}, {filePatterns})";
}

