using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp.Middlewares;

namespace WebApp.Pages;

public class IndexModel : PageModel
{
    public IActionResult OnGet()
    {
        var session = HttpContext.GetSessionData();
        if (session == null)
        {
            return RedirectToPage("/Login");
        }

        return session.Role == Role.Admin
            ? RedirectToPage("/Admin/Index")
            : RedirectToPage("/Reports/Index");
    }
}
