// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization;
using Avalanche.Localization.Internal;
using Avalanche.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

/// <summary>Dependency injection service descriptors.</summary>
public record StringLocalizerServiceDescriptors : RecordPropertiesOf<ServiceDescriptor>
{
    /// <summary>Singleton</summary>
    static readonly Lazy<StringLocalizerServiceDescriptors> instance = new Lazy<StringLocalizerServiceDescriptors>();
    /// <summary>Singleton</summary>
    public static StringLocalizerServiceDescriptors Instance => instance.Value;

    /// <summary></summary>
    public ServiceDescriptor StringLocalizerFactory = new ServiceDescriptor(typeof(IStringLocalizerFactory), typeof(DiStringLocalizerFactory), ServiceLifetime.Singleton);
    /// <summary></summary>
    public ServiceDescriptor StringLocalizer = new ServiceDescriptor(typeof(IStringLocalizer), typeof(DI.StringLocalizer), ServiceLifetime.Singleton);
    /// <summary></summary>
    public ServiceDescriptor StringLocalizerOfT = new ServiceDescriptor(typeof(IStringLocalizer<>), typeof(StringLocalizerWrapper<>), ServiceLifetime.Singleton);

    // <summary></summary>
    //public ServiceDescriptor X = new ServiceDescriptor(typeof(), typeof(), ServiceLifetime.Singleton);
}

