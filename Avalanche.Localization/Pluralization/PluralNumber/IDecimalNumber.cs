// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization.Pluralization;

/// <summary></summary>
public interface IDecimalNumber : IPluralNumber
{
    /// <summary>Get text version.</summary>
    TextNumber AsText { get; }
}
