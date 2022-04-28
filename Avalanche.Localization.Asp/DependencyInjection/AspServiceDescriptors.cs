// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization;
using Avalanche.Localization.Internal;
using Avalanche.Utilities;
using Avalanche.Utilities.Provider;
using Microsoft.Extensions.DependencyInjection;

/// <summary>Dependency injection service descriptors.</summary>
public record AspServiceDescriptors : RecordPropertiesOf<ServiceDescriptor>
{
    /// <summary>Singleton</summary>
    static readonly Lazy<AspServiceDescriptors> instance = new Lazy<AspServiceDescriptors>();
    /// <summary>Singleton</summary>
    public static AspServiceDescriptors Instance => instance.Value;

    /// <summary>Adapts <![CDATA[IOptions<RequestLocalizationOptions>]]> into fallback culture provider</summary>
    public ServiceDescriptor AspFallbackCultureProvider { get; init; } = new ServiceDescriptor(typeof(IProvider<string, string[]>), typeof(AspFallbackCultureProvider), ServiceLifetime.Singleton);
}

