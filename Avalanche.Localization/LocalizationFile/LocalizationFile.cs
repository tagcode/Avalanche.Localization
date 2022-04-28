// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization;
using System.Diagnostics.CodeAnalysis;
using Avalanche.Utilities;

/// <summary></summary>
public class LocalizationFile : LocalizationFileBase
{
    /// <summary>Create file at <paramref name="filename"/>.</summary>
    /// <remarks>The caller finalize <see cref="ILocalizationFile.FileFormat"/> and then finalized into read-only state with <see cref="IReadOnly"/>.</remarks>
    public static LocalizationFile Create(string filename)
        => new LocalizationFile
        {
            FileName = filename,
            FileSystem = LocalizationFileSystem.ApplicationRoot
        };

    /// <summary>File system file reader, e.g. <see cref="LocalizationFileSystem"/></summary>
    protected ILocalizationFileSystem fileSystem = null!;

    /// <summary>File system file reader, e.g. <see cref="LocalizationFileSystem"/></summary>
    public virtual ILocalizationFileSystem FileSystem { get => fileSystem; set => this.AssertWritable().fileSystem = value; }

    /// <summary>Try open <paramref name="stream"/> to resource.</summary>
    /// <exception cref="FileNotFoundException"></exception>
    /// <exception cref="IOException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public override bool TryOpen([NotNullWhen(true)] out Stream? stream)
    {
        // Get snapshots
        var _fileSystem = FileSystem;
        var _fileName = FileName;
        // Assert
        if (_fileName == null) throw new InvalidOperationException($"No {nameof(FileName)}");
        if (_fileSystem == null) throw new InvalidOperationException($"No {nameof(FileSystem)}");
        // Return stream
        return _fileSystem.TryOpen(FileName!, out stream);
    }
}

