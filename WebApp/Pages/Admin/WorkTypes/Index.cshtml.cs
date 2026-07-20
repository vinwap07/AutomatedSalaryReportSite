using Domain.Dtos;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApp.Logging;
using WebApp.Middlewares;

namespace WebApp.Pages.Admin.WorkTypes;

/// <summary>
/// Справочник видов работ
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

    [BindProperty(SupportsGet = true)]
    public string? Search { get; set; }

    public List<WorkType> Items { get; private set; } = [];

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        var items = (await workTypeService.GetAllAsync(cancellationToken))
            .OrderBy(w => w.Name);
        Items = string.IsNullOrWhiteSpace(Search)
            ? items.ToList()
            : items.Where(w => w.Name.Contains(Search.Trim(), StringComparison.OrdinalIgnoreCase)).ToList();
    }

    public async Task<IActionResult> OnPostCreateAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            TempData["Error"] = "Укажите название вида работы";
            return RedirectToPage();
        }

        var workType = await workTypeService.CreateAsync(new CreateWorkTypeRequest { Name = Name.Trim() }, cancellationToken);
        await auditLogger.LogAsync(CurrentLogin, "Добавлен вид работы", workType.Name);
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
            var workType = await workTypeService.UpdateAsync(new UpdateWorkTypeRequest { Id = id, Name = Name.Trim() }, cancellationToken);
            await auditLogger.LogAsync(CurrentLogin, "Изменён вид работы", workType.Name);
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

    private string CurrentLogin => HttpContext.GetSessionData()?.Login ?? "?";
}
