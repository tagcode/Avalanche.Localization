using System.Globalization;
using Avalanche.Localization;
using Avalanche.Utilities;
using Avalanche.Utilities.Provider;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

IServiceCollection services = builder.Services;
services
    .AddAvalancheLocalizationService()
    .AddAvalancheLocalizationEmbeddedResourceProviderDefault()
    .AddAvalancheLocalizationResourceManagerProvider()
    .AddAvalancheLocalizationAspSupport()
    .AddAvalancheStringLocalizer()
    .AddAvalancheLocalizationFileSystemApplicationRoot()
    .AddSingleton(typeof(ILocalizationFilePatterns), new LocalizationFilePatterns("Pages/{Key}", "Pages/{Culture}/{Key}"))
    .AddAvalancheLocalizationEmbeddedResourceProvider("*/*.Pages.{Key}", "*/*.Pages.{Culture}.{Key}")
    .Configure<Microsoft.Extensions.Localization.LocalizationOptions>(options =>
    {
        options.ResourcesPath = null!;
    })
    .Configure<RequestLocalizationOptions>(options =>
    {
        options.AddSupportedCultures("en", "fi", "sv");
        options.AddSupportedUICultures("en", "fi", "sv");
        options.SetDefaultCulture("en"); // <- Is not applied. Why?
        options.FallBackToParentCultures = true;
        options.FallBackToParentUICultures = true;
    });

// Add services: IHtmlLocalizer, IViewLocalizer (see Pages/Index2.cshtml)
services.AddRazorPages().AddViewLocalization();

var app = builder.Build();

// Localization debug diagnostics
/*
    // Get localization context
    ILocalization localization = app.Services.GetService<ILocalization>()!;
    // Get logger
    ILogger logger = app.Services.GetService<ILogger<Program>>()!;
    // Print lines that are visible to localization
    if (localization.LineQueryCached.TryGetValue((null, null), out IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>> lines))
    {
        foreach (var line in lines)
        {
            // Read values
            line.ReadValues(out MarkedText pluralRules, out MarkedText culture, out MarkedText key, out MarkedText plurals, out MarkedText template, out MarkedText text);
            // No value
            if (!key.HasValue || !text.HasValue) continue;
            // Log
            logger.LogInformation("Culture={Culture}, Key={Key}, Text={Text}, Template={Template}, PluralRules={PluralRules}, Plurals={Plurals}", culture.Text, key.Text, text.Text, template.Text, pluralRules.Text, plurals.Text);
        }
    }
*/

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}
app.UseStaticFiles();

app.UseRouting();

RequestLocalizationOptions localizationOptions = app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value;
app.UseRequestLocalization(localizationOptions);

app.UseAuthorization();

app.MapRazorPages();

app.Run();
