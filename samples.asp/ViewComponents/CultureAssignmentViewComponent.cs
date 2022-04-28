namespace samples.asp.ViewComponents;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using samples.asp.Models;

/// <summary></summary>
public class CultureAssignmentViewComponent : ViewComponent
{
    /// <summary>Options</summary>
    IOptions<RequestLocalizationOptions> localizationOptions;

    /// <summary>Create component</summary>
    public CultureAssignmentViewComponent(IOptions<RequestLocalizationOptions> localizationOptions)
    {
        this.localizationOptions = localizationOptions ?? throw new ArgumentNullException(nameof(localizationOptions));
    }

    /// <summary>Assign culture</summary>
    public IViewComponentResult Invoke()
    {
        // Get culture feature with assigned culture
        IRequestCultureFeature cultureFeature = HttpContext.Features.Get<IRequestCultureFeature>()!;
        // Create model with culture assignment record
        CultureAssignmentModel model = new CultureAssignmentModel(
            CurrentUICulture: cultureFeature.RequestCulture.UICulture,
            SupportedCultures: localizationOptions.Value.SupportedUICultures?.ToArray() ?? Array.Empty<CultureInfo>()
        );
        // Return partial view
        return View(model);
    }
}
