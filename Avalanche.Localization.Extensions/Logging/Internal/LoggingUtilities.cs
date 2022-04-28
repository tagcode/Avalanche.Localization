// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization.Internal;
using Microsoft.Extensions.Logging;

/// <summary></summary>
public static class LoggingUtilities
{
    /// <summary></summary>
    static readonly EventId eventIdKeyNotFound = new EventId(LocalizationMessageIds.KeyNotFound, "Avalanche.Localization." + nameof(LocalizationMessageIds.KeyNotFound));
    /// <summary></summary>
    static readonly Action<ILogger, string?, string?, Exception?> lineNotFound = LoggerMessage.Define<string?, string?>(LogLevel.Debug, eventIdKeyNotFound, "Localization line was not found: Key={Key}, Culture={Culture}");
    /// <summary></summary>
    static readonly Action<ILogger, string?, string?, Exception?> fileNotFound = LoggerMessage.Define<string?, string?>(LogLevel.Debug, eventIdKeyNotFound, "Localization file was not found: Key={Key}, Culture={Culture}");

    /// <summary></summary>
    public static EventId EventIdKeyNotFound => eventIdKeyNotFound;
    /// <summary></summary>
    public static Action<ILogger, string?, string?, Exception?> LineNotFound => lineNotFound;
    /// <summary></summary>
    public static Action<ILogger, string?, string?, Exception?> FileNotFound => fileNotFound;
}
