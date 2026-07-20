using Domain.Dtos;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp.Logging;
using WebApp.Middlewares;

namespace WebApp.Pages.Admin;

/// <summary>
/// Журнал выполненных работ (главная страница админки)
/// </summary>
public class IndexModel(
    IWorkLogService workLogService,
    IEmployeeService employeeService,
    IWorkTypeService workTypeService,
    IAuditLogger auditLogger
    ) : PageModel
{
    private const int MaxPageSize = 100_000;

    [BindProperty(SupportsGet = true)]
    public DateOnly? From { get; set; }

    [BindProperty(SupportsGet = true)]
    public DateOnly? To { get; set; }

    [BindProperty(SupportsGet = true)]
    public Guid? EmployeeId { get; set; }

    [BindProperty(SupportsGet = true)]
    public Guid? WorkTypeId { get; set; }

    public List<EmployeeListItemDto> Employees { get; private set; } = [];
    public List<WorkType> WorkTypes { get; private set; } = [];

    /// <summary>Записи, сгруппированные по дням (новые дни первыми)</summary>
    public List<(DateOnly Date, List<WorkLogListItemDto> Logs, decimal DaySum)> Days { get; private set; } = [];

    public decimal TotalSum { get; private set; }
    public int TotalHours { get; private set; }
    public int TotalRecords { get; private set; }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        From ??= new DateOnly(today.Year, today.Month, 1);
        To ??= today;

        Employees = (await employeeService.GetAllAsync(cancellationToken))
            .OrderBy(e => e.Name).ToList();
        WorkTypes = (await workTypeService.GetAllAsync(cancellationToken))
            .OrderBy(w => w.Name).ToList();

        var logs = await workLogService.GetByFiltersAsync(new WorkLogFilters
        {
            From = From,
            To = To,
            EmployeeId = EmployeeId,
            WorkTypeId = WorkTypeId,
            PageSize = MaxPageSize,
        }, cancellationToken);

        Days = logs
            .GroupBy(l => l.Date)
            .OrderByDescending(g => g.Key)
            .Select(g => (
                Date: g.Key,
                Logs: g.OrderBy(l => l.Employee.Name).ThenBy(l => l.WorkType?.Name).ToList(),
                DaySum: g.Sum(l => l.WorkCost?.Total ?? 0)))
            .ToList();

        TotalSum = Days.Sum(d => d.DaySum);
        TotalHours = Days.Sum(d => d.Logs.Sum(l => l.WorkHours ?? 0));
        TotalRecords = Days.Sum(d => d.Logs.Count);
    }

    public async Task<IActionResult> OnPostDeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var log = await workLogService.GetByIdAsync(id, cancellationToken);
            await workLogService.DeleteAsync(id, cancellationToken);

            var login = HttpContext.GetSessionData()?.Login ?? "?";
            await auditLogger.LogAsync(login, "Удалена запись о работе",
                $"{log.Date:yyyy-MM-dd} | {log.Employee.Name} | {log.WorkType?.Name ?? log.ReasonForAbsence?.Name ?? "-"}");
            TempData["Success"] = "Запись удалена";
        }
        catch (NotFoundException)
        {
            TempData["Error"] = "Запись не найдена";
        }

        return RedirectToPage(new
        {
            From = From?.ToString("yyyy-MM-dd"),
            To = To?.ToString("yyyy-MM-dd"),
            EmployeeId,
            WorkTypeId,
        });
    }
}
