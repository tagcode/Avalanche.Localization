<b>Avalanche.Localization.Extensions</b> contains extension methods for adding <em>ILocalization</em> to dependency injection,
[[git]](https://github.com/tagcode/Avalanche.Message/Avalanche.Message.Extensions/), 
[[www]](https://avalanche.fi/Avalanche.Core/Avalanche.Localization/docs/microsoft.extensions/index.html), 
[[licensing]](https://avalanche.fi/Avalanche.Core/license/index.html).

```csharp
// Add service descriptors
IServiceCollection serviceCollection = new ServiceCollection()
    .AddAvalancheLocalizationService()
    .AddAvalancheLocalizationFileSystemApplicationRoot()
    .AddAvalancheLocalizationEmbeddedResourceProviderDefault()
    .AddAvalancheLocalizationResourceManagerProvider()
    .AddAvalancheStringLocalizer();
// Build service
using ServiceProvider service = serviceCollection.BuildServiceProvider();
// Get Localization
ILocalization localization = service.GetService<ILocalization>()!;
// Get text
ILocalizedText localizedText = localization.LocalizedTextCached[("", "Namespace.Apples")];
// Print text
WriteLine(localizedText.Print(new object[] { 2 })); // "You've got 2 apples."
```


