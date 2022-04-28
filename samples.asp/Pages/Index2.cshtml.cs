namespace samples.asp.Pages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class Index2Model : PageModel
{
    protected ILogger<IndexModel> logger;

    public Index2Model(ILogger<IndexModel> logger)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public void OnGet()
    {

    }
}
