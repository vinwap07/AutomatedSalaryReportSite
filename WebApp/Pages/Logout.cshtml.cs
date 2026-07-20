using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp.Session.Service;

namespace WebApp.Pages;

public class LogoutModel(ISessionService sessionService) : PageModel
{
    public IActionResult OnGet()
    {
        return RedirectToPage("/Index");
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        await sessionService.SignOutAsync(cancellationToken);
        return RedirectToPage("/Login");
    }
}
