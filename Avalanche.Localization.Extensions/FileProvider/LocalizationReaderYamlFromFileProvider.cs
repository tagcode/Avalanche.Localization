// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization;
using System.IO;
using Avalanche.Utilities;
using Microsoft.Extensions.FileProviders;
using YamlDotNet.Helpers;
using YamlDotNet.RepresentationModel;

/// <summary>Flattens .yml files into a table of <![CDATA[KeyValuePair<string, string?>]]>.</summary>
public class LocalizationReaderYamlFromFileProvider : LocalizationReaderYaml
{
    /// <summary>File provider</summary>
    protected IFileProvider fileProvider = null!;

    /// <summary>File to read</summary>
    public virtual IFileProvider FileProvider { get => fileProvider; set => this.AssertWritable().fileProvider = value; }

    /// <summary>Create uninitialized file</summary>
    public LocalizationReaderYamlFromFileProvider() : base() { }
    /// <summary>Create <paramref name="filename"/> reader</summary>
    public LocalizationReaderYamlFromFileProvider(IFileProvider fileProvider, string filename) : base()
    {
        this.filename = filename;
        this.fileProvider = fileProvider;
    }

    /// <summary>Open stream to associated file</summary>
    protected override IEnumerable<YamlNode> CreateNodes()
    {
        // Get file info
        IFileInfo fileinfo = fileProvider.GetFileInfo(filename);
        // Open stream
        using Stream s = fileinfo.CreateReadStream();
        // Create file reader
        using var reader = new StreamReader(s);
        // Create yaml stream
        YamlStream yamlStream = new YamlStream();
        // Load file into stream
        yamlStream.Load(reader);
        // Return root nodes
        return yamlStream.Documents.Select(d => d.RootNode).ToArray();
    }

    /// <summary>Print information</summary>
    public override string ToString() => $"{GetType().Name}({FileName})";
}

