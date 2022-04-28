// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization;
using System.Text.Json.Nodes;
using System.IO;
using Avalanche.Utilities;
using Microsoft.Extensions.FileProviders;

/// <summary>Flattens .json files into a table of <![CDATA[KeyValuePair<string, string?>]]>.</summary>
public class LocalizationFileReaderJsonFromFileProvider : LocalizationReaderJson
{
    /// <summary>File provider</summary>
    protected IFileProvider fileProvider = null!;

    /// <summary>File to read</summary>
    public virtual IFileProvider FileProvider { get => fileProvider; set => this.AssertWritable().fileProvider = value; }

    /// <summary>Create uninitialized file</summary>
    public LocalizationFileReaderJsonFromFileProvider() : base() { }
    /// <summary>Create <paramref name="filename"/> reader</summary>
    public LocalizationFileReaderJsonFromFileProvider(IFileProvider fileProvider, string filename) : base()
    {
        this.filename = filename;
        this.fileProvider = fileProvider;
    }

    /// <summary>Open stream to associated file</summary>
    protected override JsonNode CreateNode()
    {
        // Get file info
        IFileInfo fileinfo = fileProvider.GetFileInfo(filename);
        // Open stream
        using Stream s = fileinfo.CreateReadStream();
        // Parse from stream
        JsonNode document = JsonNode.Parse(s) ?? (JsonNode)""!;
        // Return node
        return document;
    }

    /// <summary>Print information</summary>
    public override string ToString() => $"{GetType().Name}({FileName})";
}

