// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization;
using Avalanche.Template;

// <docs>
/// <summary>Text localizer</summary>
public interface ITextLocalizer : ILocalizer<ITemplatePrintable> 
{
    /// <summary>
    /// Get localized text for key "Namespace.Name".
    /// 
    /// If <see cref="ILocalizer.Namespace"/> or <paramref name="name"/> is null, then counter part is used as key without separator '.'.
    /// </summary>
    new ILocalizedText? this[string? name] { get; }
}
/// <summary>Text localizer</summary>
public interface ITextLocalizer<Namespace> : ITextLocalizer, ILocalizer<ITemplatePrintable, Namespace> { }
/// <summary>Text localizer</summary>
public interface ITextLocalizer<Namespace, CultureProvider> : ITextLocalizer<Namespace>, ILocalizer<ITemplatePrintable, Namespace, CultureProvider> where CultureProvider : ICultureProvider { }
// </docs>
