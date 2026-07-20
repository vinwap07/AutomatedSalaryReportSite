using Domain.Dtos;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApp.Logging;
using WebApp.Middlewares;

namespace WebApp.Pages.Admin.Absences;

/// <summary>
/// Справочник причин отсутствия
/// </summary>
public class IndexModel(
    IReasonForAbsenceService reasonForAbsenceService,
    IAuditLogger auditLogger
    ) : PageModel
{
    [BindProperty(SupportsGet = true)]
    public Guid? EditId { get; set; }

    [BindProperty]
    public string Name { get; set; } = string.Empty;

    public List<ReasonForAbsence> Items { get; private set; } = [];

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        Items = (await reasonForAbsenceService.GetAllAsync(cancellationToken))
            .OrderBy(r => r.Name).ToList();
    }

    public async Task<IActionResult> OnPostCreateAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            TempData["Error"] = "Укажите название причины";
            return RedirectToPage();
        }

        var reason = await reasonForAbsenceService.CreateAsync(new CreateReasonForAbsenceRequest { Name = Name.Trim() }, cancellationToken);
        await auditLogger.LogAsync(CurrentLogin, "Добавлена причина отсутствия", reason.Name);
        TempData["Success"] = $"Причина «{reason.Name}» добавлена";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostUpdateAsync(Guid id, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            TempData["Error"] = "Укажите название причины";
            return RedirectToPage(new { EditId = id });
        }

        try
        {
            var reason = await reasonForAbsenceService.UpdateAsync(new UpdateReasonForAbsenceRequest { Id = id, Name = Name.Trim() }, cancellationToken);
            await auditLogger.LogAsync(CurrentLogin, "Изменена причина отсутствия", reason.Name);
            TempData["Success"] = "Изменения сохранены";
        }
        catch (NotFoundException)
        {
            TempData["Error"] = "Причина не найдена";
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(Guid id, string name, CancellationToken cancellationToken)
    {
        try
        {
            await reasonForAbsenceService.DeleteAsync(id, cancellationToken);
            await auditLogger.LogAsync(CurrentLogin, "Удалена причина отсутствия", name);
            TempData["Success"] = $"Причина «{name}» удалена";
        }
        catch (DbUpdateException)
        {
            TempData["Error"] = $"Нельзя удалить «{name}» — в журнале есть записи с этой причиной";
        }

        return RedirectToPage();
    }

    private string CurrentLogin => HttpContext.GetSessionData()?.Login ?? "?";
}
