using Domain.Dtos;
using Domain.Entities;
using Domain.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp.Middlewares;

namespace WebApp.Pages.Reports;

/// <summary>
/// Отчёт по работам за период: работник видит только свои работы,
/// администратор — всех или выбранного работника
/// </summary>
public class IndexModel(
    IWorkLogService workLogService,
    IEmployeeService employeeService
    ) : PageModel
{
    private const int MaxPageSize = 100_000;

    [BindProperty(SupportsGet = true)]
    public DateOnly? From { get; set; }

    [BindProperty(SupportsGet = true)]
    public DateOnly? To { get; set; }

    [BindProperty(SupportsGet = true)]
    public Guid? EmployeeId { get; set; }

    public bool IsAdmin { get; private set; }

    /// <summary>Работник текущего пользователя (для роли "работник")</summary>
    public EmployeeListItemDto? OwnEmployee { get; private set; }

    public List<EmployeeListItemDto> Employees { get; private set; } = [];

    /// <summary>Показывать ли колонку с именем работника (админ смотрит всех)</summary>
    public bool ShowEmployeeColumn => IsAdmin && EmployeeId == null;

    public List<(DateOnly Date, List<WorkLogListItemDto> Logs, decimal DaySum, int DayHours)> Days { get; private set; } = [];

    /// <summary>Сводка по работникам за период (для админа при просмотре всех)</summary>
    public List<(string Name, int WorkDays, int Hours, decimal Sum, int Absences)> EmployeeSummary { get; private set; } = [];

    public decimal TotalSum { get; private set; }
    public int TotalHours { get; private set; }
    public int WorkDaysCount { get; private set; }
    public int AbsenceCount { get; private set; }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        var session = HttpContext.GetSessionData();
        IsAdmin = session?.Role == Role.Admin;

        var today = DateOnly.FromDateTime(DateTime.Today);
        From ??= new DateOnly(today.Year, today.Month, 1);
        To ??= today;

        if (IsAdmin)
        {
            Employees = (await employeeService.GetAllAsync(cancellationToken))
                .OrderBy(e => e.Name).ToList();
        }
        else
        {
            OwnEmployee = (await employeeService.GetByFiltersAsync(
                    new EmployeeFilters { UserId = session!.UserId },
                    cancellationToken))
                .FirstOrDefault();
            if (OwnEmployee == null)
            {
                return;
            }
            EmployeeId = OwnEmployee.Id;
        }

        var logs = (await workLogService.GetByFiltersAsync(new WorkLogFilters
        {
            From = From,
            To = To,
            EmployeeId = EmployeeId,
            PageSize = MaxPageSize,
        }, cancellationToken)).ToList();

        Days = logs
            .GroupBy(l => l.Date)
            .OrderByDescending(g => g.Key)
            .Select(g => (
                Date: g.Key,
                Logs: g.OrderBy(l => l.Employee.Name).ThenBy(l => l.WorkType?.Name).ToList(),
                DaySum: g.Sum(l => l.WorkCost?.Total ?? 0),
                DayHours: g.Sum(l => l.WorkHours ?? 0)))
            .ToList();

        TotalSum = Days.Sum(d => d.DaySum);
        TotalHours = Days.Sum(d => d.DayHours);
        WorkDaysCount = logs.Where(l => l.ReasonForAbsence == null).Select(l => l.Date).Distinct().Count();
        AbsenceCount = logs.Count(l => l.ReasonForAbsence != null);

        if (ShowEmployeeColumn)
        {
            EmployeeSummary = logs
                .GroupBy(l => l.Employee.Name)
                .Select(g => (
                    Name: g.Key,
                    WorkDays: g.Where(l => l.ReasonForAbsence == null).Select(l => l.Date).Distinct().Count(),
                    Hours: g.Sum(l => l.WorkHours ?? 0),
                    Sum: g.Sum(l => l.WorkCost?.Total ?? 0),
                    Absences: g.Count(l => l.ReasonForAbsence != null)))
                .OrderBy(s => s.Name)
                .ToList();
        }
    }
}
