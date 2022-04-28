namespace samples.asp.Models;
using System.Globalization;

/// <summary>Culture assignment model</summary>
public record CultureAssignmentModel(CultureInfo CurrentUICulture, IList<CultureInfo> SupportedCultures);
