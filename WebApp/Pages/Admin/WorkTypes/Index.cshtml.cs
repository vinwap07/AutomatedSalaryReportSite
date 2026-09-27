using Domain.Dtos;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Extensions;
using Domain.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApp.Logging;
using WebApp.Middlewares;

namespace WebApp.Pages.Admin.WorkTypes;

/// <summary>
/// Справочник видов работ: работы специалистов сгруппированы по специальностям
/// (неназначенные первыми), работы разнорабочих — в отдельной колонке
/// </summary>
public class IndexModel(
    IWorkTypeService workTypeService,
    IAuditLogger auditLogger
    ) : PageModel
{
    [BindProperty(SupportsGet = true)]
    public Guid? EditId { get; set; }

    [BindProperty]
    public string Name { get; set; } = string.Empty;

    /// <summary>Специальность вида работы (null — не назначена)</summary>
    [BindProperty]
    public Specialty? Specialty { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Search { get; set; }

    /// <summary>Работы специалистов и неназначенные, по группам специальностей</summary>
    public List<IGrouping<Specialty?, WorkType>> SpecialistGroups { get; private set; } = [];

    /// <summary>Работы разнорабочих</summary>
    public List<WorkType> LaborerItems { get; private set; } = [];

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        var items = await workTypeService.GetAllAsync(cancellationToken);
        if (!string.IsNullOrWhiteSpace(Search))
        {
            items = items.Where(w => w.Name.Contains(Search.Trim(), StringComparison.OrdinalIgnoreCase));
        }

        var groups = items.GroupBySpecialty();
        SpecialistGroups = groups.Where(g => g.Key != Domain.Entities.Specialty.Laborer).ToList();
        LaborerItems = groups.FirstOrDefault(g => g.Key == Domain.Entities.Specialty.Laborer)?.ToList() ?? [];
    }

    public async Task<IActionResult> OnPostCreateAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            TempData["Error"] = "Укажите название вида работы";
            return RedirectToPage();
        }

        var workType = await workTypeService.CreateAsync(new CreateWorkTypeRequest
        {
            Name = Name.Trim(),
            Specialty = Specialty,
        }, cancellationToken);
        await auditLogger.LogAsync(CurrentLogin, "Добавлен вид работы", $"{workType.Name} | {SpecialtyName(workType.Specialty)}");
        TempData["Success"] = $"Вид работы «{workType.Name}» добавлен";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostUpdateAsync(Guid id, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            TempData["Error"] = "Укажите название вида работы";
            return RedirectToPage(new { EditId = id });
        }

        try
        {
            var workType = await workTypeService.UpdateAsync(new UpdateWorkTypeRequest
            {
                Id = id,
                Name = Name.Trim(),
                Specialty = Specialty,
            }, cancellationToken);
            await auditLogger.LogAsync(CurrentLogin, "Изменён вид работы", $"{workType.Name} | {SpecialtyName(workType.Specialty)}");
            TempData["Success"] = "Изменения сохранены";
        }
        catch (NotFoundException)
        {
            TempData["Error"] = "Вид работы не найден";
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(Guid id, string name, CancellationToken cancellationToken)
    {
        try
        {
            await workTypeService.DeleteAsync(id, cancellationToken);
            await auditLogger.LogAsync(CurrentLogin, "Удалён вид работы", name);
            TempData["Success"] = $"Вид работы «{name}» удалён";
        }
        catch (DbUpdateException)
        {
            TempData["Error"] = $"Нельзя удалить «{name}» — в журнале есть записи с этим видом работы";
        }

        return RedirectToPage();
    }

    private static string SpecialtyName(Specialty? specialty) => specialty?.GetDisplayName() ?? "без специальности";

    private string CurrentLogin => HttpContext.GetSessionData()?.Login ?? "?";
}
