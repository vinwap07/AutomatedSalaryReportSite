using Domain.Dtos;
using Domain.Entities;
using Domain.Services.Abstractions;
using Domain.ValueObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp.Logging;
using WebApp.Middlewares;

namespace WebApp.Pages.Admin;

/// <summary>
/// Внесение работ за день (как в экселе): таблица строк с выпадающими списками,
/// кнопки "повторить работников" и "повторить день" копируют данные предыдущего дня
/// </summary>
public class AddDayModel(
    IWorkLogService workLogService,
    IEmployeeService employeeService,
    IWorkTypeService workTypeService,
    IReasonForAbsenceService reasonForAbsenceService,
    IAuditLogger auditLogger
    ) : PageModel
{
    private const int MaxPageSize = 100_000;

    /// <summary>
    /// Одна строка ввода (работа или отсутствие одного работника)
    /// </summary>
    public class RowInput
    {
        public Guid? EmployeeId { get; set; }
        public Guid? WorkTypeId { get; set; }
        public int? WorkHours { get; set; }
        public decimal? Rate { get; set; }
        public decimal? Volume { get; set; }
        public UnitOfMeasure? Unit { get; set; }
        public Guid? ReasonForAbsenceId { get; set; }

        public bool IsEmpty =>
            EmployeeId == null && WorkTypeId == null && WorkHours == null &&
            Rate == null && Volume == null && ReasonForAbsenceId == null;
    }

    [BindProperty(SupportsGet = true)]
    public DateOnly Date { get; set; }

    [BindProperty]
    public List<RowInput> Rows { get; set; } = [];

    public List<EmployeeListItemDto> Employees { get; private set; } = [];
    public List<WorkType> WorkTypes { get; private set; } = [];
    public List<ReasonForAbsence> Reasons { get; private set; } = [];

    /// <summary>Последний день с записями до выбранной даты (источник для копирования)</summary>
    public DateOnly? PreviousDay { get; private set; }

    public List<string> RowErrors { get; } = [];

    public async Task OnGetAsync(string? prefill, CancellationToken cancellationToken)
    {
        if (Date == default)
        {
            Date = DateOnly.FromDateTime(DateTime.Today);
        }

        await LoadListsAsync(cancellationToken);
        PreviousDay = await FindPreviousDayAsync(cancellationToken);

        if (PreviousDay != null && prefill is "employees" or "full")
        {
            var previousLogs = (await workLogService.GetByFiltersAsync(new WorkLogFilters
            {
                From = PreviousDay,
                To = PreviousDay,
                PageSize = MaxPageSize,
            }, cancellationToken)).OrderBy(l => l.Employee.Name).ToList();

            Rows = prefill == "employees"
                ? previousLogs
                    .Select(l => l.Employee.Id)
                    .Distinct()
                    .Select(id => new RowInput { EmployeeId = id })
                    .ToList()
                : previousLogs
                    .Select(l => new RowInput
                    {
                        EmployeeId = l.Employee.Id,
                        WorkTypeId = l.WorkType?.Id,
                        WorkHours = l.WorkHours,
                        Rate = l.WorkCost?.Rate,
                        Volume = l.WorkCost?.Volume,
                        Unit = l.WorkCost?.UnitOfMeasure,
                        ReasonForAbsenceId = l.ReasonForAbsence?.Id,
                    })
                    .ToList();
        }

        if (Rows.Count == 0)
        {
            Rows = [new RowInput(), new RowInput(), new RowInput()];
        }
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        await LoadListsAsync(cancellationToken);
        PreviousDay = await FindPreviousDayAsync(cancellationToken);

        var filledRows = Rows.Where(r => !r.IsEmpty).ToList();
        if (filledRows.Count == 0)
        {
            RowErrors.Add("Заполните хотя бы одну строку");
            return Page();
        }

        for (var i = 0; i < filledRows.Count; i++)
        {
            var row = filledRows[i];
            var rowNumber = i + 1;
            if (row.EmployeeId == null)
            {
                RowErrors.Add($"Строка {rowNumber}: выберите работника");
            }
            if (row.WorkTypeId == null && row.ReasonForAbsenceId == null)
            {
                RowErrors.Add($"Строка {rowNumber}: выберите вид работы или причину отсутствия");
            }
            if (row.Rate != null ^ row.Volume != null)
            {
                RowErrors.Add($"Строка {rowNumber}: для расчёта суммы нужны и расценка, и объём");
            }
        }

        if (RowErrors.Count > 0)
        {
            Rows = filledRows;
            return Page();
        }

        var login = HttpContext.GetSessionData()?.Login ?? "?";
        var employeesById = Employees.ToDictionary(e => e.Id);

        foreach (var row in filledRows)
        {
            var workCost = row.Rate != null && row.Volume != null
                ? new WorkCost(row.Rate.Value, row.Volume.Value, row.Unit ?? UnitOfMeasure.Ton)
                : null;

            await workLogService.CreateAsync(new CreateWorkLogRequest
            {
                EmployeeId = row.EmployeeId!.Value,
                Date = Date,
                WorkTypeId = row.WorkTypeId,
                WorkHours = row.WorkHours,
                WorkCost = workCost,
                ReasonForAbsenceId = row.ReasonForAbsenceId,
            }, cancellationToken);

            var employeeName = employeesById.TryGetValue(row.EmployeeId.Value, out var employee) ? employee.Name : "?";
            var workName = row.WorkTypeId != null
                ? WorkTypes.FirstOrDefault(w => w.Id == row.WorkTypeId)?.Name ?? "?"
                : Reasons.FirstOrDefault(r => r.Id == row.ReasonForAbsenceId)?.Name ?? "?";
            await auditLogger.LogAsync(login, "Добавлена запись о работе",
                $"{Date:yyyy-MM-dd} | {employeeName} | {workName} | часы: {row.WorkHours?.ToString() ?? "-"} | сумма: {(workCost?.Total.ToString("0.##") ?? "-")}");
        }

        TempData["Success"] = $"Добавлено записей: {filledRows.Count} (за {Date:dd.MM.yyyy})";
        return RedirectToPage("/Admin/Index", new
        {
            From = Date.ToString("yyyy-MM-dd"),
            To = Date.ToString("yyyy-MM-dd"),
        });
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

    private async Task<DateOnly?> FindPreviousDayAsync(CancellationToken cancellationToken)
    {
        var earlierLogs = await workLogService.GetByFiltersAsync(new WorkLogFilters
        {
            To = Date.AddDays(-1),
            PageSize = MaxPageSize,
        }, cancellationToken);
        var dates = earlierLogs.Select(l => l.Date).ToList();
        return dates.Count > 0 ? dates.Max() : null;
    }
}
