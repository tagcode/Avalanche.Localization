// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization;
using Avalanche.Utilities;

/// <summary>Localization error</summary>
public record LocalizationError : ReadOnlyAssignableRecord, ILocalizationError
{
    /// <summary>Key</summary>
    protected string key = null!;
    /// <summary>Culture</summary>
    protected string culture = null!;
    /// <summary>Error message</summary>
    protected string message = null!;
    /// <summary>The text span where the error occurs. Can have reference to source file, line, column, and position index.</summary>
    protected MarkedText text;
    /// <summary>Error code, see <see cref="LocalizationMessageIds"/>.</summary>
    protected int code;

    /// <summary>Key</summary>
    public string Key { get => key; set => this.AssertWritable().key = value; }
    /// <summary>Culture</summary>
    public string Culture { get => culture; set => this.AssertWritable().culture = value; }
    /// <summary>Error message</summary>
    public string Message { get => message; set => this.AssertWritable().message = value; }
    /// <summary>The text span where the error occurs. Can have reference to source file, line, column, and position index.</summary>
    public MarkedText Text { get => text; set => this.AssertWritable().text = value; }
    /// <summary>Error code, see <see cref="LocalizationMessageIds"/>.</summary>
    public int Code { get => code; set => this.AssertWritable().code = value; }

    /// <summary>Print information</summary>
    public override string ToString() => $"{Message} {Culture} {Key} {Text.Position}";    
}

