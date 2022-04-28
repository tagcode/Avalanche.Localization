// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization;
using Avalanche.Utilities;

/// <summary></summary>
public class LocalizedBase<T> : ReadOnlyAssignableClass, ILocalized<T>
{
    /// <summary>Culture, "" for invariant culture.</summary>
    protected string culture = null!;
    /// <summary>Resource key</summary>
    protected string key = null!;
    /// <summary>Resource value</summary>
    protected T value = default!;
    /// <summary>Is this record initialized with value.</summary>
    protected bool hasValue;
    /// <summary>Parse errors</summary>
    protected IList<ILocalizationError> errors = null!;

    /// <summary>Culture, "" for invariant culture.</summary>
    public virtual string Culture { get => culture; set => this.AssertWritable().culture = value; }
    /// <summary></summary>
    public virtual IFormatProvider Format { get => null!; set { } }
    /// <summary>Resource key</summary>
    public virtual string Key { get => key; set => this.AssertWritable().key = value; }
    /// <summary>Resource value</summary>
    public virtual T Value { get => value; set => this.AssertWritable().setValue(value); }
    /// <summary>Is this record initialized with value.</summary>
    public virtual bool HasValue => hasValue;
    /// <summary>Parse errors</summary>
    public virtual IList<ILocalizationError> Errors { get => errors; set => this.AssertWritable().errors = value; }

    /// <summary></summary>
    /// <param name="value"></param>
    protected virtual void setValue(T value)
    {
        this.value = value;
        this.hasValue = true;
    }
}

