// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization;

// <docs>
/// <summary>Culture and format provider.</summary>
public interface ICultureProvider
{
    /// <summary>Culture (ISO 649-1)[-(ISO 3166-2)], e.g. "en" or "en-UK". "" for invariant culture. If null, then unassigned or resource is assigned to multiple cultures.</summary>
    string Culture { get; set; }
    /// <summary>Format provider</summary>
    IFormatProvider Format { get; set; }
}
// </docs>
