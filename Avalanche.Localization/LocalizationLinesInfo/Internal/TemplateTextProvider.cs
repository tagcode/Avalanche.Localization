// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization.Internal;
using Avalanche.Template;
using Avalanche.Utilities;
using Avalanche.Utilities.Provider;

/// <summary>Converts "Text" from intermediate source line format into <see cref="ITemplateText"/>, uses template format provider.</summary>
public class TemplateTextProvider : ProviderBase<IEnumerable<KeyValuePair<string, MarkedText>>, ITemplateBreakdown>
{
    /// <summary>Singleton that uses the default template formats: brace</summary>
    static TemplateTextProvider instance = new TemplateTextProvider(TemplateFormats.All.ByName);
    /// <summary>Singleton that uses the default template formats: brace</summary>
    public static TemplateTextProvider Instance => instance;

    /// <summary>Template format provider</summary>
    protected IProvider<string, ITemplateFormat> templateFormatProvider;
    /// <summary>Template format provider</summary>
    public IProvider<string, ITemplateFormat> TemplateFormatProvider => templateFormatProvider;

    /// <summary>Create template text provider</summary>
    /// <param name="templateFormatProvider">Template text provider, e.g. <see cref="ITemplateFormats.ByName"/>.</param>
    public TemplateTextProvider(IProvider<string, ITemplateFormat> templateFormatProvider)
    {
        this.templateFormatProvider = templateFormatProvider ?? throw new ArgumentNullException(nameof(templateFormatProvider));
    }

    /// <summary>Get template text</summary>
    public override bool TryGetValue(IEnumerable<KeyValuePair<string, MarkedText>> line, out ITemplateBreakdown value)
    {
        // Read values
        line.ReadTemplateFormatText(templateFormat: out MarkedText templateFormat, text: out MarkedText text);        
        // No template format or text
        if (!templateFormat.HasValue || !text.HasValue) { value = null!; return false; }
        // Get template format
        if (!templateFormatProvider.TryGetValue(templateFormat, out ITemplateFormat? templateFormat1)) { value = null!; return false; }
        // Parse text
        if (!templateFormat1.Breakdown.TryGetValue(text, out ITemplateBreakdown templateText)) { value = null!; return false; }
        // Assign value
        value = templateText;
        return true;
    }
}
