// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization;
using Avalanche.Utilities;

/// <summary>Yaml file format</summary>
public class LocalizationFileFormatYaml : LocalizationFileFormat, ILocalizationLineFileFormat
{
    /// <summary>Singleton</summary>
    static LocalizationFileFormatYaml instance = new LocalizationFileFormatYaml(".yml", ".yaml");
    /// <summary>Singleton</summary>
    public static LocalizationFileFormatYaml Instance => instance;

    /// <summary></summary>
    public LocalizationFileFormatYaml(params string[] extensions) : base(extensions) { }

    /// <summary>Read lines from <paramref name="stream"/></summary>
    public IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>> Read(Stream stream) => new LocalizationReaderYaml.FromStream(stream).ToList();
}

