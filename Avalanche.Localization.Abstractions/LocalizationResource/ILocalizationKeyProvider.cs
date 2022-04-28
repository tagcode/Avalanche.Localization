// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization;

// <docs>
/// <summary>Interface for classes that represent a localization resource.</summary>
public interface ILocalizationKeyProvider
{
    /// <summary>Key that identifies the resource. Typically in format of "Namespace.Name". The last dot '.' separates namespaces from name part. If null, then unassigned or resource contains multiple keys.</summary>
    string Key { get; set; }
}
// </docs>
