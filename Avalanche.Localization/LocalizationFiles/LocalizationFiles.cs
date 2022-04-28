// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization;

/// <summary>Record of localization file properties and providers with initial providers.</summary>
public class LocalizationFiles : LocalizationFilesBase
{
    /// <summary>
    /// Singleton with following configuration:
    ///     File provider: Operating System provider <see cref="LocalizationFileSystem"/>
    ///     File extensions: .yaml, .yml, .xml, .json
    ///     Paths: Application\Resources\{Culture}\{Key}.ext
    /// </summary>
    public static ILocalizationFiles Instance = Localization.Default.Files;

    /// <summary></summary>
    public LocalizationFiles() : base()
    {
    }
}
