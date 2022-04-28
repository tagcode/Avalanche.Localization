// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization;
using System.IO;
using System.Xml.Linq;
using Avalanche.Utilities;
using Microsoft.Extensions.FileProviders;

/// <summary>Flattens .xml files into a table of <![CDATA[KeyValuePair<string, string?>]]>.</summary>
public class LocalizationReaderXmlFromFileProvider : LocalizationReaderXml
{
    /// <summary>File provider</summary>
    protected IFileProvider fileProvider = null!;

    /// <summary>File to read</summary>
    public virtual IFileProvider FileProvider { get => fileProvider; set => this.AssertWritable().fileProvider = value; }

    /// <summary>Create uninitialized file</summary>
    public LocalizationReaderXmlFromFileProvider() : base() { }
    /// <summary>Create <paramref name="filename"/> reader</summary>
    public LocalizationReaderXmlFromFileProvider(IFileProvider fileProvider, string filename) : base()
    {
        this.filename = filename;
        this.fileProvider = fileProvider;
    }

    /// <summary>Open stream to associated file</summary>
    protected override XElement CreateElement()
    {
        // Get file info
        IFileInfo fileinfo = fileProvider.GetFileInfo(filename);
        // Open stream
        using Stream s = fileinfo.CreateReadStream();
        // Create yaml stream
        XDocument document = XDocument.Load(s);
        //
        return document.Root ?? throw new InvalidOperationException("No root element");
    }

    /// <summary>Print information</summary>
    public override string ToString() => $"{GetType().Name}({FileName})";
}

