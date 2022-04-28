// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization;

/// <summary>Parameter's plural category+case assignment</summary>
public readonly record struct PluralAssignment(string parameterName, string category, string @case, string? culture = null)
{

    /// <summary>Create new assignment with <paramref name="newCulture"/>.</summary>
    public PluralAssignment WithCulture(string? newCulture) => new PluralAssignment(parameterName, category, @case, newCulture);

    /// <summary>Hash array of plural assignments, order of elements doesn't matter.</summary>
    public static ulong Hash<List>(List list) where List : IList<PluralAssignment>
    {
        // Init hash
        ulong hash = 0;
        // Item count
        int count = list.Count;
        // Hash-in each element
        for (int i = 0; i < count; i++)
        {
            ulong hash2 = 0xcbf29ce484222325;
            PluralAssignment pa = list[i];
            if (pa.parameterName != null) hash2 ^= (ulong)pa.parameterName.GetHashCode();
            hash2 *= 0x100000001b3;
            if (pa.category != null) hash2 ^= (ulong)pa.category.GetHashCode();
            hash2 *= 0x100000001b3;
            if (pa.@case != null) hash2 ^= (ulong)pa.@case.GetHashCode();
            hash2 *= 0x100000001b3;
            if (pa.culture != null) hash2 ^= (ulong)pa.culture.GetHashCode();
            hash ^= hash2;
        }
        // Return
        return hash;
    }

    /// <summary></summary>
    static string Escape(string value) => value.Replace("\\", "\\\\").Replace(",", "\\,").Replace(":", "\\:").Replace(" ", "\\");

    /// <summary>Print information</summary>
    public override string ToString() =>
        culture == null ?
        $"{Escape(parameterName)}:{Escape(category)}:{Escape(@case)}" :
        $"{Escape(parameterName)}:{Escape(category)}:{Escape(@case)}:{Escape(culture)}";
}
