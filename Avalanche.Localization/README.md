<b>Avalanche.Localization</b> is localization class library with pluralization features, 
[[git]](https://github.com/tagcode/Avalanche.Localization/Avalanche.Localization), 
[[www]](https://avalanche.fi/Avalanche.Core/Avalanche.Localization/docs/), 
[[licensing]](https://avalanche.fi/Avalanche.Core/license/index.html).

<b>Localization in short.</b> Add package reference to .csproj.
```xml
<PropertyGroup>
    <RestoreAdditionalProjectSources>https://avalanche.fi/Avalanche.Core/nupkg/index.json</RestoreAdditionalProjectSources>
</PropertyGroup>
<ItemGroup>
    <PackageReference Include="Avalanche.Localization"/>
    <PackageReference Include="Avalanche.Localization.Cldr"/>
</ItemGroup>
```

Localization files can be dropped into "Resources/&lt;Key&gt;" and "Resources/&lt;Culture&gt;/&lt;Key&gt;" either as embedded resource or as output copied resource. 

Example: <b>Resources/Namespace.yaml</b>. Root file can supply texts for multiple languages. Pluralization [rules](xref:Unicode.CLDR41) are language specific.

```yml
TemplateFormat: Brace
PluralRules: Unicode.CLDR41

Invariant:
- Culture: ""
  Items:
  - Key: Namespace.Apples
    Cases:
    - Text: "You've got an apple."
      Plurals: "0:cardinal:one:en"
    - Text: "You've got {0} apples."
      Plurals: "0:cardinal:other:en" 
  - Key: Namespace.Today
    Text: "Today is {0}."

Finnish:
- Culture: fi
  Items:
  - Key: Namespace.Apples
    Cases:
    - Text: "Sinulla on yksi omena."
      Plurals: "0:cardinal:one"
    - Text: "Sinulla on {0} omenaa."
      Plurals: "0:cardinal:other"
  - Key: Namespace.Today
    Text: "Tänään on {0}."

```

The following example <b>Resources/sv/Namespace.yaml</b> is loaded when 'sv' culture is requested.

```yml
TemplateFormat: Brace
PluralRules: Unicode.CLDR41

Swedish:
- Culture: sv
  Items:
  - Key: Namespace.Apples
    Cases:
    - Text: "Du har ett äpple."
      Plurals: "0:cardinal:one"
    - Text: "Du har {0} äpplen."
      Plurals: "0:cardinal:other"
  - Key: Namespace.Today
    Text: "Idag är det {0}."

```

<b>Localization.Default.LocalizableTextCached</b> provides texts that can localize and pluralize to culture argument.

```csharp
// Create text
ILocalizedText text = Localization.Default.LocalizableTextCached["Namespace.Apples"];
// Get culture
IFormatProvider culture = CultureInfo.GetCultureInfo("sv");
// Create arguments
object[] arguments = { 2 };
// Print
WriteLine(text.Print(culture, arguments)); // "Du har 2 äpplen."
```

<b>Localization.Default.LocalizingTextCached</b> provides localizing texts that localize and pluralize to the active culture.

```csharp
// Get active culture provider (CurrentThread)
ICultureProvider cultureProvider = CultureProvider.CurrentCulture.Instance;
// Get localizing text
ILocalizedText text = Localization.Default.LocalizingTextCached[(cultureProvider, "Namespace.Apples")];
// Assign language
CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("sv");
// Assign format provider
CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("sv");
// Print to active culture
WriteLine(text.Print(new object[] { 2 })); // "Du har 2 äpplen."
```
