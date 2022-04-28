// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Avalanche.Utilities;

/// <summary></summary>
public abstract class LocalizationFileBase : ReadOnlyAssignableClass, ILocalizationFile, ILocalized<ILocalizationFile>
{
    /// <summary></summary>
    protected string? fileName;
    /// <summary></summary>
    protected ILocalizationFileFormat fileFormat = null!;
    /// <summary></summary>
    protected string culture = null!;
    /// <summary></summary>
    protected string key = null!;
    /// <summary></summary>
    protected IList<ILocalizationError> errors = Array.Empty<ILocalizationError>();

    /// <summary></summary>
    public virtual string? FileName { get => fileName; set => this.AssertWritable().fileName = value; }
    /// <summary></summary>
    public virtual ILocalizationFileFormat FileFormat { get => fileFormat; set => this.AssertWritable().fileFormat = value; }
    /// <summary></summary>
    public virtual string Culture { get => culture; set => this.AssertWritable().culture = value; }
    /// <summary></summary>
    public virtual IFormatProvider Format { get => null!; set { } }
    /// <summary></summary>
    public virtual string Key { get => key; set => this.AssertWritable().key = value; }

    /// <summary></summary>
    ILocalizationFile ILocalized<ILocalizationFile>.Value => this;
    /// <summary></summary>
    public virtual IList<ILocalizationError> Errors { get => errors; set => this.AssertWritable().errors = value; }

    /// <summary>Try open <paramref name="stream"/> to resource.</summary>
    /// <exception cref="Exception">On unexpected error.</exception>
    public abstract bool TryOpen([NotNullWhen(true)] out Stream? stream);

    /// <summary></summary>
    public override int GetHashCode()
    {
        FNVHash32 hash = new FNVHash32();
        hash.HashInObject(fileFormat);
        hash.HashInObject(fileName);
        return hash.Hash;
    }

    /// <summary></summary>
    public override bool Equals(object? obj)
    {
        if (obj is not ILocalizationFile other) return false;

        ILocalizationFileFormat? _ff1 = this.FileFormat, _ff2 = other.FileFormat;
        if (_ff1 == null && _ff2 == null) { }
        else if (_ff1 == null || _ff2 == null) return false;
        else if (!_ff1.Equals(_ff2)) return false;

        string? _fn1 = this.FileName, _fn2 = other.FileName;
        if (_fn1 == null && _fn2 == null) { }
        else if (_fn1 == null || _fn2 == null) return false;
        else if (!_fn1.Equals(_fn2)) return false;

        return true;
    }

    /// <summary></summary>
    public override string ToString() => FileName ?? Key ?? GetType().Name;
}

