// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization;

// <docs>
/// <summary>Text localizer</summary>
public interface IFileLocalizer : ILocalizer<ILocalizationFile[]> { }
/// <summary>Text localizer</summary>
public interface IFileLocalizer<Namespace> : IFileLocalizer, ILocalizer<ILocalizationFile[], Namespace> { }
/// <summary>Text localizer</summary>
public interface IFileLocalizer<Namespace, CultureProvider> : IFileLocalizer<Namespace>, ILocalizer<ILocalizationFile[], Namespace, CultureProvider> where CultureProvider : ICultureProvider { }
// </docs>
