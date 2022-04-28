// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization;
using System.Runtime.Serialization;

/// <summary>Exception within localization</summary>
public class LocalizationException : Exception
{
    /// <summary></summary>
    public LocalizationException() { }
    /// <summary></summary>
    public LocalizationException(string? message) : base(message) { }
    /// <summary></summary>
    public LocalizationException(string? message, Exception? innerException) : base(message, innerException) { }
    /// <summary></summary>
    protected LocalizationException(SerializationInfo info, StreamingContext context) : base(info, context) { }
}

