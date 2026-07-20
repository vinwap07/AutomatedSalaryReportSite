using Domain.Dtos;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp.Logging;
using WebApp.Middlewares;

namespace WebApp.Pages.Admin.Equipment;

/// <summary>
/// Справочник техники
/// </summary>
public class IndexModel(
    IEquipmentService equipmentService,
    IAuditLogger auditLogger
    ) : PageModel
{
    [BindProperty(SupportsGet = true)]
    public Guid? EditId { get; set; }

    [BindProperty]
    public string Name { get; set; } = string.Empty;

    [BindProperty]
    public bool HasTracker { get; set; }

    public List<Domain.Entities.Equipment> Items { get; private set; } = [];

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        Items = (await equipmentService.GetAllAsync(cancellationToken))
            .OrderBy(e => e.Name).ToList();
    }

    public async Task<IActionResult> OnPostCreateAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            TempData["Error"] = "Укажите название техники";
            return RedirectToPage();
        }

        var equipment = await equipmentService.CreateAsync(new CreateEquipmentRequest
        {
            Name = Name.Trim(),
            HasTracker = HasTracker,
        }, cancellationToken);

        await auditLogger.LogAsync(CurrentLogin, "Добавлена техника", equipment.Name);
        TempData["Success"] = $"Техника «{equipment.Name}» добавлена";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostUpdateAsync(Guid id, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            TempData["Error"] = "Укажите название техники";
            return RedirectToPage(new { EditId = id });
        }

        try
        {
            var equipment = await equipmentService.UpdateAsync(new UpdateEquipmentRequest
            {
                Id = id,
                Name = Name.Trim(),
                HasTracker = HasTracker,
            }, cancellationToken);

            await auditLogger.LogAsync(CurrentLogin, "Изменена техника", equipment.Name);
            TempData["Success"] = "Изменения сохранены";
        }
        catch (NotFoundException)
        {
            TempData["Error"] = "Техника не найдена";
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(Guid id, string name, CancellationToken cancellationToken)
    {
        await equipmentService.DeleteAsync(id, cancellationToken);
        await auditLogger.LogAsync(CurrentLogin, "Удалена техника", name);
        TempData["Success"] = $"Техника «{name}» удалена";
        return RedirectToPage();
    }

    private string CurrentLogin => HttpContext.GetSessionData()?.Login ?? "?";
}
