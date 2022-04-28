// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization;

/// <summary></summary>
public class LocalizationErrorHandler : ILocalizationErrorHandler
{
    /// <summary></summary>
    protected Action<ILocalizationError> handler;

    /// <summary></summary>
    public LocalizationErrorHandler(Action<ILocalizationError> handler)
    {
        this.handler = handler ?? throw new ArgumentNullException(nameof(handler));
    }

    /// <summary></summary>
    public void Handle(ILocalizationError error) => handler(error);
}
