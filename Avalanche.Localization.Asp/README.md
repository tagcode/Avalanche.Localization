<b>Avalanche.Localization.Asp</b> contains extensions for asp .net.

To run Avalanche.Localization on ASP.Net Core, make sure the Application's <b>.csproj</b> is targeting platform .NET6 or higher.

```xml
  <PropertyGroup>
    <TargetFrameworks>net6.0;net7.0</TargetFrameworks>
  </PropertyGroup>
```

Add package references.
```xml
<ItemGroup>
    <PackageReference Include="Avalanche.Localization"/>
    <PackageReference Include="Avalanche.Localization.Abstractions"/>
    <PackageReference Include="Avalanche.Localization.Extensions"/>
    <PackageReference Include="Avalanche.Localization.Asp"/>
</ItemGroup>
```


Go to <b>appsettings.json</b> and add "Debug" level for "Avalanche" to get notified on localization issues.
```json
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Avalanche": "Debug"
    }
  },
```

If there is no <em>Startup.cs</em>, then in <b>Program.cs</b> get reference to service collection
```cs
IServiceCollection services = builder.Services;
```

Remove possible previous <em>.AddLocalization()</em> service, ... 
```cs
services.AddLocalization(options =>
{
    options.ResourcesPath = "Resources";
});
```

Add <b>.AddAvalancheLocalizationService()</b>.
```cs
services
    .AddAvalancheLocalizationService()
    .AddAvalancheLocalizationEmbeddedResourceProviderDefault()
    .AddAvalancheLocalizationResourceManagerProvider()
    .AddAvalancheLocalizationAspSupport()
    .AddAvalancheStringLocalizer()
    .Configure<Microsoft.Extensions.Localization.LocalizationOptions>(options => 
    {
        options.ResourcesPath = "Resources";
    });
```

<b>Also</b> add source for localization resources, either:
```cs
services
    .AddAvalancheLocalizationFileSystemApplicationRoot();
```
or adapt file provider, which will be adapted to ILocalizationFileSystem for access to localization resources. 
Note that there should be "Resources" folder at root of the <i>IFileProvider</i>.
```cs
services
    .AddAvalancheLocalizationToUseFileProvider()
    .AddSingleton(typeof(IFileProvider), new PhysicalFileProvider(AppDomain.CurrentDomain.BaseDirectory));
```


More information in [Microsoft.Extensions.DependencyInjection](https://avalanche.fi/Avalanche.Core/Avalanche.Localization/docs/microsoft.extensions.dependencyinjection/index.html).

# Supplying localization

Localization can be supplied in both <em>.resx</em> and <em>.yaml</em> files. .yaml can be dropped in the <em>Resources/</em> folder either as embedded resource or as output copied resource. 

<b>Note</b> that it may be good idea to have "null" as Resources path in <b>Startup.cs</b> to have easier keys and consistent across service interfaces.
```cs
services.Configure<Microsoft.Extensions.Localization.LocalizationOptions>(options => 
    {
        options.ResourcesPath = null;
    });
```

Example: <b>Resources/WebApplication.Pages.IndexModel.yaml</b>
```yml
TemplateFormat: BraceNumeric
PluralRules: Unicode.CLDR41

Invariant:
- Culture: ""
  Items:
  - Key: WebApplication.Pages.IndexModel.Home page
    Text: "Home page"
  - Key: WebApplication.Pages.IndexModel.Learn
    Text: >
        Learn about <a href="https://docs.microsoft.com/aspnet/core">building Web apps with ASP.NET Core</a>.
  - Key: WebApplication.Pages.IndexModel.Submit
    Text: "Submit"
  - Key: WebApplication.Pages.IndexModel.Superhero
    Text: "Superhero"
  - Key: WebApplication.Pages.IndexModel.SuperHeroFieldIsRequired
    Text: "The Superhero field is required."
  - Key: WebApplication.Pages.IndexModel.Welcome
    Text: "Welcome!"
```

Example: <b>Resources/de/WebApplication.Pages.IndexModel.yaml</b>
```yml
TemplateFormat: BraceNumeric
PluralRules: Unicode.CLDR41

German:
- Culture: de
  Items:
  - Key: WebApplication.Pages.IndexModel.Home page
    Text: "Startseite"
  - Key: WebApplication.Pages.IndexModel.Learn
    Text: >
        Erfahren Sie mehr über <a href="https://docs.microsoft.com/aspnet/core"> das Erstellen von Webanwendungen mit ASP.NET Core </a>.
  - Key: WebApplication.Pages.IndexModel.Submit
    Text: "Einreichen"
  - Key: WebApplication.Pages.IndexModel.Superhero
    Text: "Superheld"
  - Key: WebApplication.Pages.IndexModel.SuperHeroFieldIsRequired
    Text: "Das Superheldenfeld ist erforderlich."
  - Key: WebApplication.Pages.IndexModel.Welcome
    Text: "Willkommen!"
```

Example: <b>Resources/ja/WebApplication.Pages.IndexModel.yaml</b>
```yml
TemplateFormat: BraceNumeric
PluralRules: Unicode.CLDR41
    
Japanese:
- Culture: ja
  Items:
  - Key: WebApplication.Pages.IndexModel.Home page
    Text: "ホームページ"
  - Key: WebApplication.Pages.IndexModel.Learn
    Text: >
        <a href="https://docs.microsoft.com/aspnet/core"> ASP.NET Coreを使用したWebアプリの構築</a>について学習します
  - Key: WebApplication.Pages.IndexModel.Submit
    Text: "申し出る"
  - Key: WebApplication.Pages.IndexModel.Superhero
    Text: "スーパーヒーロー"
  - Key: WebApplication.Pages.IndexModel.SuperHeroFieldIsRequired
    Text: "スーパーヒーローフィールドは必須です。"
  - Key: WebApplication.Pages.IndexModel.Welcome
    Text: "ようこそ!"
```

# Injecting to pages

Page can be designed to use either Microsoft's localization interface or Avalanche localization interface, or both.

If Microsoft's services are used then <b>@inject</b> with <em>IStringLocalizer&lt;T&gt;</em>, <em>IViewLocalizer</em> and <em>IHtmlLocalizer&lt;T&gt;</em>.
The localization files must be supply lines to "Assembly[.Resources].Namespace.Key" key notation.

```html
@page
@using Avalanche.Localization
@using Microsoft.Extensions.Localization
@using Microsoft.AspNetCore.Mvc.Localization
@model IndexModel

@inject IStringLocalizer<IndexModel> Localizer
@inject IHtmlLocalizer<IndexModel> HtmlLocalizer
@inject IViewLocalizer ViewLocalizer

@{
    ViewData["Title"] = @Localizer["Home page"];
}

<div class="text-center">
    <h1 class="display-4">@ViewLocalizer["Home page"]</h1>
    <p>
        @HtmlLocalizer["Learn"]
    </p>
</div>

<form method="post" asp-page="Index">
    <div class="form-group">
        <label asp-for="Superhero" class="control-label"></label>
        <input asp-for="Superhero" class="form-control"/>
        <span asp-validation-for="Superhero" class="text-danger"></span>
    </div>
    <button type="submit">@Localizer["Submit"]</button>
</form>
```

<br />

If page uses Avalanche's services then <b>@inject</b> with <em>ITextLocalizer</em>, <em>ITextLocalizer&lt;T&gt;</em>, <em>IFileLocalizer</em>, <em>IFileLocalizer&lt;T&gt;</em>, <em>ILocalization</em> interfaces.
Use any '.' dot separated key notation in localization files.

There is extension method <b>.LocalizeHtml(args?)</b> and <b>.Localize(args?)</b> (Avalanche.Localization.Asp.dll) that adapt to <b>LocalizedHtmlString</b> and <b>LocalizedString</b> respective.

```html
@page
@using Avalanche.Localization
@using Microsoft.Extensions.Localization
@using Microsoft.AspNetCore.Mvc.Localization
@model IndexModel

@inject ITextLocalizer<IndexModel> Localizer

@{
    ViewData["Title"] = @Localizer["Home page"].Localize();
}

<div class="text-center">
    <h1 class="display-4">@Localizer["Home page"].LocalizeHtml()</h1>
    <p>
        @Localizer["Learn"].LocalizeHtml();
    </p>
</div>

<form method="post" asp-page="Index">
    <div class="form-group">
        <label asp-for="Superhero" class="control-label"></label>
        <input asp-for="Superhero" class="form-control"/>
        <span asp-validation-for="Superhero" class="text-danger"></span>
    </div>
    <button type="submit">@Localizer["Submit"].LocalizeHtml()</button>
</form>
```

> [!WARNING]
> Asp &lt;form&gt; element is based on <em>ResourceTypeAttribute</em> annotated property and direct <em>ResourceManager</em> reference. 
>
> Element localization doesn't go through DependencyInjection, IServiceProvider and IStringLocalizer stack and thus doesn't get localized. 
> There is inconsistent design, some parts are dependency injectible, others not.

```html
<form method="post" asp-page="Index">
    <div class="form-group">
        <label asp-for="Superhero" class="control-label"></label>
        <input asp-for="Superhero" class="form-control"/>
        <span asp-validation-for="Superhero" class="text-danger"></span>
    </div>
    <button type="submit">@Localizer["Submit"]</button>
</form>
```

If localization issues require [diagnosis](https://avalanche.fi/Avalanche.Core/Avalanche.Localization/docs/diagnostics/index.html), the reference for localization can be acquired in <em>Program.cs</em>.
```cs
IHostBuilder hostBuilder = CreateHostBuilder(args);
IHost host = hostBuilder.Build();
ILocalization localization = (ILocalization)host.Services.GetService(typeof(ILocalization));
```

<b>AspNetCore</b> support is experimental and under-tested.