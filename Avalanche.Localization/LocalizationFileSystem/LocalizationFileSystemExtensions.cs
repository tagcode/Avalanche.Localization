// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization;

/// <summary>Extension methods for <see cref="ILocalizationFileSystem"/></summary>
public static class LocalizationFileSystemExtensions_
{
    /// <summary>Decorate <paramref name="filesystem"/> to cache</summary>
    /// <param name="cacheLists">Caches file and directory lists</param>
    /// <param name="cacheFiles">Caches files contents into memory</param>
    public static ILocalizationFileSystem Cached(this ILocalizationFileSystem filesystem, bool cacheLists, bool cacheFiles) => new LocalizationFileSystemCached(filesystem, cacheLists, cacheFiles);
}

