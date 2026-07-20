using Domain.Dtos;
using Domain.Entities;
using Domain.Extensions;
using Domain.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp.Logging;
using WebApp.Middlewares;

namespace WebApp.Pages.Admin.Employees;

/// <summary>
/// Список работников и добавление нового
/// </summary>
public class IndexModel(
    IEmployeeService employeeService,
    IEquipmentService equipmentService,
    IAuditLogger auditLogger
    ) : PageModel
{
    [BindProperty]
    public string Name { get; set; } = string.Empty;

    [BindProperty]
    public Specialty Specialty { get; set; }

    [BindProperty]
    public Guid? EquipmentId { get; set; }

    [BindProperty]
    public string? Comment { get; set; }

    public List<EmployeeListItemDto> Employees { get; private set; } = [];
    public List<Domain.Entities.Equipment> EquipmentList { get; private set; } = [];

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        await LoadAsync(cancellationToken);
    }

    public async Task<IActionResult> OnPostCreateAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            TempData["Error"] = "Укажите имя работника";
            return RedirectToPage();
        }

        var employee = await employeeService.CreateAsync(new CreateEmployeeRequest
        {
            Name = Name.Trim(),
            Specialty = Specialty,
            EquipmentId = EquipmentId,
            Comment = string.IsNullOrWhiteSpace(Comment) ? null : Comment.Trim(),
        }, cancellationToken);

        var login = HttpContext.GetSessionData()?.Login ?? "?";
        await auditLogger.LogAsync(login, "Добавлен работник", $"{employee.Name} | {employee.Specialty.GetDisplayName()}");
        TempData["Success"] = $"Работник «{employee.Name}» добавлен";
        return RedirectToPage();
    }

    private async Task LoadAsync(CancellationToken cancellationToken)
    {
        Employees = (await employeeService.GetAllAsync(cancellationToken))
            .OrderBy(e => e.Name).ToList();
        EquipmentList = (await equipmentService.GetAllAsync(cancellationToken))
            .OrderBy(e => e.Name).ToList();
    }
}
