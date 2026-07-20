using Domain.Dtos;
using Domain.Exceptions;
using Domain.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp.Logging;
using WebApp.Middlewares;
using WebApp.Session.Service;

namespace WebApp.Pages;

public class LoginModel(
    IAuthService authService,
    ISessionService sessionService,
    IAuditLogger auditLogger
    ) : PageModel
{
    [BindProperty]
    public string Username { get; set; } = string.Empty;

    [BindProperty]
    public string Password { get; set; } = string.Empty;

    [BindProperty(SupportsGet = true)]
    public string? ReturnUrl { get; set; }

    public string? ErrorMessage { get; set; }

    public IActionResult OnGet()
    {
        if (HttpContext.GetSessionData() != null)
        {
            return RedirectToPage("/Index");
        }
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Введите логин и пароль";
            return Page();
        }

        try
        {
            var user = await authService.LoginAsync(
                new LoginData { Username = Username.Trim(), Password = Password },
                cancellationToken);
            await sessionService.SignInAsync(user, cancellationToken);
            await auditLogger.LogAsync(user.Login, "Вход в систему", $"Роль: {user.Role}");

            if (!string.IsNullOrEmpty(ReturnUrl) && ReturnUrl.StartsWith('/') && !ReturnUrl.StartsWith("//"))
            {
                return Redirect(ReturnUrl);
            }
            return RedirectToPage("/Index");
        }
        catch (UnauthorizedException)
        {
            ErrorMessage = "Неверный логин или пароль";
            return Page();
        }
    }
}
