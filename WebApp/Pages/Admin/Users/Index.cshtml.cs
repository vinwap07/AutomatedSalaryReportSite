using Domain.Dtos;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApp.Logging;
using WebApp.Middlewares;

namespace WebApp.Pages.Admin.Users;

/// <summary>
/// Учётные записи пользователей системы
/// </summary>
public class IndexModel(
    IUserService userService,
    IEmployeeService employeeService,
    IAuditLogger auditLogger
    ) : PageModel
{
    [BindProperty(SupportsGet = true)]
    public Guid? EditId { get; set; }

    [BindProperty]
    public string Login { get; set; } = string.Empty;

    [BindProperty]
    public string Password { get; set; } = string.Empty;

    [BindProperty]
    public Role Role { get; set; }

    public List<UserListItemDto> Users { get; private set; } = [];

    /// <summary>Имя работника, привязанного к учётной записи (по идентификатору пользователя)</summary>
    public Dictionary<Guid, string> EmployeeNamesByUserId { get; private set; } = [];

    public Guid? CurrentUserId => HttpContext.GetSessionData()?.UserId;

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        Users = (await userService.GetAllAsync(cancellationToken))
            .OrderBy(u => u.Login).ToList();
        EmployeeNamesByUserId = (await employeeService.GetAllAsync(cancellationToken))
            .Where(e => e.UserId != null)
            .ToDictionary(e => e.UserId!.Value, e => e.Name);
    }

    public async Task<IActionResult> OnPostCreateAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(Login) || string.IsNullOrWhiteSpace(Password))
        {
            TempData["Error"] = "Укажите логин и пароль";
            return RedirectToPage();
        }

        try
        {
            var user = await userService.CreateAsync(new CreateUserRequest
            {
                Login = Login.Trim(),
                Password = Password,
                Role = Role,
            }, cancellationToken);

            await auditLogger.LogAsync(CurrentLogin, "Создана учётная запись", $"{user.Login} | роль: {user.Role}");
            TempData["Success"] = $"Учётная запись «{user.Login}» создана";
        }
        catch (DbUpdateException)
        {
            TempData["Error"] = "Логин уже занят — выберите другой";
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostResetPasswordAsync(Guid id, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(Password))
        {
            TempData["Error"] = "Укажите новый пароль";
            return RedirectToPage(new { EditId = id });
        }

        try
        {
            var user = await userService.UpdateAsync(new UpdateUserRequest
            {
                Id = id,
                Password = Password,
            }, cancellationToken);

            await auditLogger.LogAsync(CurrentLogin, "Сброшен пароль учётной записи", user.Login);
            TempData["Success"] = $"Пароль для «{user.Login}» изменён";
        }
        catch (NotFoundException)
        {
            TempData["Error"] = "Учётная запись не найдена";
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(Guid id, string login, CancellationToken cancellationToken)
    {
        if (id == CurrentUserId)
        {
            TempData["Error"] = "Нельзя удалить собственную учётную запись";
            return RedirectToPage();
        }

        await userService.DeleteAsync(id, cancellationToken);
        await auditLogger.LogAsync(CurrentLogin, "Удалена учётная запись", login);
        TempData["Success"] = $"Учётная запись «{login}» удалена";
        return RedirectToPage();
    }

    private string CurrentLogin => HttpContext.GetSessionData()?.Login ?? "?";
}
