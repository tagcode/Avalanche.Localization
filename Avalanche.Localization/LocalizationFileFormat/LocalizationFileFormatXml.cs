// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization;
using Avalanche.Utilities;

/// <summary>Xml file format</summary>
public class LocalizationFileFormatXml : LocalizationFileFormat, ILocalizationLineFileFormat
{
    /// <summary>Singleton</summary>
    static LocalizationFileFormatXml instance = new LocalizationFileFormatXml(".xml");
    /// <summary>Singleton</summary>
    public static LocalizationFileFormatXml Instance => instance;

    /// <summary></summary>
    public LocalizationFileFormatXml(params string[] extensions) : base(extensions) { }

    /// <summary>Read lines from <paramref name="stream"/></summary>
    public IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>> Read(Stream stream) => new LocalizationReaderXml.FromStream(stream).ToList();
}

