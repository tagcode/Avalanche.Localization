// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization;
using Microsoft.Extensions.Logging;

/// <summary>Error logger</summary>
public class LocalizationErrorLogger : ILocalizationErrorHandler
{
    /// <summary></summary>
    public static ILocalizationErrorHandler Create(ILogger logger) => new LocalizationErrorLogger(logger);

    /// <summary></summary>
    protected ILogger? logger;

    /// <summary></summary>
    public LocalizationErrorLogger()
    {
        this.logger = null;
    }
    /// <summary></summary>
    protected LocalizationErrorLogger(ILogger logger)
    {
        this.logger = logger;
    }
    /// <summary></summary>
    public LocalizationErrorLogger(ILogger<ILocalization>? logger)
    {
        this.logger = logger;
    }

    /// <summary></summary>
    public void Handle(ILocalizationError error)
    {
        // Get logger reference
        var _logger = this.logger;
        // No logger
        if (_logger == null) return;
        // Create EventId
        EventId eventId = new EventId(error.Code);
        // Log
        _logger.LogError(eventId, "{Message} {Culture} {Key} {Position}", error.Message, error.Culture, error.Key, error.Text.Position);
    }

    /// <summary></summary>
    public override bool Equals(object? obj)
    {
        // Not adapter
        if (obj is not LocalizationErrorLogger other) return false;
        // Compare loggers
        ILogger? logger1 = this.logger, logger2 = other.logger;
        if (logger1 == null && logger2 == null) return true;
        if (logger1 == null || logger2 == null) return false;
        return (logger1.Equals(logger2));
    }

    /// <summary></summary>
    public override int GetHashCode() => (logger == null ? 0 : logger.GetHashCode()) ^ 0x123148;

    /// <summary></summary>
    public override string ToString() => GetType().Name + " " + logger;
}
