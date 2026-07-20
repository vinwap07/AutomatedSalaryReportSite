using Domain.Dtos;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApp.Logging;
using WebApp.Middlewares;

namespace WebApp.Pages.Admin.Employees;

/// <summary>
/// Редактирование работника и управление его учётной записью
/// </summary>
public class EditModel(
    IEmployeeService employeeService,
    IEquipmentService equipmentService,
    IUserService userService,
    IAuditLogger auditLogger
    ) : PageModel
{
    [BindProperty(SupportsGet = true)]
    public Guid Id { get; set; }

    [BindProperty]
    public string Name { get; set; } = string.Empty;

    [BindProperty]
    public Specialty Specialty { get; set; }

    [BindProperty]
    public Guid? EquipmentId { get; set; }

    [BindProperty]
    public string? Comment { get; set; }

    [BindProperty]
    public string? AccountLogin { get; set; }

    [BindProperty]
    public string? AccountPassword { get; set; }

    public EmployeeDetailsDto? Employee { get; private set; }
    public List<Domain.Entities.Equipment> EquipmentList { get; private set; } = [];

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        try
        {
            await LoadAsync(cancellationToken);
            Name = Employee!.Name;
            Specialty = Employee.Specialty;
            EquipmentId = Employee.EquipmentId;
            Comment = Employee.Comment;
            return Page();
        }
        catch (NotFoundException)
        {
            TempData["Error"] = "Работник не найден";
            return RedirectToPage("/Admin/Employees/Index");
        }
    }

    public async Task<IActionResult> OnPostSaveAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            TempData["Error"] = "Укажите имя работника";
            return RedirectToPage(new { Id });
        }

        try
        {
            var employee = await employeeService.UpdateAsync(new UpdateEmployeeRequest
            {
                Id = Id,
                Name = Name.Trim(),
                Specialty = Specialty,
                EquipmentId = EquipmentId,
                Comment = string.IsNullOrWhiteSpace(Comment) ? null : Comment.Trim(),
            }, cancellationToken);

            await auditLogger.LogAsync(CurrentLogin, "Изменён работник", employee.Name);
            TempData["Success"] = "Данные работника сохранены";
        }
        catch (NotFoundException)
        {
            TempData["Error"] = "Работник не найден";
            return RedirectToPage("/Admin/Employees/Index");
        }

        return RedirectToPage(new { Id });
    }

    public async Task<IActionResult> OnPostCreateAccountAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(AccountLogin) || string.IsNullOrWhiteSpace(AccountPassword))
        {
            TempData["Error"] = "Укажите логин и пароль для учётной записи";
            return RedirectToPage(new { Id });
        }

        var employee = await employeeService.GetByIdAsync(Id, cancellationToken);
        if (employee.UserId != null)
        {
            TempData["Error"] = "У работника уже есть учётная запись";
            return RedirectToPage(new { Id });
        }

        try
        {
            var user = await userService.CreateAsync(new CreateUserRequest
            {
                Login = AccountLogin.Trim(),
                Password = AccountPassword,
                Role = Role.User,
            }, cancellationToken);
            await employeeService.LinkUserAsync(Id, user.Id, cancellationToken);

            await auditLogger.LogAsync(CurrentLogin, "Создана учётная запись работника", $"{employee.Name} | логин: {user.Login}");
            TempData["Success"] = $"Учётная запись «{user.Login}» создана";
        }
        catch (DbUpdateException)
        {
            TempData["Error"] = "Логин уже занят — выберите другой";
        }

        return RedirectToPage(new { Id });
    }

    public async Task<IActionResult> OnPostResetPasswordAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(AccountPassword))
        {
            TempData["Error"] = "Укажите новый пароль";
            return RedirectToPage(new { Id });
        }

        var employee = await employeeService.GetByIdAsync(Id, cancellationToken);
        if (employee.UserId == null)
        {
            TempData["Error"] = "У работника нет учётной записи";
            return RedirectToPage(new { Id });
        }

        await userService.UpdateAsync(new UpdateUserRequest
        {
            Id = employee.UserId.Value,
            Password = AccountPassword,
        }, cancellationToken);

        await auditLogger.LogAsync(CurrentLogin, "Сброшен пароль учётной записи", $"{employee.Name} | логин: {employee.UserLogin}");
        TempData["Success"] = "Пароль изменён";
        return RedirectToPage(new { Id });
    }

    public async Task<IActionResult> OnPostDeleteAccountAsync(CancellationToken cancellationToken)
    {
        var employee = await employeeService.GetByIdAsync(Id, cancellationToken);
        if (employee.UserId == null)
        {
            TempData["Error"] = "У работника нет учётной записи";
            return RedirectToPage(new { Id });
        }

        await employeeService.LinkUserAsync(Id, null, cancellationToken);
        await userService.DeleteAsync(employee.UserId.Value, cancellationToken);

        await auditLogger.LogAsync(CurrentLogin, "Удалена учётная запись работника", $"{employee.Name} | логин: {employee.UserLogin}");
        TempData["Success"] = "Учётная запись удалена";
        return RedirectToPage(new { Id });
    }

    public async Task<IActionResult> OnPostDeleteAsync(CancellationToken cancellationToken)
    {
        try
        {
            var employee = await employeeService.GetByIdAsync(Id, cancellationToken);
            if (employee.UserId != null)
            {
                await employeeService.LinkUserAsync(Id, null, cancellationToken);
                await userService.DeleteAsync(employee.UserId.Value, cancellationToken);
            }
            await employeeService.DeleteAsync(Id, cancellationToken);

            await auditLogger.LogAsync(CurrentLogin, "Удалён работник", employee.Name);
            TempData["Success"] = $"Работник «{employee.Name}» удалён";
        }
        catch (NotFoundException)
        {
            TempData["Error"] = "Работник не найден";
        }

        return RedirectToPage("/Admin/Employees/Index");
    }

    private string CurrentLogin => HttpContext.GetSessionData()?.Login ?? "?";

    private async Task LoadAsync(CancellationToken cancellationToken)
    {
        Employee = await employeeService.GetByIdAsync(Id, cancellationToken);
        EquipmentList = (await equipmentService.GetAllAsync(cancellationToken))
            .OrderBy(e => e.Name).ToList();
    }
}
