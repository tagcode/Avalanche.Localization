// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization;
using Avalanche.Utilities;

/// <summary>Resource localizer base class</summary>
public abstract class Localizer : ReadOnlyAssignableClass, ILocalizer
{
    /// <summary>Print namespace for <paramref name="type"/>.</summary>
    public static string PrintNamespace(Type type) => CanonicalName.Print(type, CanonicalNameOptions.IncludeGenerics | CanonicalNameOptions.IncludeNamespace);
    /// <summary>Print namespace for <typeparamref name="T"/>.</summary>
    public static string PrintNamespace<T>() => CanonicalName.Print(typeof(T), CanonicalNameOptions.IncludeGenerics | CanonicalNameOptions.IncludeNamespace);

    /// <summary>Active culture provider</summary>
    protected ICultureProvider cultureProvider = null!;
    /// <summary>Localization context</summary>
    protected ILocalization localization = null!;
    /// <summary>Base namespace</summary>
    protected string? @namespace = null;

    /// <summary>Active culture provider</summary>
    public virtual ICultureProvider CultureProvider { get => cultureProvider; set => this.AssertWritable().cultureProvider = value; }
    /// <summary>Localization context</summary>
    public virtual ILocalization Localization { get => localization; set => this.AssertWritable().localization = value; }
    /// <summary>Base namespace</summary>
    public virtual string? Namespace { get => @namespace; set => this.AssertWritable().@namespace = value; }
    /// <summary>Resource Type</summary>
    public abstract Type ResourceType { get; }

    /// <summary></summary>
    public virtual ILocalized? this[string? name] => GetLocalized(name);

    /// <summary></summary>
    public Localizer(ILocalization localization, ICultureProvider cultureProvider, string? @namespace) : base()
    {
        this.localization = localization;
        this.cultureProvider = cultureProvider;
        this.@namespace = @namespace;
    }

    /// <summary>
    /// Get localized resource for key "Namespace.Name".
    /// 
    /// If <see cref="ILocalizer.Namespace"/> or <paramref name="name"/> is null, then counter part is used as key without separator '.'.
    /// </summary>
    protected abstract ILocalized? GetLocalized(string? name);

    /// <summary>Hook point for handling localization result.</summary>
    /// <param name="resource">Detected resource or 'null' if not found.</param>
    protected virtual void SearchedLocation(string? key, string? culture, object? resource) { }

    /// <summary>Creates key by appending <see cref="Namespace"/> to <paramref name="name"/>.</summary>
    /// <returns>"Namespace.Name"</returns>
    protected virtual string? CreateKey(string? name)
    {
        // Get namespace snapshot
        string? _namespace = Namespace;
        // Namespace and Name are null
        if (name == null && _namespace == null) return null;
        // Only Name
        if (_namespace == null && name != null) return name;
        // Only Namespace
        if (_namespace != null && name == null) return _namespace;
        // Estimate length
        int length = _namespace!.Length + 1 + name!.Length;
        // Allocate chars
        Span<char> buf = length < 256 ? stackalloc char[length] : new char[length];
        // Copy chars
        _namespace.CopyTo(buf);
        buf[_namespace.Length] = '.';
        name.CopyTo(buf.Slice(_namespace.Length + 1));
        // Create string
        string str = new string(buf);
        // Return
        return str;
    }

    /// <summary></summary>
    public override int GetHashCode()
    {
        FNVHash32 hash = new();
        hash.HashIn(Localization);
        hash.HashIn(CultureProvider);
        hash.HashIn(Namespace);
        hash.HashIn(ResourceType);
        return hash.Hash;
    }

    /// <summary></summary>
    public override bool Equals(object? obj)
    {
        // Not localizer
        if (obj is not Localizer other) return false;
        // Disqualify by properties
        if (!this.ResourceType.Equals(other.ResourceType)) return false;
        if (this.Namespace != other.Namespace) return false;
        if (this.Localization != other.Localization) return false;
        ICultureProvider? cultureProvider1 = this.CultureProvider, cultureProvider2 = other.CultureProvider;
        if ((cultureProvider1 == null) != (cultureProvider2 == null)) return false;
        if (cultureProvider1 != null && cultureProvider2 != null && !cultureProvider1.Equals(cultureProvider2)) return false;
        // Are equals
        return true;
    }

    /// <summary>Print information</summary>
    public override string ToString() => $"Localizer({CultureProvider}:{Namespace})";
}

/// <summary><typeparamref name="T"/> localizer</summary>
/// <typeparam name="T">Resource type</typeparam>
public abstract class Localizer<T> : Localizer, ILocalizer<T>
{
    /// <summary>
    /// Get localized resource for key "Namespace.Name".
    /// 
    /// If <see cref="ILocalizer.Namespace"/> or <paramref name="name"/> is null, then counter part is used as key without separator '.'.
    /// </summary>
    public new virtual ILocalized<T>? this[string? name] => (ILocalized<T>?)GetLocalized(name);

    /// <summary></summary>
    public Localizer() : base(null!, null!, null) { }
    /// <summary></summary>
    public Localizer(ILocalization localization) : base(localization, null!, null!) { }
    /// <summary></summary>
    public Localizer(ILocalization localization, ICultureProvider cultureProvider) : base(localization, cultureProvider, null!) { }
    /// <summary></summary>
    public Localizer(ILocalization localization, ICultureProvider cultureProvider, string? @namespace) : base(localization, cultureProvider, @namespace) { }

    /// <summary>Print information</summary>
    public override string ToString() => $"Localizer<{CanonicalName.Print(typeof(T))}>({CultureProvider}:{Namespace})";
}

/// <summary><typeparamref name="T"/> localizer that derives namespace from <typeparamref name="Namespace"/>.</summary>
/// <typeparam name="T">Resource type</typeparam>
/// <typeparam name="Namespace">Key namespace</typeparam>
public abstract class Localizer<T, Namespace> : Localizer<T>, ILocalizer<T, Namespace>
{
    /// <summary></summary>
    public Localizer() : base(null!, null!, CanonicalName.Print(typeof(Namespace))) { }
    /// <summary></summary>
    public Localizer(ILocalization localization) : base(localization, null!, PrintNamespace<Namespace>()) { }
    /// <summary></summary>
    public Localizer(ILocalization localization, ICultureProvider cultureProvider) : base(localization, cultureProvider, PrintNamespace<Namespace>()) { }
    /// <summary></summary>
    public Localizer(ILocalization localization, ICultureProvider cultureProvider, string? @namespace) : base(localization, cultureProvider, @namespace) { }
}

/// <summary><typeparamref name="T"/> localizer that derives namespace from <typeparamref name="Namespace"/> and culture provider from <typeparamref name="CultureProvider"/>.</summary>
/// <typeparam name="T">Resource type</typeparam>
/// <typeparam name="Namespace">Key namespace</typeparam>
/// <typeparam name="CultureProvider">Culture provider type. Type must have a parameterless constructor.</typeparam>
public abstract class Localizer<T, Namespace, CultureProvider> : Localizer<T, Namespace>, ILocalizer<T, Namespace, CultureProvider> where CultureProvider : ICultureProvider
{
    /// <summary></summary>
    public Localizer() : base(null!, Avalanche.Localization.CultureProvider.ByTypeCached[typeof(CultureProvider)]) { }
    /// <summary></summary>
    public Localizer(ILocalization localization) : base(localization, Avalanche.Localization.CultureProvider.ByTypeCached[typeof(CultureProvider)]) { }
    /// <summary></summary>
    public Localizer(ILocalization localization, ICultureProvider cultureProvider) : base(localization, cultureProvider, PrintNamespace<Namespace>()) { }
    /// <summary></summary>
    public Localizer(ILocalization localization, ICultureProvider cultureProvider, string? @namespace) : base(localization, cultureProvider, @namespace) { }
}
