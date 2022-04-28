// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization;
using Avalanche.Utilities;

/// <summary>Json file format</summary>
public class LocalizationFileFormatJson : LocalizationFileFormat, ILocalizationLineFileFormat
{
    /// <summary>Singleton</summary>
    static LocalizationFileFormatJson instance = new LocalizationFileFormatJson(".json");
    /// <summary>Singleton</summary>
    public static LocalizationFileFormatJson Instance => instance;

    /// <summary></summary>
    public LocalizationFileFormatJson(params string[] extensions) : base(extensions) { }

    /// <summary>Read lines from <paramref name="stream"/></summary>
    public IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>> Read(Stream stream) => new LocalizationReaderJson.FromStream(stream).ToList();


}

