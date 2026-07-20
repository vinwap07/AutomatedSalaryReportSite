using Domain.Dtos;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Services.Abstractions;
using Domain.ValueObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp.Logging;
using WebApp.Middlewares;

namespace WebApp.Pages.Admin;

/// <summary>
/// Редактирование одной записи о работе
/// </summary>
public class EditWorkLogModel(
    IWorkLogService workLogService,
    IEmployeeService employeeService,
    IWorkTypeService workTypeService,
    IReasonForAbsenceService reasonForAbsenceService,
    IAuditLogger auditLogger
    ) : PageModel
{
    [BindProperty(SupportsGet = true)]
    public Guid Id { get; set; }

    [BindProperty]
    public DateOnly Date { get; set; }

    [BindProperty]
    public Guid EmployeeId { get; set; }

    [BindProperty]
    public Guid? WorkTypeId { get; set; }

    [BindProperty]
    public int? WorkHours { get; set; }

    [BindProperty]
    public decimal? Rate { get; set; }

    [BindProperty]
    public decimal? Volume { get; set; }

    [BindProperty]
    public UnitOfMeasure Unit { get; set; }

    [BindProperty]
    public Guid? ReasonForAbsenceId { get; set; }

    public List<EmployeeListItemDto> Employees { get; private set; } = [];
    public List<WorkType> WorkTypes { get; private set; } = [];
    public List<ReasonForAbsence> Reasons { get; private set; } = [];
    public List<string> Errors { get; } = [];

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        await LoadListsAsync(cancellationToken);

        try
        {
            var log = await workLogService.GetByIdAsync(Id, cancellationToken);
            Date = log.Date;
            EmployeeId = log.Employee.Id;
            WorkTypeId = log.WorkType?.Id;
            WorkHours = log.WorkHours;
            Rate = log.WorkCost?.Rate;
            Volume = log.WorkCost?.Volume;
            Unit = log.WorkCost?.UnitOfMeasure ?? UnitOfMeasure.Ton;
            ReasonForAbsenceId = log.ReasonForAbsence?.Id;
            return Page();
        }
        catch (NotFoundException)
        {
            TempData["Error"] = "Запись не найдена";
            return RedirectToPage("/Admin/Index");
        }
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        await LoadListsAsync(cancellationToken);

        if (EmployeeId == Guid.Empty)
        {
            Errors.Add("Выберите работника");
        }
        if (WorkTypeId == null && ReasonForAbsenceId == null)
        {
            Errors.Add("Выберите вид работы или причину отсутствия");
        }
        if (WorkTypeId != null && ReasonForAbsenceId != null)
        {
            Errors.Add("Укажите либо работу, либо причину отсутствия, но не оба поля сразу");
        }
        if (ReasonForAbsenceId != null && (WorkHours != null || Rate != null || Volume != null))
        {
            Errors.Add("Для отсутствия не указываются часы, расценка и объём");
        }
        if (Rate != null ^ Volume != null)
        {
            Errors.Add("Для расчёта суммы нужны и расценка, и объём");
        }

        if (Errors.Count > 0)
        {
            return Page();
        }

        try
        {
            var updated = await workLogService.UpdateAsync(new UpdateWorkLogRequest
            {
                Id = Id,
                EmployeeId = EmployeeId,
                Date = Date,
                WorkTypeId = WorkTypeId,
                WorkHours = WorkHours,
                WorkCost = Rate != null && Volume != null ? new WorkCost(Rate.Value, Volume.Value, Unit) : null,
                ReasonForAbsenceId = ReasonForAbsenceId,
            }, cancellationToken);

            var login = HttpContext.GetSessionData()?.Login ?? "?";
            await auditLogger.LogAsync(login, "Изменена запись о работе",
                $"{updated.Date:yyyy-MM-dd} | {updated.Employee.Name} | {updated.WorkType?.Name ?? updated.ReasonForAbsence?.Name ?? "-"}");

            TempData["Success"] = "Запись сохранена";
            return RedirectToPage("/Admin/Index", new
            {
                From = Date.ToString("yyyy-MM-dd"),
                To = Date.ToString("yyyy-MM-dd"),
            });
        }
        catch (NotFoundException)
        {
            TempData["Error"] = "Запись не найдена";
            return RedirectToPage("/Admin/Index");
        }
    }

    public async Task<IActionResult> OnPostDeleteAsync(CancellationToken cancellationToken)
    {
        try
        {
            var log = await workLogService.GetByIdAsync(Id, cancellationToken);
            await workLogService.DeleteAsync(Id, cancellationToken);

            var login = HttpContext.GetSessionData()?.Login ?? "?";
            await auditLogger.LogAsync(login, "Удалена запись о работе",
                $"{log.Date:yyyy-MM-dd} | {log.Employee.Name} | {log.WorkType?.Name ?? log.ReasonForAbsence?.Name ?? "-"}");
            TempData["Success"] = "Запись удалена";
        }
        catch (NotFoundException)
        {
            TempData["Error"] = "Запись не найдена";
        }

        return RedirectToPage("/Admin/Index");
    }

    private async Task LoadListsAsync(CancellationToken cancellationToken)
    {
        Employees = (await employeeService.GetAllAsync(cancellationToken))
            .OrderBy(e => e.Name).ToList();
        WorkTypes = (await workTypeService.GetAllAsync(cancellationToken))
            .OrderBy(w => w.Name).ToList();
        Reasons = (await reasonForAbsenceService.GetAllAsync(cancellationToken))
            .OrderBy(r => r.Name).ToList();
    }
}
